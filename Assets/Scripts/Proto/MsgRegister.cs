
public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        protoName = "MsgRegister";
    }

    public string id = "";
    public string pw = "";
    public bool result = true;
}
