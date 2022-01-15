using System;

using FancyLibrary.Utils;


namespace FancyLibrary.Bridges {

    public enum DatagramType {
        Heartbeat = 0, // heartbeat
        Replay = 1, // replay heartbeat
        Message = 2, // normal message
        Package = 3, // structures usually  
    }

    public struct DatagramStruct {
        public DatagramType Type;
        public int Sid; // serial id
        public int Port;
        public byte[] Content;

        public override string ToString() => $"{Type}, {Port}, {BitConverter.ToString(Content)}";

        public byte[] GetBytes() => Converter.GetBytes(this, Converter.ConvertMethod.Json);

        public static bool FromBytes(byte[] bytes, out DatagramStruct ds) => 
            Converter.FromBytes(bytes, out ds, Converter.ConvertMethod.Json);

    }

}