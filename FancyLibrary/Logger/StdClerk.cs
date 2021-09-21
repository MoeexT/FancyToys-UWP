namespace FancyLibrary.Logger {

    public enum StdType {
        //Input = 0,
        Output = 1,
        Error = 2
    }

    public struct StdStruct {
        public StdType Type;
        public string Process;
        public string Content;
    }

    public class StdClerk {
    }

}