namespace FancyLibrary.Nursery {

    public enum OperationType {
        Add = 1,
        Args = 2,
        Start = 3,
        Stop = 4,
        Restart = 5,
        Remove = 6,
        AutoRestart = 7,
    }

    public enum OperationResult {
        Void = -1,
        Failed = 0,
        Success = 1,
    }
    
    public struct OperationStruct {
        public OperationType Type;
        public OperationResult Code;
        public int Id;
        public byte[] Content;
        // public string PathName;
        // public string Args;
        // public string ProcessName;
    }
}