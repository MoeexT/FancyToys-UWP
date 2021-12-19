using FancyLibrary.Utils;


namespace FancyLibrary.Logger {

    public static class StdClerk {
        public delegate void OnStdReceivedHandler(StdStruct ss);
        internal delegate void OnStdReadyHandler(object stdStruct);

        /// <summary>
        /// Standard output, error should be processed.
        /// </summary>
        /// <sender>BackEnd</sender>
        /// <subscriber>FrontEnd</subscriber>
        public static event OnStdReceivedHandler OnStdReceived;
        internal static event OnStdReadyHandler OnStdReady;
        
        public static StdType StdLevel { get; set; } = StdType.Error;

        public static void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out StdStruct ss);
            if (!success) return;
            OnStdReceived?.Invoke(ss);
        }
        
        public static void StdOutput(string sender, string message) {
            OnStdReady?.Invoke(
                new StdStruct {
                    Type = StdType.Output,
                    Sender = sender,
                    Content = GlobalSettings.Encoding.GetBytes(message),
                }
            );
        }

        public static void StdError(string sender, string message) {
            OnStdReady?.Invoke(
                new StdStruct {
                    Type = StdType.Error,
                    Sender = sender,
                    Content = GlobalSettings.Encoding.GetBytes(message),
                }
            );
        }
    }

}