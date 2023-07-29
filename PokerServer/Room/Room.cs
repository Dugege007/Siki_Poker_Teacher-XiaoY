
public class Room
{
    /// <summary>
    /// 房间 ID
    /// </summary>
    public int id = 0;

    /// <summary>
    /// 房间内的最大玩家数量
    /// </summary>
    public int maxPlayer = 3;

    /// <summary>
    /// 存储房间内玩家的列表
    /// </summary>
    public List<string> playerList = new List<string>();

    /// <summary>
    /// 房主 ID
    /// </summary>
    public string hostID = "";

    /// <summary>
    /// 房间状态枚举
    /// 表示房间是否已经开始游戏
    /// </summary>
    public enum Status
    {
        Prepare,
        Start,
    }

    /// <summary>
    /// 房间当前的状态
    /// </summary>
    public Status status = Status.Prepare;

    /// <summary>
    /// 添加玩家到房间的方法
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <returns>添加是否成功</returns>
    public bool AddPlayer(string id)
    {
        // 从玩家管理器中，通过 ID 获取玩家对象
        Player player = PlayerManager.GetPlayer(id);

        // 检查
        if (player == null)
        {
            Console.WriteLine("Room.AddPlayer() 错误，玩家为空");
            return false;
        }
        if (playerList.Count >= maxPlayer)
        {
            Console.WriteLine("Room.AddPlayer() 错误，已达到最大玩家数");
            return false;
        }
        if (status == Status.Start)
        {
            Console.WriteLine("Room.AddPlayer() 错误，已经开始游戏");
            return false;
        }
        if (playerList.Contains(id))
        {
            Console.WriteLine("Room.AddPlayer() 错误，玩家已在房间中");
            return false;
        }

        // 将玩家添加到房间
        playerList.Add(id);
        player.roomID = this.id;
        // 如果房间没有房主，将这个玩家设置为房主
        if (hostID == "")
        {
            hostID = player.id;
            player.isHost = true;
        }

        Broadcast(ToMsg());
        return true;
    }

    /// <summary>
    /// 从房间中移除玩家的方法
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <returns>移除是否成功</returns>
    public bool RemovePlayer(string id)
    {
        // 从玩家管理器中，通过 ID 获取玩家对象
        Player player = PlayerManager.GetPlayer(id);

        // 检查
        if (player == null)
        {
            Console.WriteLine("Room.RemovePlayer() 错误，玩家为空");
            return false;
        }
        if (!playerList.Contains(id))
        {
            Console.WriteLine("Room.RemovePlayer() 错误，玩家不在房间中");
            return false;
        }

        // 从房间中移除玩家
        playerList.Remove(id);
        player.roomID = -1;

        // 如果被移除的玩家是房主，需要选择新的房主
        if (player.isHost)
        {
            player.isHost = false;

            // 选择列表中的第一个玩家作为新的房主
            foreach (string playerID in playerList)
            {
                hostID = playerID;
                Player newHost = PlayerManager.GetPlayer(playerID);
                newHost.isHost = true;
                break;
            }
        }

        // 如果房间中没有玩家，清空房主 ID，删除房间
        if (playerList.Count == 0)
        {
            hostID = "";
            RoomManager.RemoveRoom(this.id);
        }

        Broadcast(ToMsg());
        return true;
    }

    /// <summary>
    /// 广播
    /// </summary>
    /// <param name="msgBase"></param>
    public void Broadcast(MsgBase msgBase)
    {
        foreach (string id in playerList)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msgBase);
        }
    }

    /// <summary>
    /// 将当前房间的信息转换为消息对象，以便发送给客户端
    /// </summary>
    /// <returns>包含房间信息的消息对象</returns>
    public MsgBase ToMsg()
    {
        // 创建一个新的获取房间信息的消息对象
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        // 获取房间内玩家的数量
        int count = playerList.Count;
        // 初始化消息中的玩家信息数组
        msg.players = new PlayerInfo[count];

        // 遍历房间内的每个玩家
        int i = 0;
        foreach (string id in playerList)
        {
            // 获取玩家对象
            Player player = PlayerManager.GetPlayer(id);

            // 创建一个新的玩家信息对象，并填充信息
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.id = player.id;
            playerInfo.bean = player.data.bean;
            playerInfo.isPrepare = player.isPrepare;
            playerInfo.isHost = player.isHost;

            // 将玩家信息添加到消息中
            msg.players[i] = playerInfo;
            i++;
        }

        // 返回填充了房间信息的消息对象
        return msg;
    }
}
