using System;

using FancyLibrary.Logging;


namespace FancyLibrary.Setting {

    public enum SettingType {
        LogLevel = 1,
    }

    public struct SettingStruct {
        public SettingType Type;
        /**
         * bit vector
         * LogLevel: 0~15, 0b00000~0b01111
         *  _,     _,   _._._,
         * dialog, std, log
         * dialog: 0 forever
         * std: {0, 1, 2}
         * log: {0b000~0b111}
         */
        public int LogLevel;
    }

}