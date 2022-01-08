namespace FancyLibrary.Nursery {

    public enum NurseryConfigType {
        FlushTime,
        AutoRestart,
    }
    
    public struct NurseryConfigStruct {
        public NurseryConfigType Type;
        public int FlushTime;
        public bool AutoRestart;
    }

}