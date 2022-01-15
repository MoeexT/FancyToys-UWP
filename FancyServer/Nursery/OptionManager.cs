using System;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Utils;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class NurseryConfigManager {
        private const int Port = Ports.NurseryConfig;
        private readonly Bridge BridgeServer;
        private readonly ProcessManager ProcessManager;
        private readonly NurseryInformationManager InformationManager;

        public NurseryConfigManager(Bridge server, ProcessManager processManager, NurseryInformationManager informationManager) {
            BridgeServer = server ?? throw new ArgumentNullException(nameof(server));
            ProcessManager = processManager;
            InformationManager = informationManager;

            BridgeServer.OnMessageReceived += Deal;
        }

        private void Deal(int port, byte[] bytes) {
            if (!(port is Port)) return;
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