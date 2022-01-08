using System.Text;


namespace FancyLibrary {

    // global settings
    public static class Consts {
        public static Encoding Encoding { get; } = Encoding.UTF8;
    }
    
    public static class Ports {
        public const int Bridge = 0;
        public const int Logger = 1;  // normal log
        public const int Stdio = 2;  // stdout stderr
        public const int Dialog = 3;  // dialog
        public const int Action = 4;
        public const int Setting = 5;
        public const int NurseryConfig = 6;
        public const int NurseryInformation = 7;
        public const int NurseryOperation = 8;
        
    }

}