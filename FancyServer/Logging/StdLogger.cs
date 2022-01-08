using System;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Logging;
using FancyLibrary.Utils;


namespace FancyServer.Logging {

    public static class StdLogger {
        private const int Port = Ports.Stdio;
        public static Bridge Server { get; set; }
        public static StdType Level { get; set; } = StdType.Error;

        public static void StdOutput(int pid, string message) {
            _ = Server ?? throw new ArgumentNullException(nameof(Server));

            if (Level == StdType.Output) {
                Server.Send(
                    Port, Converter.GetBytes(
                        new StdStruct {
                            Level = StdType.Output,
                            Sender = pid,
                            Content = Consts.Encoding.GetBytes(message),
                        }
                    )
                );
            }
        }

        public static void StdError(int sender, string message) {
            _ = Server ?? throw new ArgumentNullException(nameof(Server));
            Server.Send(
                Port, Converter.GetBytes(
                    new StdStruct {
                        Level = StdType.Error,
                        Sender = sender,
                        Content = Consts.Encoding.GetBytes(message),
                    }
                )
            );
        }
    }

}