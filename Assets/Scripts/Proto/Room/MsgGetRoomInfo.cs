
public class MsgGetRoomInfo:MsgBase
{
    public MsgGetRoomInfo()
    {
        protoName = "MsgGetRoomInfo";
    }

    public PlayerInfo[] players;
}
