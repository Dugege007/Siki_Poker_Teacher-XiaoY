
#nullable disable
public class RoomManager
{
    /// <summary>
    /// 最大房间 ID
    /// </summary>
    private static int maxID = 1;

    /// <summary>
    /// 房间字典
    /// </summary>
    public static Dictionary<int, Room> roomsDict = new Dictionary<int, Room>();

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="id">房间 ID</param>
    /// <returns>获取的房间对象</returns>
    public static Room GetRoom(int id)
    {
        // 先检查 ID 是否存在，避免抛出异常
        if (roomsDict.ContainsKey(id))
            return roomsDict[id];
        else
            return null;
    }

    /// <summary>
    /// 添加房间
    /// </summary>
    /// <returns>添加的房间对象</returns>
    public static Room AddRoom()
    {
        Room room = new Room();
        room.id = maxID;
        roomsDict.Add(room.id, room);
        maxID++;

        return room;
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    /// <param name="id">房间 ID</param>
    public static void RemoveRoom(int id)
    {
        roomsDict.Remove(id);
    }

    // 由于字典转成 Json 后，无法再有 Json 转回字典
    /// <summary>
    /// 将房间信息转换为消息
    /// </summary>
    /// <returns>包含房间信息的消息</returns>
    public static MsgBase ToMsg()
    {
        // 创建一个获取房间列表的消息
        MsgGetRoomList msg = new MsgGetRoomList();
        // 获取房间的数量
        int count = roomsDict.Count;
        // 初始化消息中的房间信息数组
        msg.rooms = new RoomInfo[count];

        // 遍历所有房间
        int i = 0;
        foreach (Room room in roomsDict.Values)
        {
            // 创建一个新的房间信息对象
            RoomInfo roomInfo = new RoomInfo();
            // 设置房间信息的 ID 和玩家数量
            roomInfo.id = room.id;
            roomInfo.count = room.playerIDList.Count;
            // 根据房间的状态设置房间信息的准备状态
            if (room.status == Room.Status.Prepare)
                roomInfo.isPrepare = true;
            else
                roomInfo.isPrepare = false;

            // 将房间信息添加到消息的房间信息数组中
            msg.rooms[i] = roomInfo;
            i++;
        }

        return msg;
    }
}
