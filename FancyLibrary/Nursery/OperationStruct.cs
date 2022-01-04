namespace FancyLibrary.Nursery {

    public enum OperationType {
        Add = 1,
        Start = 2,
        Stop = 3,
        Restart = 4,
        Remove = 5,
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
        public string PathName;
        public string Args;
        public string ProcessName;
    }
}