// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// using FancyLibrary.Action;
// using FancyLibrary.Bridges;
// using FancyLibrary.Logging;
// using FancyLibrary.Nursery;
// using FancyLibrary.Setting;
// using FancyLibrary.Utils;
//
//
// namespace FancyLibrary {
//
//     public class Messenger {
//         public enum Ports {
//             Bridge = 0,
//             Log = 1, // normal log
//             Stdio = 2, // stdout stderr
//             Dialog = 3, // dialog
//             Action = 4,
//             Setting = 5,
//             NurseryConfig = 6,
//             NurseryInformation = 7,
//             NurseryOperation = 8,
//         }
//
//         public delegate void MessengerReadyEventHandler();
//         public delegate void MessengerSleepEventHandler();
//
//         public event MessengerReadyEventHandler OnMessengerReady;
//         public event MessengerSleepEventHandler OnMessengerSleep;
//
//         public delegate void ActionStructReceivedHandler(ActionStruct actionStruct);
//
//         public delegate void LogStructReceivedHandler(LogStruct logStruct);
//
//         public delegate void StdStructReceivedHandler(StdStruct stdioStruct);
//
//         public delegate void DialogStructReceivedHandler(DialogStruct dialogStruct);
//
//         public delegate void SettingStructReceivedHandler(SettingStruct settingStruct);
//
//         public delegate void NurseryConfigStructReceivedHandler(NurseryConfigStruct nurseryConfigStruct);
//
//         public delegate void NurseryOperationStructReceivedHandler(NurseryOperationStruct nurseryOperationStruct);
//
//         public delegate void NurseryInformationStructReceivedHandler(List<NurseryInformationStruct> nurseryInformationStructList);
//
//         public delegate IStruct RequestHandler<in T>(T sct) where T: IStruct;
//
//         public event ActionStructReceivedHandler OnActionStructReceived;
//         public event LogStructReceivedHandler OnLogStructReceived;
//         public event StdStructReceivedHandler OnStdStructReceived;
//         public event DialogStructReceivedHandler OnDialogStructReceived;
//         public event SettingStructReceivedHandler OnSettingStructReceived;
//         public event NurseryConfigStructReceivedHandler OnNurseryConfigStructReceived;
//         public event NurseryOperationStructReceivedHandler OnNurseryOperationStructReceived;
//         public event NurseryInformationStructReceivedHandler OnNurseryInformationStructReceived;
//
//         /// <summary>
//         /// records which port is used by which class
//         /// </summary>
//
//         // private Dictionary<string, ushort> _portMap;
//         private UdpBridgeClient _server;
//
//         private readonly Dictionary<Type, object> _requestHandlers;
//
//         private UdpBridgeClient Server {
//             get => _server ?? throw new NullReferenceException(nameof(Server));
//             set => _server = value ?? throw new ArgumentNullException(nameof(value));
//         }
//
//         public Messenger(int localPort, int remotePort) {
//             _server = new UdpBridgeClient(localPort, remotePort) {
//                 SendHeartbeat = true,
//             };
//             _server.OnMessageReceived += OnMessageReceived;
//
//             _server.OnClientOpened += () => {
//                 OnMessengerReady?.Invoke();
//             };
//
//             _server.OnClientClosed += () => {
//                 OnMessengerSleep?.Invoke();
//             };
//
//             _server.OnPacketReceived += HandleRequest;
//
//             _requestHandlers = new Dictionary<Type, object>();
//         }
//
//         public void Send(object o) {
//             Ports port = o switch {
//                 ActionStruct => Ports.Action,
//                 LogStruct => Ports.Log,
//                 StdStruct => Ports.Stdio,
//                 DialogStruct => Ports.Dialog,
//                 SettingStruct => Ports.Setting,
//                 NurseryConfigStruct => Ports.NurseryConfig,
//                 NurseryOperationStruct => Ports.NurseryOperation,
//                 List<NurseryInformationStruct> => Ports.NurseryInformation,
//                 _ => 0,
//             };
//
//             if (port != 0) {
//                 Server.Send((int)port, Converter.GetBytes(o));
//             }
//         }
//
//         public async Task<T?> Request<T>(T o) where T: IStruct {
//             (Ports port, Type _) = GetPort(o);
//             if (port == Ports.Bridge) return null;
//
//             DatagramStruct response = await Server.Request(o);
//             return Converter.FromBytes(response.Content, out T res) ? res : null;
//         }
//
//         [Obsolete]
//         public bool Response<T>(T o) where T: struct {
//             (Ports port, Type t) = GetPort(o);
//             if (port == Ports.Bridge) return false;
//             Server.Response((int)port, 0, Converter.GetBytes(o));
//             return true;
//         }
//
//         public void Register<T>(RequestHandler<T> handler) where T: IStruct {
//             _requestHandlers.Add(typeof(T), handler);
//         }
//
//         private void HandleRequest(DatagramStruct ds) {
//             (Ports port, Type type) = GetPort(ds);
//
//             var t = Activator.CreateInstance(type);
//
//             bool success = Converter.FromBytes(ds.Content, out object sct);
//         }
//
//         private void OnMessageReceived(int port, byte[] bytes) {
//             bool success;
//
//             switch (port) {
//                 case (int)Ports.Action:
//                     success = Converter.FromBytes(bytes, out ActionStruct ac);
//                     if (success) OnActionStructReceived?.Invoke(ac);
//                     break;
//                 case (int)Ports.Log:
//                     success = Converter.FromBytes(bytes, out LogStruct log);
//                     if (success) OnLogStructReceived?.Invoke(log);
//                     break;
//                 case (int)Ports.Stdio:
//                     success = Converter.FromBytes(bytes, out StdStruct stdio);
//                     if (success) OnStdStructReceived?.Invoke(stdio);
//                     break;
//                 case (int)Ports.Dialog:
//                     success = Converter.FromBytes(bytes, out DialogStruct dialog);
//                     if (success) OnDialogStructReceived?.Invoke(dialog);
//                     break;
//                 case (int)Ports.Setting:
//                     success = Converter.FromBytes(bytes, out SettingStruct setting);
//                     if (success) OnSettingStructReceived?.Invoke(setting);
//                     break;
//                 case (int)Ports.NurseryConfig:
//                     success = Converter.FromBytes(bytes, out NurseryConfigStruct nurseryConfig);
//                     if (success) OnNurseryConfigStructReceived?.Invoke(nurseryConfig);
//                     break;
//                 case (int)Ports.NurseryOperation:
//                     success = Converter.FromBytes(bytes, out NurseryOperationStruct nurseryOperation);
//                     if (success) OnNurseryOperationStructReceived?.Invoke(nurseryOperation);
//                     break;
//                 case (int)Ports.NurseryInformation:
//                     success = Converter.FromBytes(bytes, out List<NurseryInformationStruct> niList);
//                     if (success) OnNurseryInformationStructReceived?.Invoke(niList);
//                     break;
//             }
//         }
//
//         public static (Ports, Type) GetPort<T>(T o) {
//             return o switch {
//                 ActionStruct => (Ports.Action, typeof(ActionStruct)),
//                 LogStruct => (Ports.Log, typeof(LogStruct)),
//                 StdStruct => (Ports.Stdio, typeof(StdStruct)),
//                 DialogStruct => (Ports.Dialog, typeof(DialogStruct)),
//                 SettingStruct => (Ports.Setting, typeof(SettingStruct)),
//                 NurseryConfigStruct => (Ports.NurseryConfig, typeof(NurseryConfigStruct)),
//                 NurseryOperationStruct => (Ports.NurseryOperation, typeof(NurseryOperationStruct)),
//                 List<NurseryInformationStruct> => (Ports.NurseryInformation, typeof(List<NurseryInformationStruct>)),
//                 _ => (Ports.Bridge, typeof(DatagramStruct)),
//             };
//         }
//     }
//
// }
