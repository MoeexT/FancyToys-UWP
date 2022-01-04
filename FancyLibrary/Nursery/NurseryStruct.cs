namespace FancyLibrary.Nursery {

    public enum NurseryType {   //    功能        信息流方向        SDU
        Config = 1,            // Nursery 设置        ↓      SettingStruct
        Operation = 2,          // 进程开启/关闭       ↓↑      OperationStruct
        Information = 3,        // 进程的具体消息       ↑      InformationStruct
    }

    public struct NurseryStruct {
        //public uint Seq;          // 序列号
        public NurseryType Type;    // 消息类型
        public byte[] Content;      // 消息内容
    }
}