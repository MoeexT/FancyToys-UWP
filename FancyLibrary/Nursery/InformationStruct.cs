namespace FancyLibrary.Nursery {

    public struct NurseryInformationStruct {
        public int Id;
        public string ProcessName;
        public double CPU;
        public int Memory;
        
        public bool Equals(NurseryInformationStruct other) {
            return Id == other.Id && ProcessName == other.ProcessName && CPU.Equals(other.CPU) && Memory == other.Memory;
        }
        
        public override string ToString() { return $"{Id}, {ProcessName}, {CPU}, {Memory}"; }
    } 

}