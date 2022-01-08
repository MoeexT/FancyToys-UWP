using System;
using System.Diagnostics;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Utils;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class OperationManager {

        private const int Port = Ports.NurseryOperation;
        private readonly Bridge BridgeServer;
        private readonly ProcessManager ProcessManager;

        public OperationManager(Bridge bridge, ProcessManager manager) {
            BridgeServer = bridge ?? throw new ArgumentNullException(nameof(bridge));
            ProcessManager = manager;
            ProcessManager.OnProcessExited += ProcessExited;
            BridgeServer.OnMessageReceived += Deal;
        }

        private void Deal(int port, byte[] bytes) {
            if (port is not Port) return;
            bool success = Converter.FromBytes(bytes, out OperationStruct os);
            if (!success) return;
            
            ProcessInfo pi;
            switch (os.Type) {
                case OperationType.Add:
                    pi = ProcessManager.Add(Consts.Encoding.GetString(os.Content));
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                case OperationType.Start:
                    pi = ProcessManager.Launch(os.Id);
                    Send(new OperationStruct {
                        Type = os.Type,
                        Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                        Id = os.Id,
                        Content = Consts.Encoding.GetBytes(pi is null ? "" : pi.Alias),
                    });
                    break;
                case OperationType.Args:
                    pi = ProcessManager.PatchArgs(os.Id, Consts.Encoding.GetString(os.Content));
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                case OperationType.Stop:
                    pi = ProcessManager.Stop(os.Id);
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                case OperationType.Restart:
                    if ((pi = ProcessManager.Stop(os.Id)) is not null) pi = ProcessManager.Launch(os.Id);
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                case OperationType.Remove:
                    pi = ProcessManager.Remove(os.Id);
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                case OperationType.AutoRestart:
                    pi = ProcessManager.SetAutoRestart(os.Id, true);
                    Send(Default(os.Id, os.Type, pi is null));
                    break;
                default:
                    Logger.Warn($"No such OperationType: {os.Type}");
                    break;
            }
        }

        private void ProcessExited(ProcessInfo info) {
            if (info is null) return;
            Send(new OperationStruct {
                Type = OperationType.Stop,
                Code = OperationResult.Void,
                Id = info.Id,
            });
        }

        private OperationStruct Default(int id, OperationType type, bool success) {
            return new OperationStruct {
                Type = type,
                Code = success ? OperationResult.Success : OperationResult.Failed,
                Id = id,
            };
        }
        
        private void Send(OperationStruct os) { BridgeServer.Send(Port, Converter.GetBytes(os)); }
    }

}