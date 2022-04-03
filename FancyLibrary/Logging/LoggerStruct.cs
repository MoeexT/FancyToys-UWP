﻿using System.Diagnostics;


namespace FancyLibrary.Logging {

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

    public enum StdType {
        Input = 0,
        Output = 1,
        Error = 2
    }

    public struct StdStruct {
        public StdType Level;
        public int Sender;
        public byte[] Content;
    }

    public struct DialogStruct {
        public string Title;
        public string Content;
    }

}