using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Nursery;


namespace FancyServer.Nursery {

    public class NurseryInformationManager {

        private int updateSpan = 1000;
        private const int minSpan = 20;
        private const int maxSpan = 5000;
        private readonly Messenger _messenger;

        public int UpdateSpan {
            get => updateSpan;
            set =>
                updateSpan = value < minSpan ? minSpan : value > maxSpan ? maxSpan : value;
        }

        public NurseryInformationManager(Messenger messenger) {
            _messenger = messenger;
        }

        public void run(ProcessManager manager) {
            Task.Run(
                async () => {
                    var infoList = new List<NurseryInformationStruct>();

                    while (true) {
                        Dictionary<int, ProcessInfo> processInfos = manager.Processes;
                        var s = new NurseryInformationStruct[processInfos.Count];

                        foreach (KeyValuePair<int, ProcessInfo> kv in processInfos) {
                            string processName = kv.Value.Pcs.ProcessName;
                            // TODO move new to outer sentence
                            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
                            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", processName);

                            infoList.Add(
                                new NurseryInformationStruct {
                                    Id = kv.Value.Id,
                                    ProcessName = kv.Value.Pcs.ProcessName,
                                    CPU = cpuCounter.NextValue(),
                                    Memory = (int)memCounter.NextValue() >> 10,
                                }
                            );
                        }

                        if (infoList.Count > 0) _messenger.Send(infoList);
                        await Task.Delay(UpdateSpan);
                    }
                }
            );
        }
    }

}