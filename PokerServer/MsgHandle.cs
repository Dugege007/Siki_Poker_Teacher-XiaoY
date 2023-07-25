using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgHandle
{
    /// <summary>
    /// 处理客户端发送的 Ping 消息
    /// </summary>
    /// <param name="c">发送 Ping 消息的客户端状态</param>
    /// <param name="msgBase">接收到的消息</param>
    public static void MsgPing(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("MsgPing");
        // 更新客户端的最后 Ping 时间为当前时间
        c.lastPingTime = NetManager.GetTimeStamp();

        // 在收到客户端的 Ping 消息后，服务器需要向客户端发送 Pong 消息以响应
        MsgPong msgPong = new MsgPong();
        // 发送 Pong 消息给客户端
        NetManager.Send(c, msgBase);
    }
}
