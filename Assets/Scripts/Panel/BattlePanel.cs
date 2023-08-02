using System;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    /// <summary>
    /// 玩家游戏物体
    /// </summary>
    private GameObject playerObj;

    /// <summary>
    /// 叫地主 按钮
    /// </summary>
    private Button callBtn;

    /// <summary>
    /// 不叫 按钮
    /// </summary>
    private Button notCallBtn;

    /// <summary>
    /// 抢地主 按钮
    /// </summary>
    private Button robBtn;

    /// <summary>
    /// 不抢 按钮
    /// </summary>
    private Button notRobBtn;

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
        callBtn = skin.transform.Find("CallButtonList/CallBtn").GetComponent<Button>();
        notCallBtn = skin.transform.Find("CallButtonList/NotCallBtn").GetComponent<Button>();
        robBtn = skin.transform.Find("RobButtonList/RobBtn").GetComponent<Button>();
        notRobBtn = skin.transform.Find("RobButtonList/NotRobBtn").GetComponent<Button>();

        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);

        // 监听网络网络事件
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);

        // 监听按钮事件
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);

        // 发送消息
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
        MsgGetStartPlayer msgGetStartPlayer = new MsgGetStartPlayer();
        NetManager.Send(msgGetStartPlayer);
    }

    // 当面板关闭时执行的操作
    public override void OnClose()
    {
        // 移除网络事件监听
        NetManager.RemoveMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.RemoveMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.RemoveMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
    }

    // 向服务器发送获取卡牌列表消息
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

    // 向服务器发送获取第一个开始的玩家的消息
    public void OnMsgGetStartPlayer(MsgBase msgBase)
    {
        MsgGetStartPlayer msg = msgBase as MsgGetStartPlayer;

        if (GameManager.id == msg.id)
        {
            callBtn.gameObject.SetActive(true);
            notCallBtn.gameObject.SetActive(true);
        }
    }

    private void OnCallBtnClick()
    {
        MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();
        NetManager.Send(msgSwitchPlayer);

    }

    private void OnNotCallBtnClick()
    {

    }

    private void OnRobBtnClick()
    {

    }

    private void OnNotRobBtnClick()
    {

    }

    public void OnMsgSwitchPlayer(MsgBase msgBase)
    {
        MsgSwitchPlayer msg = msgBase as MsgSwitchPlayer;
        switch (GameManager.status)
        {
            case PlayerStatus.Call:
                if (msg.id == GameManager.id)
                {
                    callBtn.gameObject.SetActive(true);
                    notCallBtn.gameObject.SetActive(true);
                }
                else
                {
                    callBtn.gameObject.SetActive(false);
                    notCallBtn.gameObject.SetActive(false);
                }
                break;
            case PlayerStatus.Rob:
                break;
            case PlayerStatus.Play:
                break;
            default:
                break;
        }
    }
}
