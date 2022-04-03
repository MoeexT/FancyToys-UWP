using System;
using System.Windows.Forms;

using FancyLibrary;
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

            Messenger messenger = new Messenger(626, 624);
            
            // logger
            Dialogger.Messenger = messenger;
            Logger.Messenger = messenger;
            StdLogger.Messenger = messenger;
            // action
            ActionManager actionManager = new ActionManager(messenger);
            // setting
            SettingManager settingManager = new SettingManager(messenger);
            // nursery
            ProcessManager processManager = new ProcessManager();
            NurseryInformationManager nurseryInformation = new NurseryInformationManager(messenger);
            NurseryOperationManager nurseryOperation = new NurseryOperationManager(messenger, processManager);
            NurseryConfigManager nurseryConfig = new NurseryConfigManager(messenger, processManager, nurseryInformation);

            // fetch processes' information and send to frontend
            nurseryInformation.run(processManager);

            messenger.OnMessengerReady += () => {
                Logger.Info("client connected");
            };
            messenger.OnMessengerSleep += () => {
                Console.WriteLine("client disconnected");
            };
            
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
