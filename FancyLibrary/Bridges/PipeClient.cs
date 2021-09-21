using System;
using System.IO.Pipes;


namespace FancyLibrary.Bridges {

    public class PipeClient : BridgeClient {
        
        public override event ClientOpenedEventHandler ClientOpened;
        public override event ClientClosedEventHandler ClientClosed;

        public override event MessageReceivedEventHandler MessageReceived;
        public override event MessageSentEventHandler MessageSent;

        private const string PipeName = "FancyPipe";
        private readonly NamedPipeClientStream _client;

        public PipeClient() { _client = new NamedPipeClientStream(".", $"LOCAL\\{PipeName}", PipeDirection.InOut); }

        
        public override void Receive() { throw new NotImplementedException(); }

        public override void Send(string message) { throw new NotImplementedException(); }

        public override void Close() { throw new NotImplementedException(); }


        public void Connect() {
            _client.Connect();
            ClientOpened?.Invoke();
            throw new NotImplementedException();
        }
    }

}