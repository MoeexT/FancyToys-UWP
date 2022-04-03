using System;

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
                    pi = ProcessManager.Add(Consts.Encoding.GetString(os.Content));
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Start:
                    pi = ProcessManager.Launch(os.Id);
                    _messenger.Send(new NurseryOperationStruct {
                        Type = os.Type,
                        Code = pi is null ? NurseryOperationResult.Failed : NurseryOperationResult.Success,
                        Id = os.Id,
                        Content = Consts.Encoding.GetBytes(pi is null ? "" : pi.Alias),
                    });
                    break;
                case NurseryOperationType.Args:
                    pi = ProcessManager.PatchArgs(os.Id, Consts.Encoding.GetString(os.Content));
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Stop:
                    pi = ProcessManager.Stop(os.Id);
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Restart:
                    if (!((pi = ProcessManager.Stop(os.Id)) is null)) pi = ProcessManager.Launch(os.Id);
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.Remove:
                    pi = ProcessManager.Remove(os.Id);
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                case NurseryOperationType.AutoRestart:
                    pi = ProcessManager.SetAutoRestart(os.Id, true);
                    _messenger.Send(Default(os.Id, os.Type, pi is null));
                    break;
                default:
                    Logger.Warn($"No such OperationType: {os.Type}");
                    break;
            }
        }

        private void ProcessExited(ProcessInfo info) {
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