using FancyLibrary.Logging;
using FancyLibrary.Setting;


namespace FancyToys.Services {

    public static class StdLogger {

        private static StdType _type;
        public static StdType Level {
            get => _type;
            set {
                _type = value;
                MainPage.Poster.Send(new SettingStruct {
                    Type = SettingType.LogLevel,
                    LogLevel = ((int)value << 3) + (int)Logger.Level,
                });
            }
        }
        
        public static void StdOutput(string msg) {
            
        }
        
        public static void StdError(string msg) {
            
        }
    }

}