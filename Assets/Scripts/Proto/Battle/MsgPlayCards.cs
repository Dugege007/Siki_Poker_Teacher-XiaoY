
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
}
