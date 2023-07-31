
public class MsgGetCardList : MsgBase
{
    public MsgGetCardList()
    {
        protoName = "MsgGetCardList";
    }

    public CardInfo[] cardInfos = new CardInfo[17];
}
