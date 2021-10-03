namespace FancyLibrary.Logger {

    public enum StdType {
        //Input = 0,
        Output = 1,
        Error = 2
    }


    public struct StdStruct {
        public StdType Type;
        public string Sender;
        public string Content;
    }


    public static class StdClerk {
        public static StdType StdLevel { get; set; } = StdType.Error;

        public static void StdOutput(string sender, string message) {
            LogManager.Send(
                new StdStruct {
                    Type = StdType.Output,
                    Sender = sender,
                    Content = message
                }
            );
        }

        public static void StdError(string sender, string message) {
            LogManager.Send(new StdStruct {
                Type = StdType.Error,
                Sender = sender,
                Content = message
            });
        }
    }

}