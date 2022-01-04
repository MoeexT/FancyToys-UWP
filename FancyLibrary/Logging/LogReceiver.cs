using System;

using FancyLibrary.Bridges;
using FancyLibrary.Utils;


namespace FancyLibrary.Logging {

    // receive log from backend
    public static class LogReceiver {
        private static Bridge Client { get; set; }

        public static void Init(Bridge client) {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Client.OnMessageReceived += (port, bytes) => {
                bool success;

                switch (port) {
                    case Ports.Logger:
                        success = Converter.FromBytes(bytes, out LoggerStruct ls);

                        if (success) {
                            // TODO
                        }
                        break;
                    case Ports.Dialog:
                        success = Converter.FromBytes(bytes, out DialogStruct ds);

                        if (success) {
                            // TODO
                        }
                        break;
                        break;
                    case Ports.Stdio:
                        success = Converter.FromBytes(bytes, out StdStruct ss);

                        if (success) {
                            // TODO
                        }
                        break;
                }
            };
        }
    }

}