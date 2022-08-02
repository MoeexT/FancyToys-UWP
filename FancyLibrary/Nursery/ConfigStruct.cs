namespace FancyLibrary.Nursery {

    public enum NurseryConfigType {
        FlushTime,
        AutoRestart,
    }
    
    public struct NurseryConfigStruct: IStruct {
        public NurseryConfigType Type;
        public int FlushTime;
        public bool AutoRestart;

        public override string ToString() => $"{{Type:{Type}, FlushTime:{FlushTime}, AutoRestart:{AutoRestart}}}";
    }

}
