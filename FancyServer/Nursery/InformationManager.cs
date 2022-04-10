using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FancyLibrary;
using FancyLibrary.Nursery;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    public class NurseryInformationManager {

        private int updateSpan = 1000;
        private const int minSpan = 20;
        private const int maxSpan = 5000;
        private readonly Messenger _messenger;
        private readonly ProcessManager _processManager;

        public int UpdateSpan {
            get => updateSpan;
            set =>
                updateSpan = value < minSpan ? minSpan : value > maxSpan ? maxSpan : value;
        }

        public NurseryInformationManager(Messenger messenger, ProcessManager processManager) {
            _messenger = messenger;
            _processManager = processManager;
        }

        public void run() {
            Task.Run(
                async () => {
                    bool shouldClear = false;
                    List<NurseryInformationStruct> infoList = new();

                    while (true) {
                        try {
                            Fetch(infoList);

                            if (infoList.Count > 0) {
                                _messenger.Send(infoList);
                                infoList.Clear();
                                shouldClear = true;
                            } else {
                                if (shouldClear) {
                                    _messenger.Send(infoList);
                                    shouldClear = false;
                                }
                            }
                            await Task.Delay(updateSpan);
                        } catch (Exception e) {
                            Logger.Error(e.ToString());
                            await Task.Delay(updateSpan);
                        }
                    }
                }
            );
        }

        public void Flush() {
            try {
                List<NurseryInformationStruct> infoList = new();
                Fetch(infoList);
                if (infoList.Count > 0) _messenger.Send(infoList);
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        }

        private void Fetch(List<NurseryInformationStruct> list) {
            if (list.Count > 0) {
                list.Clear();
            }

            list.AddRange(_processManager
            .GetAliveProcesses().
            Select(info => new NurseryInformationStruct {
                Id = info.Pcs.Id,
                ProcessName = info.Pcs.ProcessName,
                CPU = info.CpuCounter.NextValue(),
                Memory = (int)info.MemCounter.NextValue() >> 10,
            }));
        }
    }

}
