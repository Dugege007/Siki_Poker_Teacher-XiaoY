
public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        protoName = "MsgLogin";
    }

    public string id = "";
    public string pw = "";
    public bool result = true;
}
