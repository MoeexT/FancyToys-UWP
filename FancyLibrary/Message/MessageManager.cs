using System.Diagnostics;

using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Logger;
using FancyLibrary.Nursery;
using FancyLibrary.Setting;
using FancyLibrary.Utils;

using Newtonsoft.Json;


namespace FancyLibrary.Message {

    public static class MessageManager {
        private static Bridge messenger;

        public static void Init(Bridge bs) {
            messenger = bs;
            bs.MessageReceived += Deal;
        }

        private static void Deal(string message) {
            bool success = JsonUtil.ParseStruct(message, out MessageStruct ms);

            if (!success) return;

            switch (ms.Type) {
                case MessageType.Action:
                    ActionManager.Deal(ms.Content);
                    break;
                case MessageType.Setting:
                    SettingManager.Deal(ms.Content);
                    break;
                case MessageType.Logging:
                    LogClerk.Error("Log shouldn't be sent from front-end");
                    break;
                case MessageType.Nursery:
                    NurseryManager.Deal(ms.Content);
                    break;
                default:
                    LogClerk.Error("Invalid message type");
                    break;
            }
        }

        public static void Send(object sdu) {
            MessageStruct? pdu = sdu switch {
                ActionStruct ass => PDU(MessageType.Action, JsonConvert.SerializeObject(ass)),
                LoggerStruct ls => PDU(MessageType.Logging, JsonConvert.SerializeObject(ls)),
                NurseryStruct ns => PDU(MessageType.Nursery, JsonConvert.SerializeObject(ns)),
                SettingStruct ss => PDU(MessageType.Setting, JsonConvert.SerializeObject(ss)),
                _ => null
            };

            if (pdu != null && messenger != null) messenger.Send(JsonConvert.SerializeObject(pdu));
        }

        private static MessageStruct PDU(MessageType mt, string sdu) {
            MessageStruct pdu = new() {
                Type = mt,
                Content = sdu
            };
            return pdu;
        }
    }

}