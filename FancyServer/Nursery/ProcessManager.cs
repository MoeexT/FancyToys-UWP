using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using FancyLibrary.Logger;


namespace FancyServer.Nursery {

    public static class ProcessManager {
        public delegate void OnProcessAddHandler(ProcessInfo ps);

        public delegate void OnProcessLaunchedHandler(ProcessInfo ps);

        public delegate void OnProcessExitedHandler(ProcessInfo ps);

        public delegate void OnProcessRemovedHandler(ProcessInfo ps);

        public static event OnProcessAddHandler OnProcessAdd;
        public static event OnProcessLaunchedHandler OnProcessLaunched;
        public static event OnProcessExitedHandler OnProcessExited;
        public static event OnProcessRemovedHandler OnProcessRemoved;

        /// <summary>
        /// pid and itself
        /// </summary>
        private static Dictionary<int, ProcessInfo> Processes { get; set; }

        private static int tempId = 0;

        static ProcessManager() {
            Processes = new Dictionary<int, ProcessInfo>();
            Process p = new();
        }

        public static bool Add(string pathName) {
            if (!File.Exists(pathName)) {
                LogClerk.Error($"No such file: ${pathName}");
                return false;
            }
            OnProcessAdd?.Invoke(Processes[InitProcess(pathName)]);
            return true;
        }

        public static bool Launch(int pid, string args) {
            // no process via such `pid`
            if (!Processes.ContainsKey(pid)) {
                DialogClerk.Dialog("Error", $"Process {Processes[pid].alias} doesn't exist.");
                return false;
            }

            // process is already running
            if ((bool)Processes?[pid].isRunning) {
                LogClerk.Warn($"Process {Processes[pid].alias} is running");
                return true; // false
            }

            Process ps = Processes[pid].process;

            // add launch arguments
            if (!string.IsNullOrEmpty(args)) { ps.StartInfo.Arguments = args; }
            bool launchSucceed = ps.Start();

            if (!launchSucceed) {
                DialogClerk.Dialog("Error", $"Process launch failed: {Processes[pid].alias}");
                return false;
            }

            try {
                ps.BeginErrorReadLine();
                ps.BeginOutputReadLine();
            } catch (InvalidOperationException e) { LogClerk.Error($"[{ps.ProcessName}]{e.Message}"); }
            Processes[pid].isRunning = true;
            Processes[pid].alias = ps.ProcessName;

            // replace tmp PID with actual PID
            if (pid != ps.Id) {
                Processes[ps.Id] = Processes[pid];
                Processes.Remove(pid);
            }
            OnProcessLaunched?.Invoke(Processes[ps.Id]);
            LogClerk.Info($"Process {ps.ProcessName}[{ps.Id}] launched successfully.");
            return true;
        }

        public static void Stop(int pid) {
            if (!Processes.ContainsKey(pid)) {
                LogClerk.Error($"No such process with PID: {pid}");
                return;
            }
            Process ps = Processes[pid].process;

            try {
                ps.CancelErrorRead();
                ps.CancelOutputRead();
            } catch (InvalidOperationException e) { LogClerk.Warn($"{e.Data}"); }
            ps.Kill();
        }

        public static void Remove(int pid) {
            if (!Processes.ContainsKey(pid)) {
                LogClerk.Error($"Process({pid}) doesn't exist");
                return;
            }

            if (!Processes[pid].process.HasExited) { Processes[pid].process.Kill(); }
            OnProcessRemoved?.Invoke(Processes[pid]);
            Processes.Remove(pid);
        }

        /// <summary>
        /// add a original process and init its start info or something else
        /// </summary>
        /// <param name="pathName">file's full name with path</param>
        private static int InitProcess(string pathName) {
            Process child = new();
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.WorkingDirectory = Path.GetDirectoryName(pathName) ?? string.Empty;
            child.EnableRaisingEvents = true;


            child.OutputDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdClerk.StdOutput((s as Process)!.Id, e.Data);
            };
            child.ErrorDataReceived += (s, e) => {
                if (s != null && !string.IsNullOrEmpty(e.Data)) StdClerk.StdError((s as Process)!.Id, e.Data);
            };
            Processes[tempId++] = new ProcessInfo(child, Path.GetFileName(pathName));
            LogClerk.Info($"Add {Path.GetFileName(pathName)} succeed.");
            return tempId;
        }

        private static void OnProcessDataLoaded(object sender) { }

        private static void BeforeProcessDataLoad(object sender) { }

        public class ProcessInfo {
            public bool isRunning;
            public string alias; // process name, after process launched
            public readonly Process process;

            public ProcessInfo(Process ps, string alias) {
                this.alias = alias;
                process = ps;
                process.Exited += (sender, _) => {
                    if (sender is not Process p) return;
                    int pid = p.Id;
                    string pathName = p.StartInfo.FileName;
                    OperationClerk.OnProcessStopped(pathName, alias);
                    OnProcessExited?.Invoke(Processes[pid]);
                    LogClerk.Info($"Process {Processes[pid].alias} exited.");
                    isRunning = false;
                    OnProcessExited?.Invoke(Processes[pid]);
                };
            }
        }
    }

}