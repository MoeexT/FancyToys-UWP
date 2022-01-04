using System;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Logging;
using FancyLibrary.Setting;
using FancyLibrary.Utils;

using FancyServer.Logging;


namespace FancyServer.Setting {

    public class SettingManager {
        private const int Port = Ports.Setting;
        private readonly Bridge Server;

        public SettingManager(Bridge bridge) {
            Server = bridge ?? throw new ArgumentNullException(nameof(bridge));
            Server.OnMessageReceived += (port, bytes) => {
                if (port is not Port) return;
                bool success = Converter.FromBytes(bytes, out SettingStruct ss);
                if (!success) return;
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
            Send(new SettingStruct {
                Type = SettingType.LogLevel,
                LogLevel = ((int)StdLogger.Level << 3) + (int)Logger.Level,
            });
        }
        
        public void SetStdLevel(StdType level) {
            StdLogger.Level = level;
            Send(new SettingStruct {
                Type = SettingType.LogLevel,
                LogLevel = ((int)StdLogger.Level << 3) + (int)Logger.Level,
            });
        }

        private void Send(SettingStruct ss) {
            Server.Send(Port, Converter.GetBytes(ss));
        }
    }

}