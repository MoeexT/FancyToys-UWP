using System;

using FancyLibrary.Message;
using FancyLibrary.Utils;


namespace FancyLibrary.Action {

    public static class ActionManager {
        public delegate void ExitAppHandler();
        public delegate void ShowWindowHandler();


        public static event ExitAppHandler ExitAppEvent;
        public static event ShowWindowHandler ShowWindowEvent;


        public static void Deal(string msg) {
            bool success = JsonUtil.ParseStruct(msg, out ActionStruct ac);
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
        public static void Send(bool show, bool exit) {
            MessageManager.Send(
                new ActionStruct {
                    Show = show && !exit,
                    Exit = exit
                }
            );
        }
    }

}