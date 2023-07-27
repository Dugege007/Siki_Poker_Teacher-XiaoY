#nullable disable
public class MsgHandler
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
        NetManager.Send(c, msgPong);
    }



    /// <summary>
    /// 处理客户端发送的注册请求
    /// </summary>
    /// <param name="c">客户端状态对象，包含了客户端的连接信息</param>
    /// <param name="msgBase">客户端发送的消息对象，包含了注册信息</param>
    public static void MsgRegister(ClientState c,MsgBase msgBase)
    {
        // 将消息对象转换为注册消息对象
        MsgRegister msg = msgBase as MsgRegister;

        // 在数据库中注册新用户，如果注册成功，创建新的玩家数据
        if (DBManager.Register(msg.id, msg.pw))
        {
            DBManager.CreatePlayer(msg.id);
            msg.result = true;
        }
        else
        {
            msg.result = false;
        }

        // 将注册结果发送回客户端
        NetManager.Send(c, msg);
    }
}
