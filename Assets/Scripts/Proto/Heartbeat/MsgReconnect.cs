
public class MsgReconnect : MsgBase
{
    public MsgReconnect()
    {
        protoName = "MsgReconnectRequest";
    }

    public string playerID;
}
