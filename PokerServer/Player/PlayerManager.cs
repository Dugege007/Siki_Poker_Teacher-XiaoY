#nullable disable
public class PlayerManager
{
    /// <summary>
    /// 在线玩家字典
    /// </summary>
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <returns></returns>
    public static bool IsOnLine(string id)
    {
        return players.ContainsKey(id);
    }

    /// <summary>
    /// 根据 ID 获取对应的玩家
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <returns></returns>
    public static Player GetPlayer(string id)
    {
        if (players.ContainsKey(id))
        {
            return players[id];
        }
        else
        {
            Console.WriteLine("未获取到玩家");
            return null;
        }
    }

    /// <summary>
    /// 增添玩家
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <param name="player">玩家</param>
    public static void AddPlayer(string id, Player player)
    {
        players.Add(id, player);
    }

    /// <summary>
    /// 根据 ID 删除玩家
    /// </summary>
    /// <param name="id">玩家 ID</param>
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }
}
