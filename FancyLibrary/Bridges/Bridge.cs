#nullable enable
using FancyLibrary.Logger;


namespace FancyLibrary.Bridges {

    public abstract class Bridge {
        public delegate void MessageReceivedEventHandler(byte[] bytes);
        public delegate void MessageSentEventHandler(byte[] bytes);
        
        public abstract event MessageReceivedEventHandler MessageReceived;
        public abstract event MessageSentEventHandler MessageSent;

        public abstract void Receive();
        public abstract void Send(byte[] bytes);
        public abstract void Close();
    }

}