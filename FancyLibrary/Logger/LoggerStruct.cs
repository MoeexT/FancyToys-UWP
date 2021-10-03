namespace FancyLibrary.Logger {

    public enum LoggerType {
        Log = 1,
        Std = 2,
        Dialog = 3
    }
    
    public struct LoggerStruct {
        public LoggerType Type;
        public string Content;
    }
    
    public enum LogLevel {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6
    }
    
    public struct LogStruct {
        public LogLevel Level; // log level
        public string Source; // sender of the log
        public string Content; // log itself
    }

}