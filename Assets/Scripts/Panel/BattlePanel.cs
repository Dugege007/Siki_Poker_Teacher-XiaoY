using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    // 定义玩家游戏物体
    private GameObject playerObj;

    // 初始化面板
    public override void OnInit()
    {
        // 设置面板路径
        skinPath = "Panel/BattlePanel";
        // 设置面板层级
        layer = PanelManager.Layer.Panel;
    }

    // 当面板显示时执行的操作
    public override void OnShow(params object[] para)
    {
        // 获取组件
        playerObj = skin.transform.Find("PlayerImage").gameObject;

        // 监听网络网络事件
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);

        // 发送获取卡牌列表的消息
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    // 当面板关闭时执行的操作
    public override void OnClose()
    {
        // 移除网络事件监听
        NetManager.RemoveMsgListener("MsgGetCardList", OnMsgGetCardList);
    }

    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        // 将收到的卡牌添加到游戏管理器的卡牌列表中
        for (int i = 0; i < 17; i++)
        {
            Card card = new Card(msg.cardInfos[i].suit, msg.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        // 生成卡牌
        GenerateCard(GameManager.cards.ToArray());
    }

    // 生成卡牌的方法
    public void GenerateCard(Card[] cards)
    {
        // 遍历卡牌数组，为每张卡牌创建一个游戏物体，并设置其属性
        for (int i = 0; i < cards.Length; i++)
        {
            // 获取卡牌名称
            string name = CardManager.GetName(cards[i]);
            // 创建卡牌游戏物体
            GameObject cardObj = new GameObject(name);
            // 设置卡牌游戏物体的父物体为玩家游戏物体
            cardObj.transform.SetParent(playerObj.transform.Find("Cards"), false);
            // 为卡牌物体添加 Image 组件
            Image image = cardObj.AddComponent<Image>();
            // 根据名称获取卡牌图片
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            // 设置卡牌图片
            image.sprite = sprite;
            // 设置卡牌图片的大小
            image.SetNativeSize();
            image.rectTransform.localScale = Vector3.one * 1.75f;
            // 设置卡牌图片的层级
            cardObj.layer = LayerMask.NameToLayer("UI");
        }
    }
}
