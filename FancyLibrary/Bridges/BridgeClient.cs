namespace FancyLibrary.Bridges {

    public abstract class BridgeClient:Bridge {
        
        public  delegate void ClientClosedEventHandler();
        public delegate void ClientOpenedEventHandler();
        
        public abstract event ClientOpenedEventHandler OnClientOpened;
        public abstract event ClientClosedEventHandler OnClientClosed;
        

        public abstract override void Send(int port, byte[] bytes);
        public abstract override void Close();
    }

}