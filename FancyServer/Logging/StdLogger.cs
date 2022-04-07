using System;

using FancyLibrary;
using FancyLibrary.Logging;


namespace FancyServer.Logging {

    public static class StdLogger {
        public static Messenger Messenger { get; set; }
        public static StdType Level { get; set; } = StdType.Output;

        public static void StdOutput(int pid, string message) {
            _ = Messenger ?? throw new ArgumentNullException(nameof(Messenger));

            if (Level == StdType.Output) {
                Messenger.Send(new StdStruct {
                    Level = StdType.Output,
                    Sender = pid,
                    Content = message,
                });
            }
        }

        public static void StdError(int sender, string message) {
            _ = Messenger ?? throw new ArgumentNullException(nameof(Messenger));
            Messenger.Send(new StdStruct {
                Level = StdType.Error,
                Sender = sender,
                Content = message,
            });
        }
    }

}