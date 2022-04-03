using System;

using FancyLibrary;
using FancyLibrary.Logging;


namespace FancyServer.Logging {

    public static class Dialogger {
        public static Messenger Messenger { set; get; }
        
        public static void Dialog(string title, string message) {
            _ = Messenger ?? throw new ArgumentNullException(nameof(Messenger));
            Messenger.Send(new DialogStruct {
                Title = title,
                Content = message,
            });
        }
    }

}