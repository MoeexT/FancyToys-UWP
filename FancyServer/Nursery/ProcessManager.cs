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
        private Dictionary<int, ProcessInfo> Processes { get; set; }

        private int ID;

        public ProcessManager() { Processes = new Dictionary<int, ProcessInfo>(); }

        public ProcessInfo Add(string pathName, string args) {
            if (!File.Exists(pathName)) {
                Logger.Error($"No such file: ${pathName}");
                return null;
            }
            
            Process child = InitProcess(pathName, args);
            Processes[ID] = new ProcessInfo(ID, child, Path.GetFileName(pathName), OnProcessExited);
            OnProcessAdd?.Invoke(Processes[ID]);
            Logger.Info($"Add {Path.GetFileName(pathName)} succeed.");
            
            return Processes[ID++];
        }

        public ProcessInfo Launch(int pid) {
            // no process via such `pid`
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"Process {Processes[pid].alias} doesn't exist.");
                return null;
            }

            // process is already running
            if ((bool)Processes?[pid].isRunning) {
                Logger.Warn($"Process {Processes[pid].alias} is running");
                return null;
            }

            Process ps = Processes[pid].process;
            bool launchSucceed = ps.Start();

            // launch failed
            if (!launchSucceed) {
                Dialogger.Dialog("Error", $"Process launch failed: {Processes[pid].alias}");
                return null;
            }

            try {
                ps.BeginErrorReadLine();
                ps.BeginOutputReadLine();
            } catch (InvalidOperationException e) {
                Logger.Error($"[{ps.ProcessName}]{e.Message}");
            }

            Processes[pid].isRunning = true;
            Processes[pid].alias = ps.ProcessName;
            OnProcessLaunched?.Invoke(Processes[ps.Id]);
            Logger.Info($"Process {ps.ProcessName}[{ps.Id}] launched successfully.");
            
            return Processes[pid];
        }

        public ProcessInfo Stop(int pid) {
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"No such process with PID: {pid}");
                return null;
            }
            Process ps = Processes[pid].process;

            try {
                ps.CancelErrorRead();
                ps.CancelOutputRead();
            } catch (InvalidOperationException e) {
                Logger.Warn(e.Data.ToString());
            }

            ps.Kill();
            return Processes[pid];
        }

        public ProcessInfo Remove(int pid) {
            if (!Processes.ContainsKey(pid)) {
                Logger.Error($"Process({pid}) doesn't exist");
                return null;
            }

            if (!Processes[pid].process.HasExited) {
                Processes[pid].process.Kill();
            }
            OnProcessRemoved?.Invoke(Processes[pid]);
            Processes.Remove(pid);
            return Processes[pid];
        }

        /// <summary>
        /// add a original process and init its start info or something else
        /// </summary>
        /// <param name="pathName">file's full name with path</param>
        /// <param name="args">arguments</param>
        private static Process InitProcess(string pathName, string args) {
            Process child = new();
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.Arguments = args;
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.WorkingDirectory = Path.GetDirectoryName(pathName) ?? string.Empty;
            child.EnableRaisingEvents = true;

            child.OutputDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdLogger.StdOutput((s as Process)!.Id, e.Data);
            };
            child.ErrorDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdLogger.StdError((s as Process)!.Id, e.Data);
            };
            
            return child;
        }

        private void OnProcessDataLoaded(object sender) { }

        private void BeforeProcessDataLoad(object sender) { }

    }

    public class ProcessInfo {
        public readonly int id;
        public bool isRunning;
        public string alias; // process name, after process launched
        public readonly Process process;

        public ProcessInfo(int id, Process ps, string alias, ProcessManager.ProcessExitedHandler processExited) {
            this.id = id;
            this.alias = alias;
            process = ps;
            process.Exited += (sender, _) => {
                if (sender is not Process) return;
                isRunning = false;
                processExited?.Invoke(this);
                Logger.Info($"Process {alias} exited.");
            };
        }
    }

}