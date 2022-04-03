#nullable enable
namespace FancyLibrary.Bridges {

    internal abstract class Bridge {
        
        
        public delegate void MessageReceivedEventHandler(int port, byte[] bytes);

        public delegate void MessageSentEventHandler();

        public abstract event MessageReceivedEventHandler OnMessageReceived;
        public abstract event MessageSentEventHandler OnMessageSent;

        public abstract void Receive();

        public abstract void Send(int port, byte[] bytes);

        public abstract void Close();
    }

}