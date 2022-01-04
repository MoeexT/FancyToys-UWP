using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Logging;

using FancyServer.Action;
using FancyServer.Logging;


namespace FancyServer {

    public static class ServerManager {
        public static ActionManager actionManager;

        public static void InitPipe() {
            UdpBridgeClient server = new(626, 624) {
                ReplyHeartbeat = true,
                SendHeartbeat = false,
            };

            // init logger
            Dialogger.Server = server;
            Logger.Server = server;
            StdLogger.Server = server;


            actionManager = new ActionManager(server);
        }
    }

}