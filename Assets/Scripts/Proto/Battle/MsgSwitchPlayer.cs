
public class MsgSwitchPlayer : MsgBase
{
    public MsgSwitchPlayer()
    {
        protoName = "MsgSwitchPlayer";
    }

    public string id = "";
    public int round = 1;
}
