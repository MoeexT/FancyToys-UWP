using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class ProcessManager {
        public delegate void ProcessInfoHandler(ProcessInfo ps);

        public event ProcessInfoHandler OnProcessAdd;
        public event ProcessInfoHandler OnProcessLaunched;
        public event ProcessInfoHandler OnProcessExited;
        public event ProcessInfoHandler OnProcessRemoved;

        /// <summary>
        /// pid and itself
        /// </summary>
        private Dictionary<int, ProcessInfo> Processes { get; }

        /// <summary>
        /// restart process automatically after a process exited 
        /// </summary>
        public bool RestartOnExit { get; set; } = false;

        public readonly object _launchLock;

        private int ID;

        public ProcessManager() {
            Processes = new Dictionary<int, ProcessInfo>();
            _launchLock = new object();
        }

        public ProcessInfo Add(string pathName) {
            if (!File.Exists(pathName)) {
                Logger.Error($"No such file: ${pathName}");
                return null;
            }

            Process child = InitProcess(pathName);
            Processes[ID] = new ProcessInfo(ID, child, Path.GetFileName(pathName), this, OnProcessExited);
            OnProcessAdd?.Invoke(Processes[ID]);
            Logger.Info($"Add {Path.GetFileName(pathName)} succeed.");

            return Processes[ID++];
        }

        public ProcessInfo PatchArgs(int pid, string args) {
            if (!Processes.TryGetValue(pid, out ProcessInfo pi)) return null;
            pi.Pcs.StartInfo.Arguments = args;
            return pi;
        }

        public ProcessInfo Launch(int pid) {
            Logger.Trace($"Launch {pid}");

            // no process via such `pid`
            if (!Processes.TryGetValue(pid, out ProcessInfo pi)) {
                Logger.Error($"Process {pid} doesn't exist.");
                return null;
            }

            // process is already running
            if (pi.IsRunning) {
                Logger.Warn($"Process {pi.Alias} is running");
                return null;
            }

            Process ps = pi.Pcs;

            // if this process had been redirected std-ioe, cancel
            if (pi.RedirectingIoe) {
                ps.CancelOutputRead();
                ps.CancelErrorRead();
                pi.RedirectingIoe = false;
            }

            lock (_launchLock) {
                pi.IsRunning = true;
                bool launchSucceed = ps.Start();

                if (!launchSucceed) { // launch failed
                    Dialogger.Dialog("Error", $"Process launch failed: {pi.Alias}");
                    return null;
                }
            }

            if (!ps.HasExited) {
                pi.CpuCounter = new PerformanceCounter("Process", "% Processor Time", ps.ProcessName);
                pi.MemCounter = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
            }

            if (!pi.RedirectingIoe) {
                ps.BeginOutputReadLine();
                ps.BeginErrorReadLine();
                pi.RedirectingIoe = true;
            }
            pi.Alias = ps.ProcessName;
            OnProcessLaunched?.Invoke(pi);
            Logger.Info($"Process {ps.ProcessName}[{ps.Id}] launched successfully.");

            return pi;
        }

        public ProcessInfo Stop(int pid) {
            if (!Processes.TryGetValue(pid, out ProcessInfo pi)) {
                Logger.Error($"No such process with PID: {pid}");

                return null;
            }

            Process ps = pi.Pcs;

            if (!ps.HasExited) {
                ps.Kill();
                Logger.Info("Process killed.");
            } else {
                Logger.Warn($"Process {pi.Alias}({pi.Id}) already exited.");
            }

            return pi;
        }

        public ProcessInfo Remove(int pid) {
            if (!Processes.TryGetValue(pid, out ProcessInfo pi)) {
                Logger.Error($"Process({pid}) doesn't exist");
                return null;
            }

            if (pi.IsRunning) { pi.Pcs.Kill(); }
            OnProcessRemoved?.Invoke(pi);
            Processes.Remove(pid);
            Logger.Trace($"Process removed: {pid}, {pi.Alias}");
            return pi;
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
            Logger.Trace(pathName);
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

        public List<ProcessInfo> GetAliveProcesses() {
            return Processes.Values.Where(pi => pi.IsRunning).ToList();
        }

        private void OnProcessDataLoaded(object sender) { }

        private void BeforeProcessDataLoad(object sender) { }

        private void Debug() {
            foreach (KeyValuePair<int, ProcessInfo> kv in Processes) {
                Console.WriteLine($"{kv.Key}, {kv.Value.Alias}, running:{kv.Value.IsRunning} {kv.Value.Pcs.StartInfo.FileName}");
            }
        }

    }

    public class ProcessInfo {
        public readonly int Id;
        public bool IsRunning;
        // public bool StopByServer;
        public bool AutoRestart;
        public bool RedirectingIoe;
        public string Alias; // process name, after process launched
        public readonly Process Pcs;
        public PerformanceCounter CpuCounter;
        public PerformanceCounter MemCounter;

        public ProcessInfo(int id, Process ps, string alias, ProcessManager pm, ProcessManager.ProcessInfoHandler processExited) {
            Id = id;
            Alias = alias;
            Pcs = ps;

            Pcs.Exited += (sender, _) => {
                Logger.Trace("Process exited." + alias);

                if (AutoRestart) {
                    Logger.Info(Pcs.Start()
                        ? $"Restart {Pcs.ProcessName}({id}) successfully."
                        : $"Restart {Pcs.ProcessName}({id}) failed.");
                } else {
                    lock (pm._launchLock) {
                        IsRunning = false;
                    }
                    processExited?.Invoke(this);
                    Logger.Info($"Process {alias}, running:{IsRunning} exited.");
                }
            };
            Logger.Debug("ProcessInfo created.");
        }

        public override string ToString() => $"<ProcessInfo>{{Id:{Id}, IsRunning:{IsRunning}, AutoRestart:{AutoRestart}, Alias:{Alias}}}";
    }

}
