using System;

using FancyLibrary.Logger;


namespace FancyLibrary.Setting {

    
    public enum SettingType {
        Form = 1, // ActionManager
        Message = 2, // MessageManager
        Log = 3, // LogManager
    }
    
    public struct FormSettingStruct {
        
    }

    public struct MessageSettingStruct {
        
    }
    
    public struct LogSettingStruct {
        /**
         * LogLevel: 0~7, 0b000~0b111
         * this is a bit vector
         * _, _, _,
         * dialog, std, log
         */
        public byte LogLevel;

        public override string ToString() {
            return $"LogLevel: {LogLevel}";
        }
    }

    public struct SettingStruct {
        public SettingType Type;
        public byte[] Content;

        public override string ToString() {
            return $"SettingStruct{{\nSettingType: {Type}, \nContent: {BitConverter.ToString(Content)}\n}}";
        }
    }

}