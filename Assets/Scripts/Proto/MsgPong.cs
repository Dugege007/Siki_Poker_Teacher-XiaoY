using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 心跳机制
// Pong 服务端 -> 客户端
public class MsgPong : MsgBase
{
    public MsgPong()
    {
        protoName = "MsgPong";
    }
}
