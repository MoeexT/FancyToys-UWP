using System;
using System.Collections.Generic;
using System.IO;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using FancyLibrary.Nursery;

using FancyToys.Controls.Dialogs;
using FancyToys.Services;
using FancyToys.Services.Nursery;


namespace FancyToys.Views {

    public partial class NurseryView {
        private async void TryAdd(string pathName) {
            NurseryOperation.Add(pathName);
        }

        public void Add(int pid, string psName) {
            PidArgsMap[pid] = "";
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                ToggleSwitch ts = NewSwitch(pid);
                ts.OnContent = psName + " is running";
                ts.OffContent = psName + " stopped";
                ProcessSwitchList.Items.Add(ts);
            });
        }

        private void TryRemove(int pid) {
            NurseryOperation.Remove(pid);
        }

        public void Remove(int pid) {
            if (PidSwitchMap.ContainsKey(pid) && PidArgsMap.ContainsKey(pid)) {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    ProcessSwitchList.Items.Remove(PidSwitchMap[pid]);
                });
                PidArgsMap.Remove(pid);
                PidSwitchMap.Remove(pid);
            } else {
                Logger.Fatal($"PidSwitchMap(${PidSwitchMap.ContainsKey(pid)}) or " +
                    $"PidArgsMap(${PidArgsMap.ContainsKey(pid)}) doesn't contain {pid}");
            }
        }

        public void UpdateSwitch(int pid, string processName) {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                PidSwitchMap[pid].OnContent = processName + " is running";
                PidSwitchMap[pid].OffContent = processName + " stopped";
            });
        }

        public void ToggleSwitch(int pid, bool isOn) {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                PidSwitchMap[pid].IsOn = isOn;
            });
        }

        private ToggleSwitch NewSwitch(int pid) {
            ToggleSwitch twitch = new() {
                IsOn = false,
                Tag = pid,
                FontSize = 14,
            };
            twitch.Toggled += SwitchToggled;
            twitch.ContextFlyout = NewMenu(pid);
            PidSwitchMap[pid] = twitch;
            return twitch;
        }

        private MenuFlyout NewMenu(int pid) {
            MenuFlyout menu = new();
            MenuFlyoutItem ai = new() {
                Icon = new FontIcon { Glyph = "\uE723" },
                Tag = pid,
                Text = "参数",
            };
            ai.Click += ArgsButtonClick;
            MenuFlyoutItem ri = new() {
                Icon = new FontIcon { Glyph = "\uE74D" },
                Tag = pid,
                Text = "删除",
            };
            ri.Click += DeleteButtonClick;
            menu.Items.Add(ai);
            menu.Items.Add(ri);
            return menu;
        }

        public void UpdateProcessInformation(Dictionary<int, NurseryInformationStruct> ins) {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                var rmlist = new List<ProcessInformation>();

                foreach (ProcessInformation pi in ProcessInfoList) {
                    if (ins.TryGetValue(pi.PID, out NurseryInformationStruct si)) {
                        pi.SetCPU(si.CPU);
                        pi.SetMemory(si.Memory);
                        ins.Remove(pi.PID);
                    } else {
                        rmlist.Add(pi);
                    }
                }

                foreach (ProcessInformation rp in rmlist) {
                    ProcessInfoList.Remove(rp);
                }

                foreach (NurseryInformationStruct si in ins.Values) {
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
        private Style SetStyle(DependencyProperty property, object value)
        {
            Style style = new Style()
            {
                TargetType = typeof(ListBoxItem)
            };
            style.Setters.Add(new Setter(property, value));
            style.Setters.Add(new Setter(PaddingProperty, "10,0,0,0"));
            ProcessSwitchList.ItemContainerStyle = style;
            return style;
        }
    }

}
