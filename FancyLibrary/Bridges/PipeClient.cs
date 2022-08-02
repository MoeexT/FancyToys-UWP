using System;
using System.IO.Pipes;


namespace FancyLibrary.Bridges {

    internal class PipeClient : BridgeClient {
        
        public override event ClientOpenedEventHandler OnClientOpened;
        public override event ClientClosedEventHandler OnClientClosed;

        private const string PipeName = "FancyPipe";
        private readonly NamedPipeClientStream _client;

        public PipeClient() { _client = new NamedPipeClientStream(".", $"LOCAL\\{PipeName}", PipeDirection.InOut); }

        
        public override void Receive() { throw new NotImplementedException(); }

        public void Send(int port, byte[] bytes) { throw new NotImplementedException(); }

        public override void Close() { throw new NotImplementedException(); }


        public void Connect() {
            _client.Connect();
            OnClientOpened?.Invoke();
            throw new NotImplementedException();
        }
    }

}
