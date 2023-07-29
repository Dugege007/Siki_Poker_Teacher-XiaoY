
#nullable disable
public class RoomManager
{
    /// <summary>
    /// 最大房间 ID
    /// </summary>
    private static int maxID = 1;

    /// <summary>
    /// 房间列表
    /// </summary>
    public static Dictionary<int,Room> rooms = new Dictionary<int,Room>();

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="id">房间 ID</param>
    /// <returns>获取的房间对象</returns>
    public static Room GetRoom(int id)
    {
        // 先检查 ID 是否存在，避免抛出异常
        if (rooms.ContainsKey(id))
            return rooms[id];
        else
            return null;
    }

    /// <summary>
    /// 添加房间
    /// </summary>
    /// <returns>添加的房间对象</returns>
    public static Room AddRoom()
    {
        maxID++;
        Room room = new Room();
        room.id = maxID;
        rooms.Add(room.id, room);
        return room;
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    /// <param name="id">房间 ID</param>
    public static void RemoveRoom(int id)
    {
        rooms.Remove(id);
    }
}
