using System;


namespace FancyLibrary.Bridges {

    internal class PipeServer : BridgeServer {
        
        public override event ServerOpenedEventHandler ServerOpened;
        public override event ServerClosedEventHandler ServerClosed;


        public override void Receive() { throw new NotImplementedException(); }

        public override void Send(int port, byte[] bytes) { throw new NotImplementedException(); }

        public override void Close() { throw new NotImplementedException(); }
        public void Connect() { throw new NotImplementedException(); }
    }

}
