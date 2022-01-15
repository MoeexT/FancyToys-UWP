using System.Collections.Generic;

using Windows.UI;

using FancyLibrary.Logging;


namespace FancyToys.Consts {

    public static class SettingsConsts {
        public static readonly Dictionary<LogLevel, Color> LogForegroundColors = new() {
            { LogLevel.Trace, Colors.Gray },
            { LogLevel.Debug, Colors.Cyan },
            { LogLevel.Info, Colors.MediumSpringGreen },
            { LogLevel.Warn, Colors.Yellow },
            { LogLevel.Error, Colors.DeepPink },
            { LogLevel.Fatal, Colors.Red },
        };
        
        public static readonly Dictionary<StdType, Color> StdForegroundColors = new() {
            { StdType.Input, Colors.DodgerBlue },
            { StdType.Output, Colors.Aquamarine },
            { StdType.Error, Colors.Firebrick },
        };
    }

}