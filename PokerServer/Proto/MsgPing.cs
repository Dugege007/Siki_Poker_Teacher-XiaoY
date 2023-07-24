using System.Collections;
using System.Collections.Generic;

// 心跳机制
// Ping 客户端 -> 服务端
public class MsgPing : MsgBase
{
    public MsgPing()
    {
        protoName = "MsgPing";
    }
}
