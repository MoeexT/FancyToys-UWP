using System;

using FancyLibrary;
using FancyLibrary.Logging;
using FancyLibrary.Setting;

using FancyServer.Logging;


namespace FancyServer.Setting {

    public class SettingManager {
        private readonly Messenger _messenger;

        public SettingManager(Messenger messenger) {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _messenger.OnSettingStructReceived += ss => {
                switch (ss.Type) {
                    case SettingType.LogLevel:
                        Logger.Level = (LogLevel)(ss.LogLevel & 0b111);
                        StdLogger.Level = (StdType)((ss.LogLevel & 0b11000) >> 3);
                        break;
                    default:
                        break;
                }
            };
        }

        public void SetLogLevel(LogLevel level) {
            Logger.Level = level;
            _messenger.Send(new SettingStruct {
                Type = SettingType.LogLevel,
                LogLevel = ((int)StdLogger.Level << 3) + (int)Logger.Level,
            });
        }
        
        public void SetStdLevel(StdType level) {
            StdLogger.Level = level;
            _messenger.Send(new SettingStruct {
                Type = SettingType.LogLevel,
                LogLevel = ((int)StdLogger.Level << 3) + (int)Logger.Level,
            });
        }
    }

}