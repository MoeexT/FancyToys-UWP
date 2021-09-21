namespace FancyLibrary.Bridges {

    public abstract class Bridge {
        
        public delegate void MessageReceivedEventHandler(string message);
        public delegate void MessageSentEventHandler(string message);
        
        public abstract event MessageReceivedEventHandler MessageReceived;
        public abstract event MessageSentEventHandler MessageSent;

        public abstract void Receive();
        public abstract void Send(string message);
        public abstract void Close();
    }

}