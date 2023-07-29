
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }

    public int roomID;
    public bool result;
}
