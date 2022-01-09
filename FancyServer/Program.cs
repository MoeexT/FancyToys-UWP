using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

using FancyLibrary.Bridges;

using FancyServer.Action;
using FancyServer.Logging;
using FancyServer.Nursery;
using FancyServer.Setting;


namespace FancyServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            UdpBridgeClient bridge = new(626, 624) {
                ReplyHeartbeat = true,
                SendHeartbeat = false,
            };
            
            // logger
            Dialogger.Server = bridge;
            Logger.Server = bridge;
            StdLogger.Server = bridge;
            // action
            ActionManager actionManager = new(bridge);
            // setting
            SettingManager configManager = new(bridge);
            // nursery
            ProcessManager processManager = new();
            NurseryInformationManager nurseryInformation = new(bridge);
            NurseryOperationManager nurseryOperation = new(bridge, processManager);
            NurseryConfigManager nurseryConfig = new(bridge, processManager, nurseryInformation);

            // fetch processes' information and send to frontend
            nurseryInformation.run(processManager);
            
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            
            // processManager: add/remove/start/stop process
            NoForm noForm = new(actionManager, processManager);
            Application.Run(noForm);
        }

        /// <summary>
        /// this method solved 
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

    }
}
