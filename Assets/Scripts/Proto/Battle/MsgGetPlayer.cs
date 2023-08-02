
public class MsgGetPlayer : MsgBase
{
    public MsgGetPlayer()
    {
        protoName = "MsgGetPlayer";
    }

    public string id = "";
    public string leftID = "";
    public string rightID = "";
}
