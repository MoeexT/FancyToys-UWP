using System.Diagnostics;
using System.Reflection;

using FancyLibrary.Utils;

using Newtonsoft.Json;


namespace FancyLibrary.Logger {

    public static class LogClerk {
        public delegate void OnLogReceivedHandler(LogStruct ls);

        internal delegate void OnLogReadyHandler(object logStruct);

        /// <summary>
        /// Received a log from remote endpoint.
        /// </summary>
        /// <sender>BackEnd</sender>
        /// <subscriber>FrontEnd</subscriber>
        public static event OnLogReceivedHandler OnLogReceived;
        
        /// <summary>
        /// Notify message manager to process log.
        /// </summary>
        internal static event OnLogReadyHandler OnLogReady;

        public static LogLevel Level { get; set; } = LogLevel.Trace;

        public static void Trace(string msg, int depth = 1) { Send(LogLevel.Trace, depth + 1, msg); }

        public static void Debug(string msg, int depth = 1) { Send(LogLevel.Debug, depth + 1, msg); }

        public static void Info(string msg, int depth = 1) { Send(LogLevel.Info, depth + 1, msg); }

        public static void Warn(string msg, int depth = 1) { Send(LogLevel.Warn, depth + 1, msg); }

        public static void Error(string msg, int depth = 1) { Send(LogLevel.Error, depth + 1, msg); }

        public static void Fatal(string msg, int depth = 1) { Send(LogLevel.Fatal, depth + 1, msg); }

        public static void Deal(byte[] bytes) {
            bool success = Converter.FromBytes(bytes, out LogStruct ls);
            if (!success) return;
            OnLogReceived?.Invoke(ls);
        }

        private static void Send(LogLevel type, int depth, string content) {
            if (type >= Level) {
                OnLogReady?.Invoke(
                    new LogStruct {
                        Level = type,
                        Source = CallerName(depth + 1),
                        Content = GlobalSettings.Encoding.GetBytes(content),
                    }
                );
            }
        }

        private static string CallerName(int depth) {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method?.ReflectedType?.Name}.{method?.Name}";
        }
    }

}