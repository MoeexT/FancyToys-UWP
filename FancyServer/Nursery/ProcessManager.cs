using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class ProcessManager {
        public delegate void ProcessAddHandler(ProcessInfo ps);

        public delegate void ProcessLaunchedHandler(ProcessInfo ps);

        public delegate void ProcessExitedHandler(ProcessInfo ps);

        public delegate void ProcessRemovedHandler(ProcessInfo ps);

        public event ProcessAddHandler OnProcessAdd;
        public event ProcessLaunchedHandler OnProcessLaunched;
        public event ProcessExitedHandler OnProcessExited;
        public event ProcessRemovedHandler OnProcessRemoved;

        /// <summary>
        /// pid and itself
        /// </summary>
        public Dictionary<int, ProcessInfo> Processes { get; }

        /// <summary>
        /// restart process automatically after a process exited 
        /// </summary>
        public bool RestartOnExit { get; set; } = false;

        private int ID;

        public ProcessManager() { Processes = new Dictionary<int, ProcessInfo>(); }

        public ProcessInfo Add(string pathName) {
            if (!File.Exists(pathName)) {
                Logger.Error($"No such file: ${pathName}");
                return null;
            }

            Process child = InitProcess(pathName);
            Processes[ID] = new ProcessInfo(ID, child, Path.GetFileName(pathName), OnProcessExited);
            OnProcessAdd?.Invoke(Processes[ID]);
            Logger.Info($"Add {Path.GetFileName(pathName)} succeed.");

            return Processes[ID++];
        }

        public ProcessInfo PatchArgs(int pid, string args) {
            if (!Processes.TryGetValue(pid, out ProcessInfo info)) return null;
            info.Pcs.StartInfo.Arguments = args;
            return info;
        }

        public ProcessInfo Launch(int pid) {
            // no process via such `pid`
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"Process {Processes[pid].Alias} doesn't exist.");
                return null;
            }

            // process is already running
            if ((bool)Processes?[pid].IsRunning) {
                Logger.Warn($"Process {Processes[pid].Alias} is running");
                return null;
            }

            Process ps = Processes[pid].Pcs;
            bool launchSucceed = ps.Start();

            // launch failed
            if (!launchSucceed) {
                Dialogger.Dialog("Error", $"Process launch failed: {Processes[pid].Alias}");
                return null;
            }

            try {
                ps.BeginErrorReadLine();
                ps.BeginOutputReadLine();
            } catch (InvalidOperationException e) { Logger.Error($"[{ps.ProcessName}]{e.Message}"); }

            Processes[pid].IsRunning = true;
            Processes[pid].Alias = ps.ProcessName;
            OnProcessLaunched?.Invoke(Processes[ps.Id]);
            Logger.Info($"Process {ps.ProcessName}[{ps.Id}] launched successfully.");

            return Processes[pid];
        }

        public ProcessInfo Stop(int pid) {
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"No such process with PID: {pid}");
                return null;
            }
            Process ps = Processes[pid].Pcs;

            try {
                ps.CancelErrorRead();
                ps.CancelOutputRead();
            } catch (InvalidOperationException e) { Logger.Warn(e.Data.ToString()); }

            ps.Kill();
            return Processes[pid];
        }

        public ProcessInfo Remove(int pid) {
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"Process({pid}) doesn't exist");
                return null;
            }

            if (!Processes[pid].Pcs.HasExited) { Processes[pid].Pcs.Kill(); }
            OnProcessRemoved?.Invoke(Processes[pid]);
            Processes.Remove(pid);
            return Processes[pid];
        }

        public ProcessInfo SetAutoRestart(int pid, bool autoRestart) {
            if (!Processes.TryGetValue(pid, out ProcessInfo pi)) return null;
            pi.AutoRestart = autoRestart;
            return pi;
        }

        /// <summary>
        /// add a original process and init its start info or something else
        /// </summary>
        /// <param name="pathName">file's full name with path</param>
        private static Process InitProcess(string pathName) {
            Process child = new Process();
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.WorkingDirectory = Path.GetDirectoryName(pathName) ?? string.Empty;
            child.EnableRaisingEvents = true;

            child.OutputDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdLogger.StdOutput((s as Process).Id, e.Data);
            };
            child.ErrorDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdLogger.StdError((s as Process).Id, e.Data);
            };

            return child;
        }

        private void OnProcessDataLoaded(object sender) { }

        private void BeforeProcessDataLoad(object sender) { }

    }

    public class ProcessInfo {
        public readonly int Id;
        public bool IsRunning;
        public bool AutoRestart;
        public string Alias; // process name, after process launched
        public readonly Process Pcs;

        public ProcessInfo(int id, Process ps, string alias, ProcessManager.ProcessExitedHandler processExited) {
            Id = id;
            Alias = alias;
            Pcs = ps;
            Pcs.Exited += (sender, _) => {
                if (!(sender is Process)) return;

                if (AutoRestart) {
                    Logger.Info(Pcs.Start()
                        ? $"Restart {Pcs.ProcessName}({id}) successfully."
                        : $"Restart {Pcs.ProcessName}({id}) failed.");
                } else {
                    IsRunning = false;
                    processExited?.Invoke(this);
                    Logger.Info($"Process {alias} exited.");
                }
            };
        }
    }

}