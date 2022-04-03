using System;

using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Logging;
using FancyLibrary.Nursery;
using FancyLibrary.Setting;
using FancyLibrary.Utils;


namespace FancyLibrary {

    public class Messenger {
        private enum Ports {
            Bridge = 0,
            Log = 1,    // normal log
            Stdio = 2,  // stdout stderr
            Dialog = 3, // dialog
            Action = 4,
            Setting = 5,
            NurseryConfig = 6,
            NurseryInformation = 7,
            NurseryOperation = 8,
        }

        public delegate void MessengerReadyEventHandler();
        public delegate void MessengerSleepEventHandler();
        
        public event MessengerReadyEventHandler OnMessengerReady;
        public event MessengerSleepEventHandler OnMessengerSleep;

        public delegate void ActionStructReceivedHandler(ActionStruct actionStruct);

        public delegate void LogStructReceivedHandler(LogStruct logStruct);

        public delegate void StdStructReceivedHandler(StdStruct stdioStruct);

        public delegate void DialogStructReceivedHandler(DialogStruct dialogStruct);

        public delegate void SettingStructReceivedHandler(SettingStruct settingStruct);

        public delegate void NurseryConfigStructReceivedHandler(NurseryConfigStruct nurseryConfigStruct);

        public delegate void NurseryOperationStructReceivedHandler(NurseryOperationStruct nurseryOperationStruct);

        public delegate void NurseryInformationStructReceivedHandler(NurseryInformationStruct nurseryInformationStruct);

        public event ActionStructReceivedHandler OnActionStructReceived;
        public event LogStructReceivedHandler OnLogStructReceived;
        public event StdStructReceivedHandler OnStdStructReceived;
        public event DialogStructReceivedHandler OnDialogStructReceived;
        public event SettingStructReceivedHandler OnSettingStructReceived;
        public event NurseryConfigStructReceivedHandler OnNurseryConfigStructReceived;
        public event NurseryOperationStructReceivedHandler OnNurseryOperationStructReceived;
        public event NurseryInformationStructReceivedHandler OnNurseryInformationStructReceived;

        /// <summary>
        /// records which port is used by which class
        /// </summary>

        // private Dictionary<string, ushort> _portMap;
        private UdpBridgeClient _server;

        private UdpBridgeClient Server {
            get => _server ?? throw new NullReferenceException(nameof(Server));
            set => _server = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Messenger(int localPort, int remotePort) {
            _server = new UdpBridgeClient(localPort, remotePort) {
                SendHeartbeat = true,
            };
            _server.OnMessageReceived += OnMessageReceived;
            _server.OnClientOpened += () => {
                OnMessengerReady?.Invoke();
            };
            _server.OnClientClosed += () => {
                OnMessengerSleep?.Invoke();
            };
        }

        public void Send(object o) {
            Console.WriteLine(nameof(o));
            Ports port = o switch {
                ActionStruct => Ports.Action,
                LogStruct => Ports.Log,
                StdStruct => Ports.Stdio,
                DialogStruct => Ports.Dialog,
                SettingStruct => Ports.Setting,
                NurseryConfigStruct => Ports.NurseryConfig,
                NurseryOperationStruct => Ports.NurseryOperation,
                NurseryInformationStruct => Ports.NurseryInformation,
                _ => 0,
            };

            if (port != 0) {
                Server.Send((int)port, Converter.GetBytes(o));
            }
        }

        private void OnMessageReceived(int port, byte[] bytes) {
            bool success;

            switch (port) {
                case (int)Ports.Action:
                    success = Converter.FromBytes(bytes, out ActionStruct ac);
                    if (success) OnActionStructReceived?.Invoke(ac);
                    break;
                case (int)Ports.Log:
                    success = Converter.FromBytes(bytes, out LogStruct log);
                    if (success) OnLogStructReceived?.Invoke(log);
                    break;
                case (int)Ports.Stdio:
                    success = Converter.FromBytes(bytes, out StdStruct stdio);
                    if (success) OnStdStructReceived?.Invoke(stdio);
                    break;
                case (int)Ports.Dialog:
                    success = Converter.FromBytes(bytes, out DialogStruct dialog);
                    if (success) OnDialogStructReceived?.Invoke(dialog);
                    break;
                case (int)Ports.Setting:
                    success = Converter.FromBytes(bytes, out SettingStruct setting);
                    if (success) OnSettingStructReceived?.Invoke(setting);
                    break;
                case (int)Ports.NurseryConfig:
                    success = Converter.FromBytes(bytes, out NurseryConfigStruct nurseryConfig);
                    if (success) OnNurseryConfigStructReceived?.Invoke(nurseryConfig);
                    break;
                case (int)Ports.NurseryOperation:
                    success = Converter.FromBytes(bytes, out NurseryOperationStruct nurseryOperation);
                    if (success) OnNurseryOperationStructReceived?.Invoke(nurseryOperation);
                    break;
                case (int)Ports.NurseryInformation:
                    success = Converter.FromBytes(bytes, out NurseryInformationStruct nurseryInformation);
                    if (success) OnNurseryInformationStructReceived?.Invoke(nurseryInformation);
                    break;
            }
        }
    }

}
