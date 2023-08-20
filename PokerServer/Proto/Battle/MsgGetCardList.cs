
public class MsgGetCardList : MsgBase
{
    public MsgGetCardList()
    {
        protoName = "MsgGetCardList";
    }

    public CardInfo[] cardsInfo = new CardInfo[17];
    public CardInfo[] threeCardsInfo = new CardInfo[3];
}
