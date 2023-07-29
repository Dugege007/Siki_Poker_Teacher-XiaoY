﻿
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
                break;
            }

            // 如果房间中没有玩家，清空房主 ID
            if (playerList.Count == 0)
            {
                hostID = "";
                //TODO 移除该房间
            }
        }

        return true;
    }
}