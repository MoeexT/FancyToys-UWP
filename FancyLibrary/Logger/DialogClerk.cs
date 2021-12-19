using FancyLibrary.Utils;


namespace FancyLibrary.Logger {

    public static class DialogClerk {
        public delegate void OnDialogReceivedHandler(DialogStruct ds);
        internal delegate void OnDialogReadyHandler(object dialogStruct);

        /// <summary>
        /// Received a dialog struct
        /// </summary>
        /// <sender>BackEnd</sender>
        /// <subscriber>FrontEnd</subscriber>
        public static event OnDialogReceivedHandler OnDialogReceived;
        internal static event OnDialogReadyHandler OnDialogReady;

        public static void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out DialogStruct ds);
            if (!success) return;
            OnDialogReceived?.Invoke(ds);
        }

        public static void Dialog(string title, string message) {
            OnDialogReady?.Invoke(
                new DialogStruct {
                    Title = title,
                    Content = GlobalSettings.Encoding.GetBytes(message)
                }
            );
        }
    }

}