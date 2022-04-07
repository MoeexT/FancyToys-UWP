using System;
using System.Diagnostics;
using System.Reflection;

using FancyLibrary;
using FancyLibrary.Logging;


namespace FancyServer.Logging {

    public static class Logger {
        public static Messenger Messenger { get; set; }
        public static LogLevel Level { get; set; } = LogLevel.Trace;
        

        public static void Trace(string msg, int depth = 1) => Send(LogLevel.Trace, depth + 1, msg);

        public static void Debug(string msg, int depth = 1) => Send(LogLevel.Debug, depth + 1, msg);

        public static void Info(string msg, int depth = 1) => Send(LogLevel.Info, depth + 1, msg);

        public static void Warn(string msg, int depth = 1) => Send(LogLevel.Warn, depth + 1, msg);

        public static void Error(string msg, int depth = 1) => Send(LogLevel.Error, depth + 1, msg);

        public static void Fatal(string msg, int depth = 1) => Send(LogLevel.Fatal, depth + 1, msg);

        private static void Send(LogLevel type, int depth, string content) {
            _ = Messenger ?? throw new ArgumentNullException(nameof(Messenger));
            if (type >= Level) {
                Messenger.Send(new LogStruct {
                    Level = type,
                    Source = $"[FancyServer.{CallerName(depth + 1)}]",
                    Content = content,
                });
            }
        }

        private static string CallerName(int depth) {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method?.ReflectedType?.Name}.{method?.Name}";
        }
    }

}