using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using FancyToys.Controls.Dialogs;
using FancyToys.Services;
using FancyToys.Services.Nursery;

using Microsoft.Toolkit.Uwp.UI.Controls;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板


namespace FancyToys.Views {

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NurseryView: Page {
        private ObservableCollection<ProcessInformation> ProcessInfoList { get; }

        public Dictionary<int, NurseryInfo> NurseryInfoMap { get; }

        public NurseryView() {
            InitializeComponent();
            ProcessInfoList = new ObservableCollection<ProcessInformation>();
            NurseryInfoMap = new Dictionary<int, NurseryInfo>();
            NurseryOperationManager nurseryOperation = new(this);
            NurseryConfigManager nurseryConfig = new(this);
            NurseryInformationManager nurseryInformation = new(this);
        }

        private async void DropAreaDrop(object sender, DragEventArgs e) {
            DragOperationDeferral defer = e.GetDeferral();

            try {
                DataPackageView dpv = e.DataView;

                if (!dpv.Contains(StandardDataFormats.StorageItems)) return;

                IReadOnlyList<IStorageItem> files = await dpv.GetStorageItemsAsync();

                foreach (IStorageItem item in files) {
                    if (item.Name.EndsWith(".exe")) {
                        TryAdd(item.Path);
                    }
                }
            } finally {
                defer.Complete();
            }
        }

        private void DropAreaDragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride!.Caption = "拖放以添加";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.Handled = true;
        }

        private async void AddFileFlyoutItemClick(object sender, RoutedEventArgs e) {
            FileOpenPicker picker = new() {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.HomeGroup
            };
            picker.FileTypeFilter.Add(".exe");
            StorageFile file = await picker.PickSingleFileAsync();

            // TODO: 可能选择多个文件
            if (file != null) {
                TryAdd(file.Path);
            }
        }

        private void StopAllFlyoutItemClick(object sender, RoutedEventArgs e) {
            if (ProcessSwitchList.Items == null) return;
            foreach (ToggleSwitch ts in ProcessSwitchList.Items) {
                if (ts.IsOn) {
                    NurseryOperationManager.Stop((int)ts.Tag);
                }
            }
        }

        private async void RemoveAllFlyoutItemClick(object sender, RoutedEventArgs e) {
            if (ProcessSwitchList.Items == null) return;
            foreach (ToggleSwitch ts in ProcessSwitchList.Items) {
                TryRemove((int)ts.Tag);
            }
        }

        private void HelpFlyoutItemClick(object sender, RoutedEventArgs e) { DropFileTechingTip.IsOpen = true; }

        private void AboutFlyoutItemClick(object sender, RoutedEventArgs e) {
            _ = MessageDialog.Info("Nursery v0.1.3",
                "Nursery is a simple daemon process manager powered by FancyServer and it will keep your application online.");
        }

        private async void ArgsButtonClick(object sender, RoutedEventArgs e) {
            if (sender is not MenuFlyoutItem ai) {
                Logger.Error("args-button is null");
                return;
            }

            int pid = (int)ai.Tag;
            InputDialog inputDialog = new("Nursery", "输入参数", NurseryInfoMap[pid].Args ?? string.Empty);
            await inputDialog.ShowAsync();

            if (inputDialog.isSaved) {
                NurseryInfoMap[pid].Args = inputDialog.inputContent;
                NurseryOperationManager.AttachArgs(pid, inputDialog.inputContent);
            }
        }

        private void ListBoxSizeClick(object sender, RoutedEventArgs e) {
            ProcessSwitchList.ItemContainerStyle = SetStyle(HeightProperty, ((MenuFlyoutItem)sender).Tag);
        }

        private void DataGridSizeClick(object sender, RoutedEventArgs e) { }

        private void FlushSpeedClick(object sender, RoutedEventArgs e) {
            // SettingsClerk.Clerk.STFlushTime = int.Parse((sender as MenuFlyoutItem).Tag as string);
            Logger.Warn("this feature is not implemented yet");
        }

        private void ProcessGridSorting(object sender, DataGridColumnEventArgs e) {
            switch (e.Column.Header.ToString()) {
                case "Process":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending) {
                        SortData((x, y) => x.Process.CompareTo(y.Process));
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    } else {
                        SortData((x, y) => -x.Process.CompareTo(y.Process));
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "PID":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending) {
                        SortData((x, y) => x.PID - y.PID);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    } else {
                        SortData((x, y) => y.PID - x.PID);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "CPU":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending) {
                        SortData((x, y) => (int)(x.cpu - y.cpu));
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    } else {
                        SortData((x, y) => (int)(y.cpu - x.cpu));
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "Memory":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending) {
                        SortData((x, y) => x.memory - y.memory);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    } else {
                        SortData((x, y) => y.memory - x.memory);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
            }

            foreach (DataGridColumn dc in ProcessGrid.Columns) {
                if (dc.Header.ToString() != e.Column.Header.ToString()) {
                    dc.SortDirection = null;
                }
            }
        }

    }
}
