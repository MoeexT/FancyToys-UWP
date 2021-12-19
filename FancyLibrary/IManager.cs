
using FancyLibrary.Logger;


namespace FancyLibrary {

    public interface IManager {
        public delegate void OnMessageReadyHandler(object sdu);
        // public delegate void OnMessageFetchHandler(string message);
        
        public void Deal(byte[] bytes);

        public void Send(object sdu);
    }

}