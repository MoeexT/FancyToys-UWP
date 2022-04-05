using FancyLibrary.Logging;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

using FancyToys.Consts;
using FancyToys.Services;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板


namespace FancyToys.Views {

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerView: Page {
        public static ServerView CurrentInstance { get; private set; }

        public ServerView() {
            InitializeComponent();
            CurrentInstance = this;
        }

        public void PrintLog(LogStruct ls) {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Paragraph p = new();
                Run src = new() {
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Text = ls.Source + ' ',
                };
                Run msg = new() {
                    Foreground = new SolidColorBrush(SettingsConsts.LogForegroundColors[ls.Level]),
                    Text = ls.Content,
                };
                p.Inlines.Add(src);
                p.Inlines.Add(msg);
                FancyToysPanel.Blocks.Add(p);
            });
        }

        public void PrintStd(StdStruct ss) { }

        private void FancyToysPanelLoaded(object sender, RoutedEventArgs e) { Logger.Flush(); }
    }

}
