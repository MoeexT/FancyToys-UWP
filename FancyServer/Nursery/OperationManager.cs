using System;
using System.IO;

using FancyLibrary;
using FancyLibrary.Nursery;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class NurseryOperationManager {

        private readonly Messenger _messenger;
        private readonly ProcessManager ProcessManager;

        public NurseryOperationManager(Messenger messenger, ProcessManager manager) {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            ProcessManager = manager;
            ProcessManager.OnProcessExited += ProcessExited;
            _messenger.OnNurseryOperationStructReceived += Deal;
        }

        private void Deal(NurseryOperationStruct os) {
            ProcessInfo pi;
            switch (os.Type) {
                case NurseryOperationType.Add:
                    pi = ProcessManager.Add(os.Content);
                    Logger.Debug($"Nursery add {pi}");
                    _messenger.Send(new NurseryOperationStruct() {
                        Id = os.Id,
                        Type = os.Type,
                        Content = pi is null ? null : Path.GetFileName(os.Content),
                    });
                    break;
                case NurseryOperationType.Start:
                    pi = ProcessManager.Launch(os.Id);
                    Logger.Debug($"Nursery start {pi}");
                    _messenger.Send(new NurseryOperationStruct {
                        Type = os.Type,
                        Code = pi is null ? NurseryOperationResult.Failed : NurseryOperationResult.Success,
                        Id = os.Id,
                        Content = pi?.Alias,
                    });
                    break;
                case NurseryOperationType.Args:
                    pi = ProcessManager.PatchArgs(os.Id, os.Content);
                    Logger.Debug($"Nursery args {pi}");
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Stop:
                    pi = ProcessManager.Stop(os.Id);
                    Logger.Debug($"Nursery stop {pi}");
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Restart:
                    if (!((pi = ProcessManager.Stop(os.Id)) is null)) pi = ProcessManager.Launch(os.Id);
                    Logger.Debug($"Nursery add {pi}");
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Remove:
                    pi = ProcessManager.Remove(os.Id);
                    Logger.Debug($"Nursery remove {pi}");
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.AutoRestart:
                    pi = ProcessManager.SetAutoRestart(os.Id, true);
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
            if (info is null) return;
            _messenger.Send(new NurseryOperationStruct {
                Type = NurseryOperationType.Stop,
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