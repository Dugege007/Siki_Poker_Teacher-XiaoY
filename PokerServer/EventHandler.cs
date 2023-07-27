public class EventHandler
{
    /// <summary>
    /// 处理客户端断开连接的事件
    /// </summary>
    /// <param name="c">断开连接的客户端状态</param>
    public static void OnDisconnect(ClientState c)
    {
        if (c.player != null)
        {
            // 更新玩家数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            // 移除玩家
            PlayerManager.RemovePlayer(c.player.id);
        }
    }

    /// <summary>
    /// 处理定时任务的事件
    /// </summary>
    public static void OnTimer()
    {
        CheckPing();
    }

    /// <summary>
    /// 检查客户端的 Ping 是否超时
    /// </summary>
    public static void CheckPing()
    {
        // 遍历所有客户端，检查是否有 Ping 超时的情况
        foreach (ClientState c in NetManager.clients.Values)
        {
            // 如果客户端的最后一次 Ping 时间距离现在超过了4个心跳间隔，认为该客户端已经断开连接
            if (NetManager.GetTimeStamp() - c.lastPingTime > NetManager.pingInterval * 4)
            {
                Console.WriteLine(NetManager.GetTimeStamp());
                Console.WriteLine(c.lastPingTime);
                Console.WriteLine(NetManager.pingInterval);

                Console.WriteLine("心跳机制，断开连接");
                // 关闭与该客户端的连接
                NetManager.Close(c);
                return;
            }
        }
    }
}
