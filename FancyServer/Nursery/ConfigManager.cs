using System;

using FancyLibrary;
using FancyLibrary.Nursery;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class NurseryConfigManager {
        private readonly Messenger _messenger;
        private readonly ProcessManager ProcessManager;
        private readonly NurseryInformationManager InformationManager;

        public NurseryConfigManager(Messenger messenger, ProcessManager processManager, NurseryInformationManager informationManager) {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            ProcessManager = processManager;
            InformationManager = informationManager;

            _messenger.OnNurseryConfigStructReceived += Deal;
        }

        private void Deal(NurseryConfigStruct ncs) {
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