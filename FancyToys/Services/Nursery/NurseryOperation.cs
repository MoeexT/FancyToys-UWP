using System;

using Windows.ApplicationModel.Core;
using Windows.System;

using FancyLibrary.Nursery;

using FancyToys.Views;


namespace FancyToys.Services.Nursery {

    public class NurseryOperation {
        public NurseryOperation() { MainPage.Poster.OnNurseryOperationStructReceived += OperationStructReceived; }

        public static void Add(string pathName) {
            MainPage.Poster.Send(new NurseryOperationStruct {
                Type = NurseryOperationType.Add,
                Content = pathName,
            });
        }

        public static void Remove(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Remove,
                Id = pid,
            });
        }

        public static void Start(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Start,
                Id = pid,
            });
        }
        
        public static void Stop(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Stop,
                Id = pid,
            });
        }
        
        public static void AttachArgs(int pid, string args) {
            
        }

        private void OperationStructReceived(NurseryOperationStruct nos) {
            if (nos.Code == NurseryOperationResult.Failed) {
                Logger.Error($"Nursery Operation failed: [${nos.Type}] ${nos.Content}");
                return;
            }
            switch (nos.Type) {
                case NurseryOperationType.Add:
                    NurseryView.CurrentInstance.Add(nos.Id, nos.Content);
                    break;
                case NurseryOperationType.Args:
                    
                    break;
                case NurseryOperationType.Start:
                    
                    break;
                case NurseryOperationType.Stop:
                    
                    break;
                case NurseryOperationType.Restart:
                    
                    break;
                case NurseryOperationType.Remove:
                    NurseryView.CurrentInstance.Remove(nos.Id);
                    break;
                case NurseryOperationType.AutoRestart:
                    
                    break;
                default:
                    Logger.Warn("Unknown Nursery Operation Type");
                    break;
            }
        }
        
        delegate void UpdateUI(NurseryOperationStruct nos);

        private void Dispatch(UpdateUI ss) {
            
        }
    }

}
