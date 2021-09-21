namespace FancyLibrary.Bridges {

    public abstract class BridgeServer:Bridge {
        
        public  delegate void ServerClosedEventHandler();
        public delegate void ServerOpenedEventHandler();
        public abstract event ServerOpenedEventHandler ServerOpened;
        public abstract event ServerClosedEventHandler ServerClosed;
        
        
        public abstract override void Send(string message);
        public abstract override void Close();
    }

}