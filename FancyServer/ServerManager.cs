using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Logger;
using FancyLibrary.Message;
using FancyLibrary.Nursery;
using FancyLibrary.Setting;


namespace FancyServer {

    public static class ServerManager {
        public static ActionManager actionManager;
        public static SettingManager settingManager;
        public static NurseryManager nurseryManager;
        public static MessageManager messageManager;

        public static void InitPipe() {
            // init logger

            UdpBridgeClient server = new(626, 624) {
                ReplyHeartbeat = true,
                SendHeartbeat = false,
            }; // TODO 订阅事件，可能需要先实现nursery

            actionManager = new ActionManager();
            settingManager = new SettingManager();
            nurseryManager = new NurseryManager();
            messageManager = new MessageManager(server, actionManager, settingManager, nurseryManager);
        }
    }

}