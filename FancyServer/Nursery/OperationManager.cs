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
                    pi = ProcessManager.Add(os.PathName, os.Args);
                    Send(
                        new OperationStruct {
                            Type = OperationType.Add,
                            Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                            Id = pi?.id ?? -1,
                        }
                    );
                    break;
                case OperationType.Start:
                    pi = ProcessManager.Launch(os.Id);
                    Send(
                        new OperationStruct {
                            Type = OperationType.Start,
                            Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                            Id = os.Id,
                            ProcessName = pi?.alias,
                        }
                    );
                    break;
                case OperationType.Stop:
                    pi = ProcessManager.Stop(os.Id);
                    Send(
                        new OperationStruct {
                            Type = OperationType.Stop,
                            Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                            Id = os.Id,
                        }
                    );
                    break;
                case OperationType.Restart:
                    if ((pi = ProcessManager.Stop(os.Id)) is not null) pi = ProcessManager.Launch(os.Id);
                    Send(
                        new OperationStruct {
                            Type = OperationType.Restart,
                            Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                            Id = os.Id,
                        }
                    );
                    break;
                case OperationType.Remove:
                    pi = ProcessManager.Remove(os.Id);
                    Send(new OperationStruct {
                        Type = OperationType.Remove,
                        Code = pi is null ? OperationResult.Failed : OperationResult.Success,
                        Id = os.Id,
                    });
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
                Id = info.id,
            });
        }
        
        private void Send(OperationStruct os) { BridgeServer.Send(Port, Converter.GetBytes(os)); }
    }

}