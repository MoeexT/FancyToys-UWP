namespace FancyLibrary.Nursery {

    public enum NurseryOperationType {
        Add = 1,
        Args = 2,
        Start = 3,
        Stop = 4,
        Restart = 5,
        Remove = 6,
        AutoRestart = 7,
    }

    public enum NurseryOperationResult {
        Void = -1,
        Failed = 0,
        Success = 1,
    }
    
    public struct NurseryOperationStruct {
        public NurseryOperationType Type;
        public NurseryOperationResult Code;
        public int Id;
        public string Content;
        // public string PathName;
        // public string Args;
        // public string ProcessName;
    }
}