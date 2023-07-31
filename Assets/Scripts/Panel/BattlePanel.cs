
public class BattlePanel : BasePanel
{
    public override void OnInit()
    {
        skinPath = "BattlePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        // ¼àÌıÍøÂçÍøÂçÊÂ¼ş
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);

        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetCardList", OnMsgGetCardList);
    }

    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        for (int i = 0; i < 17; i++)
        {
            Card card = new Card(msg.cardInfos[i].suit, msg.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }
    }
}
