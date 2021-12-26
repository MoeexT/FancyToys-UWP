using FancyLibrary.Utils;


namespace FancyLibrary.Logger {

    public static class LogManager {
        public delegate void OnMessageReadyHandler(object pdu);
        
        /// <summary>
        /// Process log
        /// </summary>
        /// <sender>LogManager</sender>
        /// <subscriber>MessageManager</subscriber>
        public static event OnMessageReadyHandler OnMessageReady;

        static LogManager() {
            LogClerk.OnLogReady += Send;
            StdClerk.OnStdReady += Send;
            DialogClerk.OnDialogReady += Send;
        }

        public static void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out LoggerStruct ls);
            if (!success) return;

            switch (ls.Type) {
                case LoggerType.Log:
                    LogClerk.Deal(ls.Content);
                    break;
                case LoggerType.Std:
                    StdClerk.Deal(ls.Content);
                    break;
                case LoggerType.Dialog:
                    DialogClerk.Deal(ls.Content);
                    break;
                default:
                    LogClerk.Warn($"Invalid log type: {ls.Type}");
                    break;
            }
        }

        private static void Send(object sdu) {
            LoggerStruct? pdu = sdu switch {
                LogStruct ls => PDU(LoggerType.Log, Converter.GetBytes(ls)),
                StdStruct ss => PDU(LoggerType.Std, Converter.GetBytes(ss)),
                DialogStruct ds => PDU(LoggerType.Dialog, Converter.GetBytes(ds)),
                _ => null
            };

            if (pdu != null) {
                OnMessageReady?.Invoke(pdu);
            } else {
                LogClerk.Warn($"Invalid sdu type: {sdu}");
            }
        }

        private static LoggerStruct PDU(LoggerType type, byte[] content) {
            return new LoggerStruct {
                Type = type,
                Content = content
            };
        }
    }

}