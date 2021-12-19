using System;

using FancyLibrary.Logger;
using FancyLibrary.Message;
using FancyLibrary.Utils;


namespace FancyLibrary.Action {

    public class ActionManager: IManager {
        
        public delegate void ExitAppHandler();
        public delegate void ShowWindowHandler();


        /// <summary>
        /// Exit App
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event ExitAppHandler ExitAppEvent;
        /// <summary>
        /// Show UWP main window
        /// </summary>
        /// <sender>FrontEnd, BackEnd</sender>
        /// <subscriber>FrontEnd, BackEnd</subscriber>
        public event ShowWindowHandler ShowWindowEvent;
        /// <summary>
        /// Send action message to MessageManager
        /// </summary>
        /// <sender>ActionManager</sender>
        /// <subscriber>MessageManager</subscriber>
        public event IManager.OnMessageReadyHandler OnMessageReady;

        public void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out ActionStruct ac);
            if (!success) return;
            if (ac.Exit) ExitAppEvent?.Invoke();
            if (ac.Show) ShowWindowEvent?.Invoke();
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
        public void Send(bool show, bool exit) {
            OnMessageReady?.Invoke(
                new ActionStruct {
                    Show = show && !exit,
                    Exit = exit
                }
            );
        }
        

        public void Send(object sdu) {
            LogClerk.Warn("NotImplementedException do not use this method for now.");
        }
    }

}