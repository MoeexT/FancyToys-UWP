using FancyLibrary.Message;

using Newtonsoft.Json;


namespace FancyLibrary.Logger {

    public enum LoggerType {
        Log = 1,
        Std = 2,
        Dialog = 3
    }

    public struct LoggerStruct {
        public LoggerType Type;
        public string Content;
    }

    public static class LogManager {
        public static void Send(object sdu) {
            LoggerStruct? pdu = sdu switch {
                LogStruct ls => PDU(LoggerType.Log, JsonConvert.SerializeObject(ls)),
                StdStruct ss => PDU(LoggerType.Std, JsonConvert.SerializeObject(ss)),
                DialogStruct ds => PDU(LoggerType.Dialog, JsonConvert.SerializeObject(ds)),
                _ => null
            };

            if (pdu != null) MessageManager.Send(pdu);
        }

        private static LoggerStruct PDU(LoggerType type, string content) {
            LoggerStruct pdu = new() {
                Type = type,
                Content = content
            };
            return pdu;
        }
    }

}