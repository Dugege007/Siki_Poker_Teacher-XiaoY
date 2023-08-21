
public class MsgPlayCards : MsgBase
{
    public MsgPlayCards()
    {
        protoName = "MsgPlayCards";
    }

    public string id = "";
    public bool play;
    public CardInfo[] cardsInfo = new CardInfo[20];
    public int cardType;
    public bool canPressNotPlayBtn = true;

    /// <summary>
    /// 0 继续游戏；
    /// 1 农民胜利；
    /// 2 地主胜利
    /// </summary>
    public int win;

    // 是否处理完成
    public bool result;
}
