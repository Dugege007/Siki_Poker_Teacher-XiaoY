
#nullable disable
public class MsgHandler
{
    #region Heartbeat
    /// <summary>
    /// 处理客户端发送的 Ping 消息
    /// </summary>
    /// <param name="c">发送 Ping 消息的客户端状态</param>
    /// <param name="msgBase">接收到的消息</param>
    public static void MsgPing(ClientState c, MsgBase msgBase)
    {
        // 更新客户端的最后 Ping 时间为当前时间
        c.lastPingTime = NetManager.GetTimeStamp();
        Console.WriteLine("MsgPing " + c.lastPingTime);

        // 在收到客户端的 Ping 消息后，服务器需要向客户端发送 Pong 消息以响应
        MsgPong msgPong = new MsgPong();
        // 发送 Pong 消息给客户端
        NetManager.Send(c, msgPong);
    }
    #endregion

    #region Register
    /// <summary>
    /// 处理客户端发送的注册请求
    /// </summary>
    /// <param name="c">客户端状态对象，包含了客户端的连接信息</param>
    /// <param name="msgBase">客户端发送的消息对象，包含了注册信息</param>
    public static void MsgRegister(ClientState c, MsgBase msgBase)
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
    #endregion

    #region Login
    /// <summary>
    /// 处理客户端发送的登录请求
    /// </summary>
    /// <param name="c">客户端状态对象，包含了客户端的连接信息</param>
    /// <param name="msgBase">客户端发送的消息对象，包含了登录信息</param>
    public static void MsgLogin(ClientState c, MsgBase msgBase)
    {
        // 将消息对象转换为登录消息对象
        MsgLogin msg = msgBase as MsgLogin;

        // 在数据库中验证用户的密码，如果验证失败，返回登录失败的消息给客户端
        if (!DBManager.CheckPassword(msg.id, msg.pw))
        {
            msg.result = false;
            NetManager.Send(c, msg);
            return;
        }

        // 检查用户是否已经登录，如果已经登录，返回登录失败的消息给客户端
        if (c.player != null)
        {
            msg.result = false;
            NetManager.Send(c, msg);
            return;
        }

        // 检查用户是否在其他地方在线，如果在线，向其他地方的客户端发送踢下线的消息，并关闭连接
        if (PlayerManager.IsOnLine(msg.id))
        {
            Player otherPlayer = PlayerManager.GetPlayer(msg.id);
            MsgKick msgKick = new MsgKick();
            msgKick.isKick = true;
            otherPlayer.Send(msgKick);
            NetManager.Close(otherPlayer.state);
        }

        // 从数据库中获取用户的玩家数据，如果数据为空，返回登录失败的消息给客户端
        PlayerData playerData = DBManager.GetPlayerData(msg.id);
        if (playerData == null)
        {
            msg.result = false;
            NetManager.Send(c, msg);
            return;
        }

        // 创建新的玩家对象，并将其添加到在线玩家管理器中
        Player player = new Player(c);
        player.id = msg.id;
        player.data = playerData;
        PlayerManager.AddPlayer(msg.id, player);
        c.player = player;

        // 返回登录成功的消息给客户端
        msg.result = true;
        player.Send(msg);
    }
    #endregion

    #region Room
    /// <summary>
    /// 处理获取成就的消息
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgGetAchieve(ClientState c, MsgBase msgBase)
    {
        // 获取成就的消息
        MsgGetAchieve msg = msgBase as MsgGetAchieve;
        // 获取客户端的玩家
        Player player = c.player;

        // 如果玩家不存在，则直接返回
        if (player == null)
            return;

        // 将玩家的豆子数量赋值给消息的豆子字段
        msg.bean = player.data.bean;
        // 向客户端发送消息
        player.Send(msg);
    }

    /// <summary>
    /// 处理获取房间列表的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgGetRoomList(ClientState c, MsgBase msgBase)
    {
        // 将消息转换为获取房间列表的消息
        MsgGetRoomList msg = msgBase as MsgGetRoomList;
        // 获取客户端的玩家信息
        Player player = c.player;

        // 如果玩家信息为空，则返回
        if (player == null)
            return;

        // 将房间列表信息转换为消息并发送给玩家
        player.Send(RoomManager.ToMsg());
    }
    #endregion
}
