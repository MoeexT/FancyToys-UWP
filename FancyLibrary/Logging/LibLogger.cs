using System;
using System.Diagnostics;


namespace FancyLibrary.Logging {

    public enum DebuggerType {
        Debug,
        Console,
    }
    
    public static class Debugger {
        public static DebuggerType Type = DebuggerType.Debug;
        public static void Println(string msg) {
            if (Type == DebuggerType.Debug)
                Debug.WriteLine(msg);
            else
                Console.WriteLine(msg);
        }
        public static void Println(bool msg) {
            if (Type == DebuggerType.Debug)
                Debug.WriteLine(msg);
            else
                Console.WriteLine(msg);
        }
    }

}