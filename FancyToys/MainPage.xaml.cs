﻿using System;
using System.Collections.Generic;
using System.Linq;

using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using FancyLibrary;

using muxc = Microsoft.UI.Xaml.Controls;

using FancyToys.Views;
using FancyToys.Services;

using NLog.Config;
using NLog.Targets;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板


namespace FancyToys {

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage: Page {

        private readonly List<(string Tag, Type View)> views;
        private const string _logFileName = "fancy_toys.log";
        public static Messenger Poster { get; private set; }
        public MainPage() {
            this.InitializeComponent();
            ContentFrame.CacheSize = 64;
            views = new List<(string Tag, Type View)> {
                ("nursery", typeof(NurseryView)),
                ("teleport", typeof(TeleportView)),
                ("fancyServer", typeof(ServerView)),
            };
            Poster = new Messenger(624, 626);
            Poster.OnMessengerReady += OnServerConnected;
            Poster.OnMessengerSleep += OnServerDisconnected;
            Window.Current.SetTitleBar(AppTitleBar);
            LoggingConfiguration config = new();
            FileTarget logFile = new("logfile") {
                FileName = "${specialfolder:folder=LocalApplicationData}/" + _logFileName,
                Layout = "${longdate} ${level} ${message} ${exception}"
            };
            DebuggerTarget logDebugger = new("logdebugger");
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logDebugger);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logFile);
            NLog.LogManager.Configuration = config;
            Logger.Info("FancyToys started.");
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e) {
            ContentFrame.Navigated += OnNavigated;
            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate("nursery", new EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args) {
            if (args.IsSettingsInvoked == true) {
                NavView_Navigate("settings", new DrillInNavigationTransitionInfo());
            } else if (args.InvokedItemContainer != null) {
                string navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, new DrillInNavigationTransitionInfo());
            }
        }

        private void NavView_SelectionChanged(
            muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args) {
            if (args.IsSettingsSelected == true) {
                NavView_Navigate("settings", new DrillInNavigationTransitionInfo());
            } else if (args.SelectedItemContainer != null) {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, new DrillInNavigationTransitionInfo());
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo) {
            Type page = null;

            if (navItemTag == "settings") { page = typeof(SettingsView); } else {
                (string Tag, Type View) item = views.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                page = item.View;
            }
            Type preNavPageType = ContentFrame.CurrentSourcePageType;

            if (page is not null && preNavPageType != page) { ContentFrame.Navigate(page, null, transitionInfo); }
        }

        private void OnNavigated(object sender, NavigationEventArgs e) {
            //NavView.IsBackEnabled = ContentFrame.CanGoBack;
            if (ContentFrame.SourcePageType == typeof(SettingsView)) {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            } else if (ContentFrame.SourcePageType != null) {
                (string Tag, Type View) item = views.FirstOrDefault(p => p.View == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems.OfType<muxc.NavigationViewItem>().
                                               First(n => n.Tag.Equals(item.Tag));
            }
        }

        private async void OnServerConnected() {
            Logger.Debug("Set icon green");
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                FancyServer.Icon = new FontIcon() {
                    Glyph = "\uE95E",
                    Foreground = new SolidColorBrush(Colors.LightGreen)
                };
            });
        }

        private async void OnServerDisconnected() {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                FancyServer.Icon = new FontIcon {
                    Glyph = "\uEA92",
                    Foreground = new SolidColorBrush(Colors.Red),
                };
            }); 
        }

        private void NavView_GettingFocus(UIElement sender, GettingFocusEventArgs args) {
            args.TryCancel();
        }
    }

}
