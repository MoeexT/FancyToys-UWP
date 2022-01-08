using System;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Utils;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class OptionManager {
        private const int Port = Ports.NurseryConfig;
        private Bridge BridgeServer;
        private readonly ProcessManager ProcessManager;
        private readonly InformationManager InformationManager;

        public OptionManager(Bridge server, ProcessManager processManager, InformationManager informationManager) {
            BridgeServer = server ?? throw new ArgumentNullException(nameof(server));
            ProcessManager = processManager;
            InformationManager = informationManager;

            BridgeServer.OnMessageReceived += Deal;
        }

        private void Deal(int port, byte[] bytes) {
            if (port is not Port) return;
            bool success = Converter.FromBytes(bytes, out NurseryConfigStruct ncs);
            if (!success) return;

            switch (ncs.Type) {
                case NurseryConfigType.FlushTime:
                    InformationManager.UpdateSpan = ncs.FlushTime;
                    break;
                default:
                    Logger.Warn($"Invalid {nameof(NurseryConfigType)}: {ncs.Type}");
                    break;
            }
        }
    }

}