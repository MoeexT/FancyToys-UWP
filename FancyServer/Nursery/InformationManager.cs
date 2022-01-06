using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Utils;


namespace FancyServer.Nursery {

    public class InformationManager {

        private Bridge BridgeServer;
        private const int Port = Ports.NurseryInformation;
        public int UpdateSpan { get; set; } = 1000;

        public InformationManager(Bridge server) {
            BridgeServer = server ?? throw new ArgumentNullException(nameof(server));
        }

        public void run(ProcessManager manager) {
            Task.Run(
                async () => {
                    var infoList = new List<InformationStruct>();

                    while (true) {
                        Dictionary<int, ProcessInfo> processInfos = manager.Processes;
                        var s = new InformationStruct[processInfos.Count];

                        foreach (KeyValuePair<int, ProcessInfo> kv in processInfos) {
                            string processName = kv.Value.process.ProcessName;
                            PerformanceCounter cpuCounter = new("Process", "% Processor Time", processName);
                            PerformanceCounter memCounter = new("Process", "Working Set - Private", processName);

                            infoList.Add(
                                new InformationStruct {
                                    Id = kv.Value.id,
                                    ProcessName = kv.Value.process.ProcessName,
                                    CPU = cpuCounter.NextValue(),
                                    Memory = (int)memCounter.NextValue() >> 10,
                                }
                            );
                        }

                        if (infoList.Count > 0) { Send(infoList); }
                        await Task.Delay(UpdateSpan);
                    }
                }
            );
        }

        private void Send(List<InformationStruct> iss) {
            byte[] bytes = Converter.GetBytes(iss);
            if (bytes is not null) BridgeServer.Send(Port, bytes);
        }

    }

}