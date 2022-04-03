using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using FancyLibrary.Logging;

using FancyToys.Consts;
using FancyToys.Services;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板


namespace FancyToys.Views {

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsView: Page {

        public delegate void SettingChangedEventHandler(ApplicationDataContainer settings, string key);

        public event SettingChangedEventHandler OnSettingChanged;
        private ApplicationDataContainer LocalSettings { get; }
        private List<ComboBoxItem> LogComboItemList { get; }
        private List<ComboBoxItem> StdComboItemList { get; }

        public SettingsView() {
            InitializeComponent();

            MethodBase method = new StackTrace().GetFrame(1).GetMethod();

            LocalSettings = ApplicationData.Current.LocalSettings;

            LogComboItemList = new List<ComboBoxItem>();

            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel))) {
                LogComboItemList.Add(new ComboBoxItem {
                    Content = level,
                    Foreground = new SolidColorBrush(SettingsConsts.LogForegroundColors[level]),
                });
            }

            StdComboItemList = new List<ComboBoxItem>();

            foreach (StdType type in Enum.GetValues(typeof(StdType))) {
                StdComboItemList.Add(new ComboBoxItem {
                    Content = type,
                    Foreground = new SolidColorBrush(SettingsConsts.StdForegroundColors[type]),
                });
            }
            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings() {
            OpacitySliderValue = LocalSettings.Values[nameof(OpacitySliderValue)] as double? ?? 0.6;
            ElementTheme Theme = (ElementTheme)Enum.Parse(typeof(ElementTheme),
                LocalSettings.Values[nameof(CurrentTheme)] as string ?? ElementTheme.Default.ToString());

            switch (Theme) {
                case ElementTheme.Dark:
                    DarkThemeButton.IsChecked = true;
                    break;
                case ElementTheme.Light:
                    LightThemeButton.IsChecked = true;
                    break;
                default:
                    SystemThemeButton.IsChecked = true;
                    break;
            }
        }

        private void ChangeTheme(object sender, RoutedEventArgs e) {
            if (sender is null) return;
            CurrentTheme = (sender as RadioButton)!.Content switch {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                "System" => ElementTheme.Default,
                _ => ElementTheme.Default
            };
        }

        private void LogLevelChanged(object sender, SelectionChangedEventArgs e) {
            if (sender != LogLevelComboBox) return;

            ComboBoxItem item = (ComboBoxItem)LogLevelComboBox.SelectedItem;
            TextBlock header = (TextBlock)LogLevelComboBox.Header;
            Brush originHeaderForeground = header!.Foreground;
            LogLevelComboBox.Foreground = item!.Foreground;
            header.Foreground = originHeaderForeground;
            Logger.Level = item.Content is null ? Logger.Level : (LogLevel)item.Content;
        }

        private void StdLevelChanged(object sender, SelectionChangedEventArgs e) {
            if (sender != StdLevelComboBox) return;
            ComboBoxItem item = (ComboBoxItem)StdLevelComboBox.SelectedItem;
            TextBlock header = (TextBlock)StdLevelComboBox.Header;
            Brush originHeaderForeground = header!.Foreground;
            StdLevelComboBox.Foreground = item!.Foreground;
            header.Foreground = originHeaderForeground;
            StdLogger.Level = item.Content is null ? StdLogger.Level : (StdType)item.Content;
        }

        private int IndexOfLogLevels() {
            foreach (ComboBoxItem item in LogComboItemList) {
                if (item.Content is LogLevel level && level == Logger.Level) {
                    return LogComboItemList.IndexOf(item);
                }
            }
            Logger.Warn($"LogLevel {Logger.Level} is not in {nameof(LogComboItemList)}.");
            return 0;
        }

        private int IndexOfStdLevels() {
            foreach (ComboBoxItem item in StdComboItemList) {
                if (item.Content is StdType level && level == StdLogger.Level) {
                    return StdComboItemList.IndexOf(item);
                }
            }
            Logger.Warn($"StdLevel {StdLogger.Level} is not in {nameof(StdComboItemList)}.");
            return 0;
        }

        private void Opacity_OnTapped(object sender, TappedRoutedEventArgs e) {
            if (sender is not TextBlock opacityPreview) return;

            if (opacityPreview.Tag.ToString().Equals("White")) {
                MonitorFontColor = new SolidColorBrush(Colors.Black);
                opacityPreview.Tag = "Black";
            } else {
                MonitorFontColor = new SolidColorBrush(Colors.White);
                opacityPreview.Tag = "White";
            }
        }
    }

}
