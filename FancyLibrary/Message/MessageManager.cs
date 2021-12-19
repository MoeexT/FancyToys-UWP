using System.Diagnostics;

using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Logger;
using FancyLibrary.Nursery;
using FancyLibrary.Setting;
using FancyLibrary.Utils;



namespace FancyLibrary.Message {

    public class MessageManager {
        private readonly Bridge messenger;
        private readonly LogManager logManager;
        private readonly ActionManager actionManager;
        private readonly SettingManager settingManager;
        private readonly NurseryManager nurseryManager;

        public MessageManager(Bridge bs, ActionManager am, LogManager lm, SettingManager sm, NurseryManager nm) {
            messenger = bs;
            actionManager = am;
            logManager = lm;
            settingManager = sm;
            nurseryManager = nm;
            bs.MessageReceived += Deal;
            actionManager.OnMessageReady += Send;
            logManager.OnMessageReady += Send;
            settingManager.OnMessageReady += Send;
            nurseryManager.OnMessageReady += Send;
        }

        private void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out MessageStruct ms);
            if (!success) return;

            switch (ms.Type) {
                case MessageType.Action:
                    actionManager.Deal(ms.Content);
                    break;
                case MessageType.Setting:
                    settingManager.Deal(ms.Content);
                    break;
                case MessageType.Logging:
                    logManager.Deal(ms.Content);
                    break;
                case MessageType.Nursery:
                    nurseryManager.Deal(ms.Content);
                    break;
                default:
                    LogClerk.Error($"Invalid message type: {ms.Type}({ms.Content})");
                    break;
            }
        }

        private void Send(object sdu) {
            MessageStruct? pdu = sdu switch {
                ActionStruct ass => PDU(MessageType.Action, Converter.GetBytes(ass)),
                LoggerStruct ls => PDU(MessageType.Logging, Converter.GetBytes(ls)),
                NurseryStruct ns => PDU(MessageType.Nursery, Converter.GetBytes(ns)),
                SettingStruct ss => PDU(MessageType.Setting, Converter.GetBytes(ss)),
                _ => null
            };

            if (pdu != null && messenger != null) {
                messenger.Send(Converter.GetBytes(pdu));
            } else {
                LogClerk.Error($"Send failed: pdu?({pdu == null}), messenger?({messenger == null})");
            }
        }

        private static MessageStruct PDU(MessageType mt, byte[] sdu) {
            MessageStruct pdu = new() {
                Type = mt,
                Content = sdu
            };
            return pdu;
        }
    }

}