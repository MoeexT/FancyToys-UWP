using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

using FancyLibrary.Bridges;
using FancyLibrary.Logging;

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
        static void Main() {
            Debugger.Type = DebuggerType.Console;
            
            UdpBridgeClient bridge = new UdpBridgeClient(626, 624);
            
            // logger
            Dialogger.Server = bridge;
            Logger.Server = bridge;
            StdLogger.Server = bridge;
            // action
            ActionManager actionManager = new ActionManager(bridge);
            // setting
            SettingManager settingManager = new SettingManager(bridge);
            // nursery
            ProcessManager processManager = new ProcessManager();
            NurseryInformationManager nurseryInformation = new NurseryInformationManager(bridge);
            NurseryOperationManager nurseryOperation = new NurseryOperationManager(bridge, processManager);
            NurseryConfigManager nurseryConfig = new NurseryConfigManager(bridge, processManager, nurseryInformation);

            // fetch processes' information and send to frontend
            nurseryInformation.run(processManager);
            
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            
            // processManager: add/remove/start/stop process
            NoForm noForm = new NoForm(actionManager, processManager);
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
