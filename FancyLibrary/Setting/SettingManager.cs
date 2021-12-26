using System;

using FancyLibrary.Logger;
using FancyLibrary.Message;
using FancyLibrary.Utils;


namespace FancyLibrary.Setting {

    public class SettingManager: IManager {

        public delegate void OnFormSettingReceivedHandler(FormSettingStruct fss);
        public delegate void OnLogSettingReceivedHandler(LogSettingStruct lss);
        public delegate void OnMessageSettingReceivedHandler(MessageSettingStruct mss);

        /// <summary>
        /// FancyToys' form settings
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event OnFormSettingReceivedHandler OnFormSettingReceived;
        public event OnLogSettingReceivedHandler OnLogSettingReceived;
        public event OnMessageSettingReceivedHandler OnMessageSettingReceived;
        
        /// <summary>
        /// Send settings to MessageManager
        /// </summary>
        /// <sender>SettingManager</sender>
        /// <subscriber>MessageManager</subscriber>
        internal event IManager.OnMessageReadyHandler OnMessageReady;

        public void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out SettingStruct ss);
            if (!success) return;

            switch (ss.Type) {
                case SettingType.Form:
                    success = Converter.FromBytes(ss.Content, out FormSettingStruct fss);
                    if (success) OnFormSettingReceived?.Invoke(fss);
                    break;
                case SettingType.Log:
                    success = Converter.FromBytes(ss.Content, out LogSettingStruct lss);
                    if (success) OnLogSettingReceived?.Invoke(lss);
                    break;
                case SettingType.Message:
                    success = Converter.FromBytes(ss.Content, out MessageSettingStruct mss);
                    if (success) OnMessageSettingReceived?.Invoke(mss);
                    break;
                default:
                    LogClerk.Warn($"Parse struct failed: {bytes}");
                    break;
            }
        }

        public void Send(object sdu) {
            SettingStruct? pdu = sdu switch {
                FormSettingStruct fss => PDU(SettingType.Form, Converter.GetBytes(fss)),
                LogSettingStruct lss => PDU(SettingType.Log, Converter.GetBytes(lss)),
                MessageSettingStruct mss => PDU(SettingType.Message, Converter.GetBytes(mss)),
                _ => null
            };

            if (pdu != null) {
                OnMessageReady?.Invoke(sdu);
            } else {
                LogClerk.Warn($"Invalid sdu type: {sdu}");
            }
        }

        private static SettingStruct PDU(SettingType type, byte[] pdu) {
            return new SettingStruct {
                Type = type,
                Content = pdu,
            };
        }
    }

}