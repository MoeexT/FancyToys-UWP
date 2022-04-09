using System.ComponentModel;

using FancyLibrary.Nursery;


namespace FancyToys.Services.Nursery {

    internal class ProcessInformation: INotifyPropertyChanged {

        private const double GB = 1 << 30;

        private string process;
        private int pid;
        public double cpu;
        public int memory;

        public int PID {
            get => pid;
            private set {
                pid = value;
                RaisePropertyChanged(nameof(PID));
            }
        }

        public string Process {
            get => process;
            private set {
                process = value;
                RaisePropertyChanged(nameof(Process));
            }
        }

        public string CPU { get => $"{cpu:F}%"; }

        public string Memory { get => memory < GB ? $"{memory:N0}KB" : $"{memory >> 10:N0}MB"; }

        public void SetCPU(double _cpu) {
            cpu = _cpu;
            RaisePropertyChanged(nameof(CPU));
        }

        public void SetMemory(int mem) {
            memory = mem;
            RaisePropertyChanged(nameof(Memory));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ProcessInformation(NurseryInformationStruct nfs) {
            pid = nfs.Id;
            process = nfs.ProcessName;
            cpu = nfs.CPU;
            memory = nfs.Memory;
        }

        protected void RaisePropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProcessInformation() { }

        public override string ToString() { return $"{{{Process}, {PID}, {CPU}, {Memory}}}"; }
    }

}
