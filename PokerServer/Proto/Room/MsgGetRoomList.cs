
public class MsgGetRoomList : MsgBase
{
    public MsgGetRoomList()
    {
        protoName = "MsgGetRoomList";
    }

    public RoomInfo[] rooms;
}
