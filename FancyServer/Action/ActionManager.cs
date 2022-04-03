using System;
using System.Windows.Forms;

using FancyLibrary;
using FancyLibrary.Action;


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
        
        private readonly Messenger _messenger;

        public ActionManager(Messenger messenger) {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            messenger.OnActionStructReceived += Deal;
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

        private void Deal(ActionStruct ac) {
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
            _messenger.Send(new ActionStruct {
                Show = show && !exit,
                Exit = exit
            });
        }
    }

}