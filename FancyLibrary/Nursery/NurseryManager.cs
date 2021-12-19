using System.Collections.Generic;

using FancyLibrary.Logger;
using FancyLibrary.Utils;


namespace FancyLibrary.Nursery {

    public class NurseryManager: IManager {
        public delegate void OnInformationReceivedHandler(InformationStruct fs);

        public delegate void OnOperationReceivedHandler(OperationStruct os);

        public delegate void OnSettingReceivedHandler(ConfigStruct ss);

        /// <summary>
        /// Information of processes need to be shown on front-end.
        /// </summary>
        /// <sender>BackEnd</sender>
        /// <subscriber>FrontEnd</subscriber>
        public event OnInformationReceivedHandler OnInformationReceived;

        /// <summary>
        /// Operations on processes
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event OnOperationReceivedHandler OnOperationReceived;

        /// <summary>
        /// Settings of Nursery
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event OnSettingReceivedHandler OnSettingReceived;

        /// <summary>
        /// Send Nursery's message.
        /// </summary>
        /// <sender>NurseryManager</sender>
        /// <subscriber>MessageManager</subscriber>
        public event IManager.OnMessageReadyHandler OnMessageReady;

        public void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out NurseryStruct ns);
            if (!success) return;

            switch (ns.Type) {
                case NurseryType.Information:
                    success = Converter.FromBytes(ns.Content, out InformationStruct fs);
                    if (success) OnInformationReceived?.Invoke(fs);
                    break;
                case NurseryType.Operation:
                    success = Converter.FromBytes(ns.Content, out OperationStruct os);
                    if (success) OnOperationReceived?.Invoke(os);
                    break;
                case NurseryType.Config:
                    success = Converter.FromBytes(ns.Content, out ConfigStruct ss);
                    if (success) OnSettingReceived?.Invoke(ss);
                    break;
                default:
                    LogClerk.Warn($"Invalid Nursery type: {ns.Type}({ns.Content})");
                    break;
            }
        }

        public void Send(object sdu) {
            NurseryStruct? pdu = sdu switch {
                ConfigStruct cs => PDU(NurseryType.Config, Converter.GetBytes(cs)),
                OperationStruct os => PDU(NurseryType.Operation, Converter.GetBytes(os)),
                InformationStruct fs => PDU(NurseryType.Information, Converter.GetBytes(fs)),
                Dictionary<int, InformationStruct> lis => PDU(NurseryType.Information, Converter.GetBytes(lis)),
                _ => null
            };

            if (pdu != null) {
                OnMessageReady?.Invoke(pdu);
            } else {
                LogClerk.Warn($"Invalid sdu type: {sdu}");
            }
        }

        private static NurseryStruct PDU(NurseryType type, byte[] pdu) {
            return new NurseryStruct {
                Type = type,
                Content = pdu,
            };
        }
    }

}