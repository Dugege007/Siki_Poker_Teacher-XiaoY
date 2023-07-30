
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }

    /// <summary>
    /// 0 表示人数不足，1 表示成功，2 表示有玩家未准备，3 表示房间为空
    /// </summary>
    public int result;
}
