using System;
using System.Collections.Generic;
using System.IO;

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using FancyLibrary.Nursery;

using FancyToys.Controls.Dialogs;
using FancyToys.Services;
using FancyToys.Services.Nursery;


namespace FancyToys.Views {

    public partial class NurseryView {
        private void TryAdd(string pathName) {
            NurseryOperationManager.Add(pathName);
        }

        public void Add(int pid, string pathName) {
            NurseryInfoMap[pid] = new NurseryInfo();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                ToggleSwitch ts = NewSwitch(pid, pathName);
                ProcessSwitchList.Items!.Add(ts);
            });
        }

        private async void TryRemove(int pid) {
            if (!NurseryInfoMap.TryGetValue(pid, out NurseryInfo ni)) return;
            bool confirm = true;

            if (ni.Twitch.IsOn) {
                confirm &= await MessageDialog.Warn("进程未退出", "继续操作可能丢失工作内容", "仍然退出");
            }
            if (!confirm) return;

            NurseryOperationManager.Remove(pid);
        }

        public async void Remove(int pid) {
            if (NurseryInfoMap.ContainsKey(pid)) {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    ProcessSwitchList.Items!.Remove(NurseryInfoMap[pid].Twitch);
                });
                NurseryInfoMap.Remove(pid);
            } else {
                Logger.Fatal($"NurseryInfoMap doesn't contain {pid}");
            }
        }

        public void ToggleSwitch(int pid, bool isOn) {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if (NurseryInfoMap.TryGetValue(pid, out NurseryInfo ni) && ni.Twitch.IsOn != isOn) {
                    ni.Twitch.IsOn = isOn;
                }
            });
        }

        private ToggleSwitch NewSwitch(int pid, string pathName) {
            string pn = Path.GetFileName(pathName);

            ToggleSwitch twitch = new() {
                IsOn = false,
                Tag = pid,
                FontSize = 14,
                ContextFlyout = NewMenu(pid, pathName),
                OnContent = pn + " is running",
                OffContent = pn + " stopped"
            };
            ToolTipService.SetToolTip(twitch, pathName);

            twitch.Toggled += (sender, e) => {
                if (sender is not ToggleSwitch ts) return;
                if (!NurseryInfoMap.TryGetValue(pid, out NurseryInfo ni)) return;

                if (!ni.ServerStart && ts.IsOn) NurseryOperationManager.Start(pid);
                else if (!ni.ServerStop && !ts.IsOn) NurseryOperationManager.Stop(pid);
            };
            NurseryInfoMap[pid].Twitch = twitch;

            return twitch;
        }

        private MenuFlyout NewMenu(int pid, string pathName) {
            MenuFlyout menu = new();

            MenuFlyoutItem ai = new() {
                Icon = new FontIcon { Glyph = "\uE723" },
                Tag = pid,
                Text = "参数",
            };
            ai.Click += ArgsButtonClick;

            MenuFlyoutItem ci = new() {
                Icon = new FontIcon { Glyph = "\uE8C8" },
                Tag = pid,
                Text = "复制文件名",
            };
            ci.Click += (s, e) => {
                DataPackage dataPackage = new();
                dataPackage.SetText(pathName);
                Clipboard.SetContent(dataPackage);
            };
            ToolTipService.SetToolTip(ci, pathName);

            MenuFlyoutItem ri = new() {
                Icon = new FontIcon { Glyph = "\uE74D" },
                Tag = pid,
                Text = "删除",
            };
            ri.Click += (s, e) => {
                TryRemove(pid);
            };

            menu.Items.Add(ai);
            menu.Items.Add(ci);
            menu.Items.Add(ri);

            return menu;
        }

        public async void UpdateProcessInformation(List<NurseryInformationStruct> nisList) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var rmList = new List<ProcessInformation>();
                var insDict = new Dictionary<int, NurseryInformationStruct>();

                foreach (NurseryInformationStruct nis in nisList) {
                    insDict[nis.Id] = nis;
                }

                foreach (ProcessInformation pi in ProcessInfoList) {
                    if (insDict.TryGetValue(pi.PID, out NurseryInformationStruct si)) {
                        pi.SetCPU(si.CPU);
                        pi.SetMemory(si.Memory);
                        insDict.Remove(pi.PID);
                    } else {
                        rmList.Add(pi);
                    }
                }

                foreach (ProcessInformation rp in rmList) {
                    ProcessInfoList.Remove(rp);
                }

                foreach (NurseryInformationStruct si in insDict.Values) {
                    ProcessInfoList.Add(new ProcessInformation(si));
                }
            });
        }

        private void SortData(Comparison<ProcessInformation> comparison) {
            var sortableList = new List<ProcessInformation>(ProcessInfoList);
            sortableList.Sort(comparison);
            ProcessInfoList.Clear();

            foreach (ProcessInformation pi in sortableList) {
                ProcessInfoList.Add(pi);
            }
        }

        /// <summary>
        /// 动态改变switchToggle的样式
        /// From https://blog.csdn.net/lindexi_gd/article/details/104992276
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Style SetStyle(DependencyProperty property, object value) {
            Style style = new Style() {
                TargetType = typeof(ListBoxItem)
            };
            style.Setters.Add(new Setter(property, value));
            style.Setters.Add(new Setter(PaddingProperty, "10,0,0,0"));
            ProcessSwitchList.ItemContainerStyle = style;

            return style;
        }
    }

}
