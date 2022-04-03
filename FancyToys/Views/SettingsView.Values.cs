using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;


namespace FancyToys.Views {

    public partial class SettingsView {
        public double OpacitySliderValue {
            get => (double)LocalSettings.Values[nameof(OpacitySliderValue)];
            set {
                LocalSettings.Values[nameof(OpacitySliderValue)] = value;
                OnSettingChanged?.Invoke(LocalSettings, nameof(OpacitySliderValue));
            }
        }
        
        public Brush MonitorFontColor {
            get => (Brush)LocalSettings.Values[nameof(MonitorFontColor)];
            set {
                LocalSettings.Values[nameof(MonitorFontColor)] = value;
                OnSettingChanged?.Invoke(LocalSettings, nameof(MonitorFontColor));
            }
        }

        public ElementTheme CurrentTheme {
            get => (ElementTheme)LocalSettings.Values[nameof(CurrentTheme)];
            set {
                if (Window.Current.Content is not FrameworkElement framework) return;
                framework.RequestedTheme = value;
                LocalSettings.Values[nameof(CurrentTheme)] = value.ToString();
                OnSettingChanged?.Invoke(LocalSettings, nameof(CurrentTheme));
            }
        }

    }

}
