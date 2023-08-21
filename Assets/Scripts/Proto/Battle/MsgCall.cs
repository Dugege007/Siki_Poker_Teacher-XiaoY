
public class MsgCall : MsgBase
{
    public MsgCall()
    {
        protoName = "MsgCall";
    }

    public string id = "";
    public bool call;

    /// <summary>
    /// 0 表示下位玩家继续叫地主；
    /// 1 表示下位玩家抢地主；
    /// 2 表示重新洗牌；
    /// 3 表示不需要抢地主
    /// </summary>
    public int result;
}
