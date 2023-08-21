
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

    /// <summary>
    /// 处理创建房间的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgCreateRoom(ClientState c, MsgBase msgBase)
    {
        // 将消息转换为创建房间的消息
        MsgCreateRoom msg = msgBase as MsgCreateRoom;
        // 获取客户端的玩家信息
        Player player = c.player;

        // 如果玩家信息为空或玩家已在房间中，则返回
        if (player == null) return;

        if (player.roomID >= 0)
        {
            msg.result = false;
            player.Send(msg);
            return;
        }

        // 创建房间并添加玩家
        Room room = RoomManager.AddRoom();
        room.AddPlayer(player.id);

        // 发送创建成功的消息给客户端
        msg.result = true;
        player.Send(msg);
    }

    /// <summary>
    /// 处理进入房间的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
    {
        // 将消息转换为进入房间的消息
        MsgEnterRoom msg = msgBase as MsgEnterRoom;
        // 获取客户端的玩家信息
        Player player = c.player;

        // 如果玩家信息为空或玩家已在房间中，则返回
        if (player == null) return;

        if (player.roomID >= 0)
        {
            msg.result = false;
            player.Send(msg);
            return;
        }

        // 获取房间信息
        Room room = RoomManager.GetRoom(msg.roomID);

        // 如果房间不存在，发送失败消息给玩家
        if (room == null)
        {
            msg.result = false;
            player.Send(msg);
            return;
        }
        // 如果添加失败，发送失败消息给玩家
        if (!room.AddPlayer(player.id))
        {
            msg.result = false;
            player.Send(msg);
            return;
        }

        // 发送进入成功的消息给玩家
        msg.result = true;
        player.Send(msg);
    }

    /// <summary>
    /// 处理获取房间信息的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase)
    {
        // 将消息转换为获取房间信息的消息
        MsgGetRoomInfo msg = msgBase as MsgGetRoomInfo;
        // 获取客户端的玩家信息
        Player player = c.player;

        // 如果玩家信息为空，则返回
        if (player == null) return;

        // 获取玩家所在的房间信息
        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null)
        {
            // 如果房间不存在，发送消息给客户端
            player.Send(msg);
            return;
        }

        // 发送房间信息给客户端
        player.Send(room.ToMsg());
    }

    /// <summary>
    /// 处理离开房间的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgLeaveRoom(ClientState c, MsgBase msgBase)
    {
        // 将消息转换为离开房间的消息
        MsgLeaveRoom msg = msgBase as MsgLeaveRoom;
        // 获取客户端的玩家信息
        Player player = c.player;

        // 如果玩家信息为空，则返回
        if (player == null) return;

        // 获取玩家所在的房间信息
        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null)
        {
            // 如果房间不存在，发送失败消息给客户端
            msg.result = false;
            player.Send(msg);
            return;
        }

        // 从房间中移除玩家
        room.RemovePlayer(player.id);
        // 发送离开成功的消息给客户端
        msg.result = true;
        player.Send(msg);
    }

    /// <summary>
    /// 处理玩家准备的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgPrepare(ClientState c, MsgBase msgBase)
    {
        MsgPrepare msg = msgBase as MsgPrepare;
        Player player = c.player;

        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null)
        {
            msg.isPrepare = false;
            player.Send(msg);
            return;
        }

        msg.isPrepare = room.Prepare(player.id);
        player.Send(msg);
    }

    /// <summary>
    /// 处理玩家开始游戏的消息请求
    /// </summary>
    /// <param name="c">客户端状态，包含了客户端的连接信息和玩家信息</param>
    /// <param name="msgBase">客户端发送的消息</param>
    public static void MsgStartBattle(ClientState c, MsgBase msgBase)
    {
        MsgStartBattle msg = msgBase as MsgStartBattle;
        Player player = c.player;

        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null)
        {
            msg.result = 3;
            player.Send(msg);
            return;
        }

        Console.WriteLine("当前房间人数：" + room.playerIDList.Count);
        if (room.playerIDList.Count < 3)
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }

        foreach (string id in room.playerIDList)
        {
            if (id == room.hostID)
                continue;

            // 如果有未准备的玩家
            if (!room.playerDict.ContainsKey(id) || !room.playerDict[id])
            {
                msg.result = 2;
                player.Send(msg);
                return;
            }
        }

        // 成功开始游戏
        msg.result = 1;
        foreach (string id in room.playerIDList)
        {
            Player p = PlayerManager.GetPlayer(id);
            p.Send(msg);
        }
        room.Start();
    }
    #endregion

    #region Battle
    /// <summary>
    /// 获取玩家手牌列表
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgGetCardList(ClientState c, MsgBase msgBase)
    {
        // 将消息基类转换为获取卡牌列表的消息
        MsgGetCardList msg = msgBase as MsgGetCardList;
        // 从客户端状态中获取玩家对象
        Player player = c.player;

        // 如果玩家对象为空，则直接返回
        if (player == null)
            return;

        // 从房间管理器中获取玩家所在的房间
        Room room = RoomManager.GetRoom(player.roomID);
        // 如果房间不存在，则直接返回
        if (room == null)
            return;

        // 获取玩家的手牌列表
        Card[] cards = room.playerCard[player.id].ToArray();
        // 将手牌列表转换为卡牌信息列表
        msg.cardsInfo = CardManager.GetCardInfos(cards);
        // 获取底牌列表
        Card[] threeCards = room.playerCard[""].ToArray();
        // 将底牌列表转换为卡牌信息列表
        msg.threeCardsInfo = CardManager.GetCardInfos(threeCards);
        // 将卡牌信息列表发送给玩家
        player.Send(msg);
    }

    /// <summary>
    /// 处理获取开始玩家的消息
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">接收到的消息基类</param>
    public static void MsgGetStartPlayer(ClientState c, MsgBase msgBase)
    {
        MsgGetStartPlayer msg = msgBase as MsgGetStartPlayer;

        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        // 设置消息中的开始玩家 ID 为当前房间的当前玩家 ID
        msg.id = room.currentPlayerID;

        // 遍历房间中的所有玩家 ID
        foreach (string id in room.playerIDList)
        {
            // 从玩家管理器中获取玩家对象，并向该玩家发送消息
            PlayerManager.GetPlayer(id).Send(msg);
        }
    }

    /// <summary>
    /// 表示轮换玩家
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgSwitchPlayer(ClientState c, MsgBase msgBase)
    {
        // 将消息基类转换为获取开始玩家的消息类型
        MsgSwitchPlayer msg = msgBase as MsgSwitchPlayer;
        // 从客户端状态中获取玩家对象
        Player player = c.player;
        // 如果玩家对象为空，则直接返回
        if (player == null) return;

        // 从房间管理器中获取玩家所在的房间
        Room room = RoomManager.GetRoom(player.roomID);
        // 如果房间对象为空，则直接返回
        if (room == null) return;

        room.Index += msg.round;

        room.currentPlayerID = room.playerIDList[room.Index];
        msg.id = room.currentPlayerID;

        room.Send(msg);
    }

    /// <summary>
    /// 获取开始的玩家
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgGetPlayer(ClientState c, MsgBase msgBase)
    {
        MsgGetPlayer msg = msgBase as MsgGetPlayer;

        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        msg.id = player.id;
        for (int i = 0; i < room.playerIDList.Count; i++)
        {
            if (room.playerIDList[i] == msg.id)
            {
                msg.leftID = room.playerIDList[i - 1 < 0 ? 2 : i - 1];
                msg.rightID = room.playerIDList[i + 1 >= room.maxPlayer ? 0 : i + 1];
            }
        }
        player.Send(msg);
    }

    /// <summary>
    /// 处理叫地主逻辑
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgCall(ClientState c, MsgBase msgBase)
    {
        MsgCall msg = msgBase as MsgCall;
        Player player = c.player;
        if (player == null) return;

        msg.id = player.id;
        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        if (msg.call)
        {
            room.callID = player.id;
            room.landLordRank[player.id] += 2;
            if (room.CheckCall())
                msg.result = 3;
            else
                msg.result = 1;
        }
        else
        {
            room.landLordRank[player.id] += 1;
            if (room.CheckAllNotCall())
                msg.result = 2;
            else
                msg.result = 0;
        }

        room.Send(msg);
        return;
    }

    /// <summary>
    /// 都没有叫地主，重新开始
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgReStart(ClientState c, MsgBase msgBase)
    {
        MsgReStart msg = msgBase as MsgReStart;
        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        CardManager.Shuffle();
        room.cards = CardManager.cards;

        room.Start();
        //Console.WriteLine(room.cards.Count);
        //Console.WriteLine(room.playerCard["1"].Count);
        room.Send(msg);
    }

    /// <summary>
    /// 开始抢地主
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgStartRob(ClientState c, MsgBase msgBase)
    {
        MsgStartRob msg = msgBase as MsgStartRob;
        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        room.Send(msg);
    }

    /// <summary>
    /// 抢地主
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgRob(ClientState c, MsgBase msgBase)
    {
        MsgRob msg = msgBase as MsgRob;
        Player player = c.player;
        if (player == null) return;

        msg.id = player.id;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        if (msg.isRob)
        {
            room.landLordRank[player.id] += room.robRank++;
        }
        else // 不抢
        {
            room.landLordRank[player.id]++;
            if (room.CheckCall())
            {
                msg.landLordID = room.callID;
            }
        }

        if (player.id == room.callID)
        {
            // 检测谁是地主
            msg.landLordID = room.CheckLandLord();
        }

        if (room.landLordRank[room.playerIDList[room.Index + 1 >= 3 ? 0 : room.Index + 1]] == 0)
        {
            msg.needRob = false;
        }
        else
        {
            msg.needRob = true;
        }

        room.Send(msg);
    }

    /// <summary>
    /// 出牌
    /// </summary>
    /// <param name="c">客户端状态</param>
    /// <param name="msgBase">消息</param>
    public static void MsgPlayCards(ClientState c, MsgBase msgBase)
    {
        MsgPlayCards msg = msgBase as MsgPlayCards;
        Player player = c.player;
        if (player == null) return;

        msg.id = player.id;

        Room room = RoomManager.GetRoom(player.roomID);
        if (room == null) return;

        Card[] cards = CardManager.GetCards(msg.cardsInfo);
        // 判断出牌
        if (msg.play)
        {
            msg.cardType = (int)CardManager.GetCardType(cards);
            // 如果前两家出牌了，和他们出的牌进行比较
            if (room.prePlay || room.prePrePlay)
            {
                msg.result = CardManager.Compare(room.preCard.ToArray(), cards);
            }
            else // 前两家要不起，自己开始出
            {
                msg.result = CardManager.GetCardType(cards) != CardManager.CardType.Wrong;
            }

            // 出牌成功
            if (msg.result)
            {
                // 删除卡牌
                room.DeletCards(cards, msg.id);
                // 判断输赢

                room.preCard = cards.ToList();
                // 上家和上上家往前推
                room.prePrePlay = room.prePlay;
                // 标记已出牌
                room.prePlay = true;
            }

            room.Send(msg);
            return;
        }
        else // 不出牌
        {
            room.prePrePlay = room.prePlay;
            room.prePlay = false;

            if (!room.prePrePlay)
                msg.canPressNotPlayBtn = false;

            msg.result = true;
            room.Send(msg);
            return;
        }
    }
    #endregion
}
