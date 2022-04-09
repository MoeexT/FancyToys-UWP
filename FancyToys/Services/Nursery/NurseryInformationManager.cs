using FancyLibrary.Nursery;

using FancyToys.Views;


namespace FancyToys.Services.Nursery {

    public class NurseryInformationManager {
        private NurseryView _nurseryView;
        public NurseryInformationManager(NurseryView view) {
            _nurseryView = view;
            MainPage.Poster.OnNurseryInformationStructReceived += _nurseryView.UpdateProcessInformation;
        }
    }

}
