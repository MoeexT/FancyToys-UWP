using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Utils;


namespace FancyServer.Nursery {

    public class NurseryInformationManager {

        private int updateSpan = 1000;
        private const int minSpan = 20;
        private const int maxSpan = 5000;
        private readonly Bridge BridgeServer;
        private const int Port = Ports.NurseryInformation;

        public int UpdateSpan {
            get => updateSpan;
            set =>
                updateSpan = value < minSpan ? minSpan : value > maxSpan ? maxSpan : value;
        }

        public NurseryInformationManager(Bridge server) {
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
                            string processName = kv.Value.Pcs.ProcessName;
                            // TODO move new to outer sentence
                            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
                            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", processName);

                            infoList.Add(
                                new InformationStruct {
                                    Id = kv.Value.Id,
                                    ProcessName = kv.Value.Pcs.ProcessName,
                                    CPU = cpuCounter.NextValue(),
                                    Memory = (int)memCounter.NextValue() >> 10,
                                }
                            );
                        }

                        if (infoList.Count > 0) Send(infoList);
                        await Task.Delay(UpdateSpan);
                    }
                }
            );
        }

        private void Send(List<InformationStruct> iss) {
            byte[] bytes = Converter.GetBytes(iss);
            if (!(bytes is null)) BridgeServer.Send(Port, bytes);
        }

    }

}