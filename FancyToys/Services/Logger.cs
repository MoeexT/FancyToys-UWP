using System.Diagnostics;
using System.Reflection;

using FancyLibrary;
using FancyLibrary.Logging;
using FancyLibrary.Setting;
using FancyLibrary.Utils;


namespace FancyToys.Services {

    public static class Logger {

        private const int _port = Ports.Setting;
        private static LogLevel _level;

        public static LogLevel Level {
            get => _level;
            set {
                _level = value;
                App.Server.Send(
                    _port,
                    Converter.GetBytes(
                        new SettingStruct {
                            Type = SettingType.LogLevel,
                            LogLevel = ((int)StdLogger.Level << 3) + (int)value,
                        }
                    )
                );
            }
        }

        public static void Trace(string msg, int depth = 1) => Show(msg, LogLevel.Trace, depth + 1);

        public static void Debug(string msg, int depth = 1) => Show(msg, LogLevel.Debug, depth + 1);

        public static void Info(string msg, int depth = 1) => Show(msg, LogLevel.Info, depth + 1);

        public static void Warn(string msg, int depth = 1) => Show(msg, LogLevel.Warn, depth + 1);

        public static void Error(string msg, int depth = 1) => Show(msg, LogLevel.Error, depth + 1);

        public static void Fatal(string msg, int depth = 1) => Show(msg, LogLevel.Fatal, depth + 1);

        private static void Show(string s, LogLevel level, int depth) {
            if (level > Level) {
                string msg = $"[{CallerName(depth)}] {s}";
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        private static string CallerName(int depth) {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method?.ReflectedType?.Name}.{method?.Name}";
        }
    }

}
