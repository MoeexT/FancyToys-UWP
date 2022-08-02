#nullable enable
namespace FancyLibrary.Bridges {

    public abstract class Bridge {

        public abstract void Receive();

        // public abstract void Send(byte[] bytes);

        public abstract void Close();
    }

}
