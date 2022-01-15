using FancyLibrary;
using FancyLibrary.Logging;
using FancyLibrary.Setting;
using FancyLibrary.Utils;

using FancyToys.Consts;


namespace FancyToys.Services {

    public static class StdLogger {

        private const int _port = Ports.Setting;
        private static StdType _type;
        public static StdType Level {
            get => _type;
            set {
                _type = value;
                App.Server.Send(_port, Converter.GetBytes(new SettingStruct {
                    Type = SettingType.LogLevel,
                    LogLevel = ((int)value << 3) + (int)Logger.Level,
                }));
            }
        }
        
        public static void StdOutput(string msg) {
            
        }
        
        public static void StdError(string msg) {
            
        }
    }

}