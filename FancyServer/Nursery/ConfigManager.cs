using FancyLibrary;


namespace FancyServer.Nursery {

    public class ConfigManager {
        private const int Port = Ports.NurseryConfig;
        private readonly ProcessManager ProcessManager;

        public ConfigManager(ProcessManager manager) {
            ProcessManager = manager;
        }

    }

}