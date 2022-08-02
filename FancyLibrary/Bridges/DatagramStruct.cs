using System;

using FancyLibrary.Utils;


namespace FancyLibrary.Bridges {

    public enum RequestMethod {
        Notify,
        Request,
        Response,
    }

    // public enum DatagramType {
    //     Heartbeat = 0, // heartbeat
    //     Message = 1, // normal message
    //     Package = 2, // structures usually
    // }

    public struct DatagramStruct {
        public RequestMethod Method;
        public byte StructType;
        public ulong Seq; // serial id
        public ulong Ack; // acknowledge serial id
        public byte[] Content;

        public override string ToString() => $"{{ {Method}, {StructType}, {BitConverter.ToString(Content)} }}";

        public byte[] GetBytes() => Converter.GetBytes(this);

        public static bool FromBytes(byte[] bytes, out DatagramStruct ds) => Converter.FromBytes(bytes, out ds);

    }
}
