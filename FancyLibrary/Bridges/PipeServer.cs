using System;


namespace FancyLibrary.Bridges {

    public class PipeServer : BridgeServer {
        
        public override event ServerOpenedEventHandler ServerOpened;
        public override event ServerClosedEventHandler ServerClosed;
        
        public override event MessageReceivedEventHandler MessageReceived;
        public override event MessageSentEventHandler MessageSent;
        
        
        public override void Receive() { throw new NotImplementedException(); }

        public override void Send(byte[] bytes) { throw new NotImplementedException(); }

        public override void Close() { throw new NotImplementedException(); }
        public void Connect() { throw new NotImplementedException(); }
    }

}