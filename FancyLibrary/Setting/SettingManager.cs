using FancyLibrary.Logger;
using FancyLibrary.Message;
using FancyLibrary.Utils;


namespace FancyLibrary.Setting {

    public class SettingManager: IManager {

        public delegate void OnSettingReceivedHandler(SettingStruct ss);

        /// <summary>
        /// FancyToys' settings
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event OnSettingReceivedHandler OnSettingReceived;
        /// <summary>
        /// Send settings to MessageManager
        /// </summary>
        /// <sender>SettingManager</sender>
        /// <subscriber>MessageManager</subscriber>
        internal event IManager.OnMessageReadyHandler OnMessageReady;

        public void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out SettingStruct ss);

            if (success) {
                OnSettingReceived?.Invoke(ss);
            } else {
                LogClerk.Warn($"Parse struct failed: {bytes}");
            }
        }

        public void Send(object sdu) {
            OnMessageReady?.Invoke(sdu);
        }
    }

}