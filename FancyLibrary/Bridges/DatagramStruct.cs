﻿namespace FancyLibrary.Bridges {

    internal enum DatagramType {
        Heartbeat = 0,  // heartbeat
        Message = 1,  // normal message
    }
    
    internal struct DatagramStruct {
        public DatagramType Type;
        public int Sid;  // serial id
        public byte[] Content;
    }

}