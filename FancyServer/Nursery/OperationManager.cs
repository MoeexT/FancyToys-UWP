using System;
using System.IO;

using FancyLibrary;
using FancyLibrary.Nursery;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class NurseryOperationManager {

        private readonly Messenger _messenger;
        private readonly ProcessManager _processManager;
        private readonly NurseryInformationManager _InfoManager;

        public NurseryOperationManager(Messenger messenger, ProcessManager pm, NurseryInformationManager nim) {
            _messenger = messenger;
            _processManager = pm;
            _InfoManager = nim;
            _processManager.OnProcessExited += ProcessExited;
            _messenger.OnNurseryOperationStructReceived += Deal;
        }

        public void Start(int pid) {
            ProcessInfo pi = _processManager.Launch(pid);
            if (pi == null) return;

            _messenger.Send(new NurseryOperationStruct() {
                Id = pi.Id,
                IsRequest = true,
                Content = pi.Alias,
                Type = NurseryOperationType.Start,
                Code = NurseryOperationResult.Success,
            });
        }

        public void Stop(int pid) {
            ProcessInfo pi = _processManager.Stop(pid, true);
            if (pi == null) return;

            _messenger.Send(new NurseryOperationStruct() {
                Id = pi.Id,
                IsRequest = true,
                Type = NurseryOperationType.Stop,
                Code = NurseryOperationResult.Success,
            });
        }

        private void Deal(NurseryOperationStruct os) {
            Logger.Trace(os.ToString());
            ProcessInfo pi;

            switch (os.Type) {
                case NurseryOperationType.Add:
                    pi = _processManager.Add(os.Content);
                    Logger.Debug($"Nursery add {pi}");

                    _messenger.Send(new NurseryOperationStruct() {
                        Id = pi.Id,
                        Type = os.Type,
                        IsRequest = false,
                        Code = NurseryOperationResult.Success,
                        Content = pi is null ? null : os.Content,
                    });
                    break;
                case NurseryOperationType.Start:
                    pi = _processManager.Launch(os.Id);
                    Logger.Debug($"Nursery start {pi}");

                    _messenger.Send(new NurseryOperationStruct {
                        Type = os.Type,
                        IsRequest = false,
                        Code = pi is null ? NurseryOperationResult.Failed : NurseryOperationResult.Success,
                        Id = os.Id,
                        Content = pi?.Alias,
                    });
                    _InfoManager.Flush();
                    break;
                case NurseryOperationType.Args:
                    pi = _processManager.PatchArgs(os.Id, os.Content);
                    Logger.Debug($"Nursery args {pi}");

                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Stop:
                    pi = _processManager.Stop(os.Id);
                    Logger.Debug($"Nursery stop {pi}");

                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Restart:
                    if ((pi = _processManager.Stop(os.Id)) is not null)
                        pi = _processManager.Launch(os.Id);
                    Logger.Debug($"Nursery add {pi}");

                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Remove:
                    pi = _processManager.Remove(os.Id);
                    Logger.Debug($"Nursery remove {pi}");

                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.AutoRestart:
                    pi = _processManager.SetAutoRestart(os.Id, true);
                    Logger.Debug($"Nursery auto-restart {pi}");

                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                default:
                    Logger.Warn($"No such OperationType: {os.Type}");
                    break;
            }
        }

        private void ProcessExited(ProcessInfo info) {
            Logger.Warn($"Process exited with unknown reason: {info.ToString()}");

            _messenger.Send(new NurseryOperationStruct {
                Type = NurseryOperationType.Stop,
                IsRequest = info.StopByServer,
                Code = NurseryOperationResult.Void,
                Id = info.Id,
            });
        }

        private NurseryOperationStruct Default(int id, NurseryOperationType type, bool success) {
            return new NurseryOperationStruct {
                Type = type,
                Code = success ? NurseryOperationResult.Success : NurseryOperationResult.Failed,
                Id = id,
            };
        }
    }

}
