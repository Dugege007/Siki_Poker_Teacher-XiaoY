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
    /// 左玩家 ID
    /// </summary>
    private Text leftIDText;

    /// <summary>
    /// 右玩家 ID
    /// </summary>
    private Text rightIDText;

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
        playerObj = skin.transform.Find("Player").gameObject;
        callBtn = skin.transform.Find("CallButtonList/CallBtn").GetComponent<Button>();
        notCallBtn = skin.transform.Find("CallButtonList/NotCallBtn").GetComponent<Button>();
        robBtn = skin.transform.Find("RobButtonList/RobBtn").GetComponent<Button>();
        notRobBtn = skin.transform.Find("RobButtonList/NotRobBtn").GetComponent<Button>();
        leftIDText = skin.transform.Find("LeftPlayer/IDText/Text").GetComponent<Text>();
        rightIDText = skin.transform.Find("RightPlayer/Text").GetComponent<Text>();

        GameManager.leftActionObj = skin.transform.Find("LeftPlayer/Action").gameObject;
        GameManager.rightActionObj = skin.transform.Find("RightPlayer/Action").gameObject;

        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);

        // 监听网络网络事件
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCall", OnMsgCall);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRob", OnMsgStartRob);
        NetManager.AddMsgListener("MsgRob", OnMsgRob);

        // 监听按钮事件
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);

        // 发送消息
        MsgGetPlayer msgGetPlayer = new MsgGetPlayer();
        NetManager.Send(msgGetPlayer);
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
        NetManager.RemoveMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.RemoveMsgListener("MsgCall", OnMsgCall);
        NetManager.RemoveMsgListener("MsgReStart", OnMsgReStart);
        NetManager.RemoveMsgListener("MsgStartRob", OnMsgStartRob);
        NetManager.RemoveMsgListener("MsgRob", OnMsgRob);
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
            image.rectTransform.sizeDelta = new Vector2(126, 163);
            // 设置卡牌图片的缩放比例
            image.rectTransform.localScale = Vector3.one;
            // 设置卡牌图片的层级
            cardObj.layer = LayerMask.NameToLayer("UI");
        }

        CardSort();
    }

    /// <summary>
    /// 排序
    /// </summary>
    public void CardSort()
    {
        Transform cardsTrans = playerObj.transform.Find("Cards");

        // 插入排序
        for (int i = 1; i < cardsTrans.childCount; i++)
        {
            int currentRank = (int)CardManager.GetCard(cardsTrans.GetChild(i).name).rank;
            int currentSuit = (int)CardManager.GetCard(cardsTrans.GetChild(i).name).suit;
            for (int j = 0; j < i; j++)
            {
                int rank = (int)CardManager.GetCard(cardsTrans.GetChild(j).name).rank;
                int suit = (int)CardManager.GetCard(cardsTrans.GetChild(j).name).suit;
                if (currentRank > rank)
                {
                    cardsTrans.GetChild(i).SetSiblingIndex(j);
                    break;
                }
                else if (currentRank == rank && currentSuit > suit)
                {
                    cardsTrans.GetChild(i).SetSiblingIndex(j);
                    break;
                }
            }
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
        MsgCall msgCall = new MsgCall();
        msgCall.call = true;
        NetManager.Send(msgCall);
    }

    private void OnNotCallBtnClick()
    {
        MsgCall msgCall = new MsgCall();
        msgCall.call = false;
        NetManager.Send(msgCall);
    }

    private void OnRobBtnClick()
    {
        MsgRob msgRob = new MsgRob();
        msgRob.isRob = true;
        NetManager.Send(msgRob);
    }

    private void OnNotRobBtnClick()
    {
        MsgRob msgRob = new MsgRob();
        msgRob.isRob = false;
        NetManager.Send(msgRob);
    }

    public void OnMsgSwitchPlayer(MsgBase msgBase)
    {
        MsgSwitchPlayer msg = msgBase as MsgSwitchPlayer;
        switch (GameManager.status)
        {
            case PlayerStatus.Call: // 叫地主
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
            case PlayerStatus.Rob:  // 抢地主
                if (msg.id == GameManager.id)
                {
                    robBtn.gameObject.SetActive(true);
                    notCallBtn.gameObject.SetActive(true);

                    callBtn.gameObject.SetActive(false);
                    notCallBtn.gameObject.SetActive(false);
                }
                else
                {
                    robBtn.gameObject.SetActive(false);
                    notCallBtn.gameObject.SetActive(false);
                    callBtn.gameObject.SetActive(false);
                    notCallBtn.gameObject.SetActive(false);
                }
                break;
            case PlayerStatus.Play: // 正式开始
                break;
            default:
                break;
        }
    }

    public void OnMsgGetPlayer(MsgBase msgBase)
    {
        MsgGetPlayer msg = msgBase as MsgGetPlayer;
        GameManager.leftID = msg.leftID;
        GameManager.rightID = msg.rightID;

        leftIDText.text = msg.leftID;
        rightIDText.text = msg.rightID;
    }

    public void OnMsgCall(MsgBase msgBase)
    {
        MsgCall msg = msgBase as MsgCall;
        if (msg.call)
            GameManager.SyncGenerate(msg.id, "Word/Call");
        else
            GameManager.SyncGenerate(msg.id, "Word/NotCall");

        if (msg.id != GameManager.id)
            return;

        switch (msg.result)
        {
            case 0:
                break;
            case 1: // 抢地主
                MsgStartRob msgStartRob = new MsgStartRob();
                NetManager.Send(msgStartRob);
                break;
            case 2: // 重新洗牌
                MsgReStart msgReStart = new MsgReStart();
                NetManager.Send(msgReStart);
                break;
            case 3: // 自己是地主
                TurnLandLord();
                break;
            default:
                break;
        }

        MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();
        NetManager.Send(msgSwitchPlayer);
    }

    public void TurnLandLord()
    {
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("Image/LandLord");
        Sprite sprite = go.GetComponent<Image>().sprite;
        playerObj.transform.Find("PlayerImage").GetComponent<Image>().sprite = sprite;
    }

    public void OnMsgReStart(MsgBase msgBase)
    {
        // 此消息中不包含任何内容，接不接收无所谓，这里就统一接收一下
        MsgReStart msg = msgBase as MsgReStart;

        Transform cardsTrans = playerObj.transform.Find("Cards");
        for (int i = cardsTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsTrans.GetChild(i).gameObject);
        }
        GameManager.cards.Clear();
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    public void OnMsgStartRob(MsgBase msgBase)
    {
        MsgStartRob msg = msgBase as MsgStartRob;
        GameManager.status = PlayerStatus.Rob;
    }

    public void OnMsgRob(MsgBase msgBase)
    {
        MsgRob msg = msgBase as MsgRob;
        MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();

        if (msg.isRob)
            GameManager.SyncGenerate(msg.id, "Word/Rob");
        else
            GameManager.SyncGenerate(msg.id, "Word/NotRob");

        if(msg.landLordID == GameManager.id)
        {
            TurnLandLord();
        }

        if (!msg.needRob)
        {
            msgSwitchPlayer.round = 2;
            NetManager.Send(msgSwitchPlayer);
            return;
        }

        msgSwitchPlayer.round = 1;
        NetManager.Send(msgSwitchPlayer);
    }
}
