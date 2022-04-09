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

            Messenger messenger = new(626, 624);
            
            bool connected = false;
            // logger
            Dialogger.Messenger = messenger;
            Logger.Messenger = messenger;
            StdLogger.Messenger = messenger;
            // action
            ActionManager actionManager = new(messenger);
            // setting
            SettingManager settingManager = new(messenger);
            // nursery
            ProcessManager processManager = new();
            NurseryInformationManager nurseryInformation = new(messenger, processManager);
            NurseryOperationManager nurseryOperation = new(messenger, processManager, nurseryInformation);
            NurseryConfigManager nurseryConfig = new(messenger, processManager, nurseryInformation);

            // fetch processes' information and send to frontend
            nurseryInformation.run();

            messenger.OnMessengerReady += () => {
                Logger.Info("client connected");
            };
            messenger.OnMessengerSleep += () => {
                if (!connected) return;
                Console.WriteLine("client disconnected");
                connected = false;
            };
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            // https://www.cnblogs.com/shy1766IT/p/7323521.html
            Control.CheckForIllegalCrossThreadCalls = false;
            // processManager: add/remove/start/stop process
            NoForm noForm = new NoForm(actionManager, processManager, nurseryOperation);
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
