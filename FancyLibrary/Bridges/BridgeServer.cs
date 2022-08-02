namespace FancyLibrary.Bridges {

    internal abstract class BridgeServer:Bridge {
        
        public delegate void ServerClosedEventHandler();
        public delegate void ServerOpenedEventHandler();
        public abstract event ServerOpenedEventHandler ServerOpened;
        public abstract event ServerClosedEventHandler ServerClosed;
        
        
        public abstract void Send(int port, byte[] bytes);
        public abstract override void Close();
    }

}
