using System;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Logging;
using FancyLibrary.Utils;


namespace FancyServer.Logging {

    public static class Dialogger {
        private const int port = Ports.Dialog;
        public static Bridge Server { set; get; }
        
        public static void Dialog(string title, string message) {
            _ = Server ?? throw new ArgumentNullException(nameof(Server));
            Server.Send(
                port, Converter.GetBytes(
                    new DialogStruct {
                        Title = title,
                        Content = message,
                    }
                )
            );
        }
    }

}