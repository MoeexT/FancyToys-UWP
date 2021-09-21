namespace FancyLibrary.Message {

    public enum MessageType {
        Action = 1, // 指令               ↑↓ 
        Setting = 2, // 设置               ↓
        Logging = 3, // 日志、错误消息      ↑
        Nursery = 4 // Nursery消息        ↑↓ 
    }

    public struct MessageStruct {
        public MessageType Type; // message type
        public string Content; // message content
    }

}