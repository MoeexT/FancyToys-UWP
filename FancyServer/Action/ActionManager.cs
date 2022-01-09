using System;
using System.Windows.Forms;

using FancyLibrary;
using FancyLibrary.Action;
using FancyLibrary.Bridges;
using FancyLibrary.Utils;


namespace FancyServer.Action {

    public class ActionManager {

        public delegate void ExitAppHandler();

        public delegate void ShowWindowHandler();

        /// <summary>
        /// Exit App
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event ExitAppHandler OnApplicationExited;

        /// <summary>
        /// Show UWP main window
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event ShowWindowHandler OnFrontEndShown;

        private const int Port = Ports.Action;
        private readonly Bridge BridgeServer;

        public ActionManager(Bridge bridge) {
            BridgeServer = bridge ?? throw new ArgumentNullException(nameof(bridge));
            bridge.OnMessageReceived += Deal;
        }

        public void Show() {
            Send(true, false);
        }

        public void Hide() {
            Send(false, false);
        }

        public void Exit() {
            Send(false, true);
        }

        private void Deal(int port, byte[] bytes) {
            if (port != Port) return;
            bool success = Converter.FromBytes(bytes, out ActionStruct ac);
            if (!success) return;
            if (ac.Exit) OnApplicationExited?.Invoke();
            if (ac.Show) OnFrontEndShown?.Invoke();
        }

        /// <summary> 
        ///         input      |      output
        ///     exit    show   |   eApp    sWin
        ///     true    true   |   true    fals
        ///     true    fals   |   true    fals
        ///     fals    true   |   fals    true
        ///     fals    fals   |   fals    fals
        ///     ---------------|-------------------------
        ///     true    show   |   true    fals
        ///     fals    show   |   fals    show
        ///     ---------------|-------------------------
        ///     exit    show   |   exit    show&&!exit
        ///      
        /// </summary>
        /// <param name="show"></param>
        /// <param name="exit"></param>
        private void Send(bool show, bool exit) {
            if (exit) Application.Exit();
            BridgeServer.Send(
                Port, Converter.GetBytes(
                    new ActionStruct {
                        Show = show && !exit,
                        Exit = exit
                    }
                )
            );
        }
    }

}