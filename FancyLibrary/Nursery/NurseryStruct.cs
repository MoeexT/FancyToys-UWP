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
    
    public struct InformationStruct {
        public int PID;
        public string ProcessName;
        public float CPU;
        public int Memory;
    }
    
    public enum OperationType {
        Add = 1,
        Remove = 2,
        Start = 3,
        Stop = 4,
        Restart = 5,
    }

    public enum OperationCode {
        Success = 1,
        Failed = 2,
    }

    public struct OperationStruct {
        public OperationType Type;
        public OperationCode Code;
        public string PathName; // 要打开的文件
        public string Args; // 参数
        public string ProcessName; // 打开后的进程名
    }

    public struct ConfigStruct {
        
    }

}