
#nullable disable
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
    /// 玩家列表
    /// </summary>
    public List<string> playerIDList = new List<string>();

    /// <summary>
    /// 玩家准备状态字典
    /// 不包括房主
    /// </summary>
    public Dictionary<string, bool> playerDict = new Dictionary<string, bool>();

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
    /// 当前房间的一套牌
    /// </summary>
    public List<Card> cards;

    /// <summary>
    /// 玩家手牌
    /// </summary>
    public Dictionary<string, List<Card>> playerCard = new Dictionary<string, List<Card>>();

    /// <summary>
    /// 当前玩家 ID
    /// </summary>
    public string currentPlayerID;

    /// <summary>
    /// 当前玩家在列表中的索引
    /// </summary>
    public int index;

    /// <summary>
    /// 玩家ID
    /// 抢地主的权值
    /// </summary>
    public Dictionary<string, int> landLordRank = new Dictionary<string, int>();

    /// <summary>
    /// 叫地主的玩家
    /// </summary>
    public string callID = "";

    /// <summary>
    /// 叫地主的权值
    /// </summary>
    public int robRank = 3;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Room()
    {
        if (cards == null)
        {
            CardManager.Shuffle();
            cards = CardManager.cards;
        }
    }

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
        if (playerIDList.Count >= maxPlayer)
        {
            Console.WriteLine("Room.AddPlayer() 错误，已达到最大玩家数");
            return false;
        }
        if (status == Status.Start)
        {
            Console.WriteLine("Room.AddPlayer() 错误，已经开始游戏");
            return false;
        }
        if (playerIDList.Contains(id))
        {
            Console.WriteLine("Room.AddPlayer() 错误，玩家已在房间中");
            return false;
        }

        // 将玩家添加到房间
        playerIDList.Add(id);
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
        if (!playerIDList.Contains(id))
        {
            Console.WriteLine("Room.RemovePlayer() 错误，玩家不在房间中");
            return false;
        }

        // 从房间中移除玩家
        playerIDList.Remove(id);
        player.roomID = -1;

        // 判断是否已准备
        if (playerDict.ContainsKey(id))
        {
            playerDict.Remove(id);
            player.isPrepare = false;
        }

        // 如果被移除的玩家是房主，需要选择新的房主
        if (player.isHost)
        {
            player.isHost = false;

            // 选择列表中的第一个玩家作为新的房主
            foreach (string playerID in playerIDList)
            {
                hostID = playerID;
                Player newHost = PlayerManager.GetPlayer(playerID);
                newHost.isHost = true;
                break;
            }
        }

        // 如果房间中没有玩家，清空房主 ID，删除房间
        if (playerIDList.Count == 0)
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
        foreach (string id in playerIDList)
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
        int count = playerIDList.Count;
        // 初始化消息中的玩家信息数组
        msg.players = new PlayerInfo[count];

        // 遍历房间内的每个玩家
        int i = 0;
        foreach (string id in playerIDList)
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

    /// <summary>
    /// 玩家准备
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <returns>返回玩家是否成功准备</returns>
    public bool Prepare(string id)
    {
        // 从玩家管理器中，通过 ID 获取玩家对象
        Player player = PlayerManager.GetPlayer(id);

        // 检查玩家是否存在
        if (player == null)
        {
            Console.WriteLine("Room.RemovePlayer() 错误，玩家为空");
            return false;
        }
        // 检查玩家是否在房间中
        if (!playerIDList.Contains(id))
        {
            Console.WriteLine("Room.RemovePlayer() 错误，玩家不在房间中");
            return false;
        }

        // 更新玩家的准备状态
        if (!playerDict.ContainsKey(id))
        {
            playerDict.Add(id, true);
        }
        else
        {
            playerDict[id] = true;
        }

        // 设置玩家为已准备状态
        player.isPrepare = true;
        // 广播房间信息
        Broadcast(ToMsg());

        return true;
    }

    /// <summary>
    /// 在开始游戏的时候调用，用于分配玩家手牌
    /// </summary>
    public void Start()
    {
        // 随机选择一个玩家开始
        Random random = new Random();
        index = random.Next(maxPlayer);
        currentPlayerID = playerIDList[index];

        robRank = 3;

        playerCard.Clear();
        landLordRank.Clear();

        // 分配玩家手牌
        for (int i = 0; i < maxPlayer; i++)
        {
            List<Card> c17 = new List<Card>();
            // 每个玩家分配17张牌
            for (int j = i * 17; j < i * 17 + 17; j++)
            {
                c17.Add(cards[j]);
            }
            // 将分配的牌添加到玩家手牌列表中
            playerCard.Add(playerIDList[i], c17);
        }

        // 分配底牌
        List<Card> c3 = new List<Card>();
        for (int i = 51; i < 54; i++)
        {
            c3.Add(cards[i]);
        }
        // 用空字符串表示底牌
        playerCard.Add("", c3); // 用空字符串表示底牌

        for (int i = 0; i < playerIDList.Count; i++)
        {
            landLordRank.Add(playerIDList[i], -1);
        }
    }

    /// <summary>
    /// 向当前房间所有玩家发送消息
    /// </summary>
    /// <param name="msgBase">消息</param>
    public void Send(MsgBase msgBase)
    {
        foreach (string id in playerIDList)
        {
            PlayerManager.GetPlayer(id).Send(msgBase);
        }
    }

    /// <summary>
    /// 判断玩家是否需要叫地主
    /// </summary>
    /// <returns>如果需要叫地主，返回 true</returns>
    public bool CheckCall()
    {
        int count = 0;
        foreach (int i in landLordRank.Values)
        {
            if (i == 0)
                count++;
        }

        if (count == 2)
            return true;
        return false;
    }

    /// <summary>
    /// 判断所有玩家是否都没叫地主
    /// </summary>
    /// <returns>如果都没叫，返回 true</returns>
    public bool CheckAllNotCall()
    {
        bool result = true;

        foreach (int i in landLordRank.Values)
        {
            if (i != 0)
                result = false;
        }

        return result;
    }
}
