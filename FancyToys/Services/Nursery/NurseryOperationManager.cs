using System;

using Windows.ApplicationModel.Core;
using Windows.System;

using FancyLibrary.Nursery;

using FancyToys.Views;


namespace FancyToys.Services.Nursery {

    public class NurseryOperationManager {
        private NurseryView _nurseryView { get; set; }

        public NurseryOperationManager(NurseryView view) {
            _nurseryView = view;
            MainPage.Poster.OnNurseryOperationStructReceived += OperationStructReceived;
        }

        public static void Add(string pathName) {
            MainPage.Poster.Send(new NurseryOperationStruct {
                Type = NurseryOperationType.Add,
                IsRequest = true,
                Content = pathName,
            });
        }

        public static void Remove(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Remove,
                IsRequest = true,
                Id = pid,
            });
        }

        public static void Start(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Start,
                IsRequest = true,
                Id = pid,
            });
        }

        public static void Stop(int pid) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Stop,
                IsRequest = true,
                Id = pid,
            });
        }

        public static void AttachArgs(int pid, string args) {
            MainPage.Poster.Send(new NurseryOperationStruct() {
                Type = NurseryOperationType.Args,
                IsRequest = true,
                Id = pid,
                Content = args,
            });
        }

        private void OperationStructReceived(NurseryOperationStruct nos) {
            NurseryInfo ni;
            switch (nos.Type) {
                case NurseryOperationType.Add:
                    if (nos.Code == NurseryOperationResult.Success) {
                        _nurseryView.Add(nos.Id, nos.Content);
                    }
                    break;
                case NurseryOperationType.Args:
                    break;
                case NurseryOperationType.Start:
                    if (nos.Code == NurseryOperationResult.Success 
                        && _nurseryView.NurseryInfoMap.TryGetValue(nos.Id, out ni)) {
                        ni.ServerStart = nos.IsRequest;
                        _nurseryView.ToggleSwitch(nos.Id, true);
                    }
                    break;
                case NurseryOperationType.Stop:
                    if (_nurseryView.NurseryInfoMap.TryGetValue(nos.Id, out ni)) {
                        ni.ServerStop = nos.IsRequest;
                        _nurseryView.ToggleSwitch(nos.Id, false);
                    }
                    break;
                case NurseryOperationType.Restart:
                    break;
                case NurseryOperationType.Remove:
                    _nurseryView.Remove(nos.Id);
                    break;
                case NurseryOperationType.AutoRestart:
                    if (nos.Code == NurseryOperationResult.Success) {
                        // TODO
                    }
                    break;
                default:
                    Logger.Warn("Unknown Nursery Operation Type");
                    break;
            }
        }

        delegate void UpdateUI(NurseryOperationStruct nos);

        private void Dispatch(UpdateUI ss) { }
    }

}
