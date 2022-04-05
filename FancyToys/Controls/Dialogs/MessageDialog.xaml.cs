using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using FancyToys.Services;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板


namespace FancyToys.Controls.Dialogs {

    public sealed partial class MessageDialog: ContentDialog {
        public enum MessageLevel {
            Notice = 0,
            Info = 1,
            Warn = 2,
            Error = 3,
        }

        private static readonly Dictionary<MessageLevel, string> IconMap = new() {
            { MessageLevel.Notice, ((char)0xEBE8).ToString() },
            { MessageLevel.Info, ((char)0xEA80).ToString() },
            { MessageLevel.Warn, ((char)0xE7BA).ToString() },
            { MessageLevel.Error, ((char)0xEDAE).ToString() },
        };

        private static readonly Dictionary<MessageLevel, Brush> ColorMap = new() {
            { MessageLevel.Notice, new SolidColorBrush(Colors.PaleGreen) },
            { MessageLevel.Info, new SolidColorBrush(Colors.Cyan) },
            { MessageLevel.Warn, new SolidColorBrush(Colors.Yellow) },
            { MessageLevel.Error, new SolidColorBrush(Colors.Red) },
        };
        
        // https://stackoverflow.com/questions/33018346/only-a-single-contentdialog-can-be-open-at-any-time-error-while-opening-anoth
        public static MessageDialog ActiveDialog;
        private static TaskCompletionSource<bool> DialogAwaiter = new();

        public MessageDialog(string title, string message, string primaryText, MessageLevel level) {
            this.InitializeComponent();
            TitleIcon.Glyph = IconMap[level];
            TitleIcon.Foreground = ColorMap[level];
            TitleText.Text = title;
            TheTextBlock.Text = message;
            PrimaryButtonText = primaryText;
            TheTextBlock.TextWrapping = TextWrapping.WrapWholeWords;
            DefaultButton = ContentDialogButton.Primary;
        }

        public static async Task<bool> Notice(string title, string message, string primaryButtonText = "OK") {
            bool res = await CreateDialog(new MessageDialog(title, message, primaryButtonText, MessageLevel.Notice) {
                },
                awaitPreviousDialog: true);
            return res;
        }

        public static async Task<bool> Info(string title, string message, string primaryButtonText = "OK") {
            bool res = await CreateDialog(new MessageDialog(title, message, primaryButtonText, MessageLevel.Info) {
                },
                awaitPreviousDialog: true);
            return res;
        }

        public static async Task<bool> Warn(string title, string message, string primaryButtonText = "OK") {
            bool res = await CreateDialog(new MessageDialog(title, message, primaryButtonText, MessageLevel.Warn) {
                },
                awaitPreviousDialog: true);
            return res;
        }

        public static async Task<bool> Error(string title, string message, string primaryButtonText = "OK") {
            bool res = await CreateDialog(new MessageDialog(title, message, primaryButtonText, MessageLevel.Error) {
                },
                awaitPreviousDialog: true);
            return res;
        }

        public static async void CreateMessageDialog(MessageDialog Dialog, bool awaitPreviousDialog) {
            await CreateDialog(Dialog, awaitPreviousDialog);
        }

        public static async Task CreateMessageDialogAsync(MessageDialog Dialog, bool awaitPreviousDialog) {
            await CreateDialog(Dialog, awaitPreviousDialog);
        }

        private static void ActiveDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args) {
            DialogAwaiter.SetResult(true);
        }

        static async Task<bool> CreateDialog(ContentDialog Dialog, bool awaitPreviousDialog) {
            if (ActiveDialog != null) {
                if (awaitPreviousDialog) {
                    await DialogAwaiter.Task;
                    DialogAwaiter = new TaskCompletionSource<bool>();
                } else ActiveDialog.Hide();
            }
            ActiveDialog = (MessageDialog)Dialog;
            ActiveDialog.Closed += ActiveDialogClosed;
            ContentDialogResult result = await ActiveDialog.ShowAsync();
            ActiveDialog.Closed -= ActiveDialogClosed;

            if (result == ContentDialogResult.Primary) {
                return true;
            }
            return false;
        }

        private async void DragImageOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;

            if (e.DragUIOverride == null) {
                Logger.Fatal("DragUIOverride is null");
                await Error("Error", "Something went wrong");
                return;
            }
            e.DragUIOverride.Caption = "Drop to set";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.Handled = true;
        }

        private async void DropImageIn(object sender, DragEventArgs e) {
            DragOperationDeferral defer = e.GetDeferral();

            try {
                DataPackageView dpv = e.DataView;

                if (dpv.Contains(StandardDataFormats.StorageItems)) {
                    IReadOnlyList<IStorageItem> files = await dpv.GetStorageItemsAsync();

                    foreach (IStorageItem item in files) {
                        if (item.Name.EndsWith(".jpg") || item.Name.EndsWith(".jpeg") || item.Name.EndsWith(".png")) {
                            StorageFile file = StorageFile.GetFileFromPathAsync(item.Path).GetResults();
                            using IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                            BitmapImage bitmapImage = new();
                            await bitmapImage.SetSourceAsync(fileStream);
                            TheImage.Source = bitmapImage;
                            break;
                        }
                    }
                }
            } finally {
                defer.Complete();
            }
        }

        private void TheTextBlockDoubleTapped(object sender, DoubleTappedRoutedEventArgs e) {
            TextBlock tb = sender as TextBlock;
            DataPackage package = new();
            package.SetText(tb.Text);
            Clipboard.Clear();
            Clipboard.SetContent(package);
            Logger.Debug($"已写入剪贴板：{tb.Text}");
        }
    }

}
