using Windows.UI.Xaml.Controls;


namespace FancyToys.Services.Nursery {

    public class NurseryInfo {
        // public bool ServerAdd { get; set; }
        // public bool ServerRemove { get; set; }
        public bool ServerStart { get; set; }
        public bool ServerStop { get; set; }
        public string Args { get; set; }
        // public string PathName { get; set; }
        public ToggleSwitch Twitch { get; set; }
    }

}
