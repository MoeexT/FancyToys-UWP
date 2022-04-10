using System;

using FancyLibrary.Logging;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
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

        public async void PrintLog(LogStruct ls) {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Color color = SettingsConsts.LogForegroundColors[ls.Level];
                bool highlight = SettingsConsts.HighlightedLogLevels.Contains(ls.Level);
                FontWeight weight = highlight ? FontWeights.Bold : FontWeights.Normal;

                Paragraph p = new();

                Run src = new() {
                    Text = ls.Source + ' ',
                    Foreground = new SolidColorBrush(highlight ? color : Colors.Gray),
                    // FontWeight = weight,
                };

                Run msg = new() {
                    Text = ls.Content,
                    Foreground = new SolidColorBrush(color),
                    FontWeight = weight,
                };

                p.Inlines.Add(src);
                p.Inlines.Add(msg);
                FancyToysPanel.Blocks.Add(p);
                FancyToysScrollViewer.ScrollToVerticalOffset(FancyToysScrollViewer.ScrollableHeight);
            });
        }

        public void PrintStd(StdStruct ss) {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Paragraph p = new();

                Run src = new() {
                    Text = $"<{ss.Sender}> ",
                    Foreground = new SolidColorBrush(Colors.RoyalBlue),
                };

                Run msg = new() {
                    Text = ss.Content,
                    Foreground = new SolidColorBrush(SettingsConsts.StdForegroundColors[ss.Level]),
                };

                p.Inlines.Add(src);
                p.Inlines.Add(msg);
                FancyToysPanel.Blocks.Add(p);
                FancyToysScrollViewer.ScrollToVerticalOffset(FancyToysScrollViewer.ScrollableHeight);
            });
        }

        private void FancyToysPanelLoaded(object sender, RoutedEventArgs e) {
            Logger.Flush();
            StdLogger.Flush();
        }
    }

}
