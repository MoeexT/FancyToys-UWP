using System;
using System.Runtime.InteropServices;


namespace FancyLibrary.Nursery {

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NurseryInformationStruct: IStruct {
        [FieldOffset(0)]
        public int Id;
        [FieldOffset(4)]
        public int Memory;
        [FieldOffset(8)]
        public double CPU;
        [FieldOffset(16)]
        public string ProcessName;
        
        
        public bool Equals(NurseryInformationStruct other) {
            return Id == other.Id && ProcessName == other.ProcessName && CPU.Equals(other.CPU) && Memory == other.Memory;
        }
        
        public override string ToString() => $"{{Id: {Id}, ProcessName: {ProcessName}, CPU: {CPU}, Memory: {Memory}}}";
        
        } 

}
