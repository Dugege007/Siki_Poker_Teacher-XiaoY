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

    // 按钮
    private Button callBtn;
    private Button notCallBtn;
    private Button robBtn;
    private Button notRobBtn;
    private Button playBtn;
    private Button notPlayBtn;

    // 游戏结束
    private Text winText;

    // 游戏音乐
    private AudioSource battleSFX;

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
        playBtn = skin.transform.Find("PlayButtonList/PlayBtn").GetComponent<Button>();
        notPlayBtn = skin.transform.Find("PlayButtonList/NotPlayBtn").GetComponent<Button>();

        GameManager.actionObj = playerObj.transform.Find("Action").gameObject;
        GameManager.playCardsObj = playerObj.transform.Find("PlayCards").gameObject;

        leftIDText = skin.transform.Find("LeftPlayer/IDText/Text").GetComponent<Text>();
        rightIDText = skin.transform.Find("RightPlayer/Text").GetComponent<Text>();

        GameManager.leftPlayerImage = skin.transform.Find("LeftPlayer/PlayerImage").gameObject;
        GameManager.rightPlayerImage = skin.transform.Find("RightPlayer/PlayerImage").gameObject;
        GameManager.leftActionObj = skin.transform.Find("LeftPlayer/Action").gameObject;
        GameManager.rightActionObj = skin.transform.Find("RightPlayer/Action").gameObject;
        GameManager.leftPlayCardsObj = skin.transform.Find("LeftPlayer/PlayCards").gameObject;
        GameManager.rightPlayCardsObj = skin.transform.Find("RightPlayer/PlayCards").gameObject;
        GameManager.leftHandCards = skin.transform.Find("LeftPlayer/HandCards").gameObject;
        GameManager.rightHandCards = skin.transform.Find("RightPlayer/HandCards").gameObject;

        GameManager.threeCardsObj = skin.transform.Find("ThreeCards").gameObject;

        winText = skin.transform.Find("WinPanel/WinText").GetComponent<Text>();

        battleSFX = skin.transform.Find("BattleSFX").GetComponent<AudioSource>();

        // 先关闭按钮
        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);
        notPlayBtn.gameObject.SetActive(false);

        // 监听网络网络事件
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCall", OnMsgCall);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRob", OnMsgStartRob);
        NetManager.AddMsgListener("MsgRob", OnMsgRob);
        NetManager.AddMsgListener("MsgPlayCards", OnMsgPlayCards);

        // 监听按钮事件
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);
        playBtn.onClick.AddListener(OnPlayBtnClick);
        notPlayBtn.onClick.AddListener(OnNotPlayBtnClick);

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
        NetManager.RemoveMsgListener("MsgPlayCards", OnMsgPlayCards);
    }

    // 向服务器发送获取卡牌列表消息
    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        // 将收到的卡牌添加到游戏管理器的卡牌列表中
        for (int i = 0; i < 17; i++)
        {
            Card card = new Card(msg.cardsInfo[i].suit, msg.cardsInfo[i].rank);
            GameManager.cards.Add(card);
        }

        for (int i = 0; i < 3; i++)
        {
            Card card = new Card(msg.threeCardsInfo[i].suit, msg.threeCardsInfo[i].rank);
            GameManager.threeCards.Add(card);
        }

        // 生成卡牌
        GenerateCard(GameManager.cards.ToArray());
    }

    /// <summary>
    /// 生成卡牌
    /// </summary>
    /// <param name="cards">卡牌数组</param>
    public void GenerateCard(Card[] cards)
    {
        Transform cardTrans = playerObj.transform.Find("HandCards");

        // 先删除所有子物体
        for (int i = cardTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardTrans.GetChild(i).gameObject);
        }

        // 遍历卡牌数组，为每张卡牌创建一个游戏物体，并设置其属性
        for (int i = 0; i < cards.Length; i++)
        {
            // 获取卡牌名称
            string name = CardManager.GetName(cards[i]);
            // 创建卡牌游戏物体
            GameObject cardObj = new GameObject(name);
            // 设置卡牌游戏物体的父物体为玩家游戏物体
            cardObj.transform.SetParent(cardTrans, false);
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
            cardObj.AddComponent<CardUI>();
        }

        CardSort();
    }

    /// <summary>
    /// 排序
    /// </summary>
    public void CardSort()
    {
        Transform cardsTrans = playerObj.transform.Find("HandCards");

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

    // 按钮事件
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

    private void OnPlayBtnClick()
    {
        MsgPlayCards msgPlayCards = new MsgPlayCards();
        msgPlayCards.play = true;
        msgPlayCards.cardsInfo = CardManager.GetCardInfos(GameManager.selectedCard.ToArray());
        NetManager.Send(msgPlayCards);
    }

    private void OnNotPlayBtnClick()
    {
        MsgPlayCards msgPlayCards = new MsgPlayCards();
        msgPlayCards.play = false;
        NetManager.Send(msgPlayCards);
    }

    public void OnMsgSwitchPlayer(MsgBase msgBase)
    {
        MsgSwitchPlayer msg = msgBase as MsgSwitchPlayer;
        switch (GameManager.status)
        {
            case PlayerStatus.Call: // 叫地主
                if (msg.id == GameManager.id)
                {
                    GameManager.SyncDestroy(msg.id);
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
                callBtn.gameObject.SetActive(false);
                notCallBtn.gameObject.SetActive(false);
                if (msg.id == GameManager.id)
                {
                    GameManager.SyncDestroy(msg.id);
                    robBtn.gameObject.SetActive(true);
                    notRobBtn.gameObject.SetActive(true);
                }
                else
                {
                    robBtn.gameObject.SetActive(false);
                    notRobBtn.gameObject.SetActive(false);
                }
                break;
            case PlayerStatus.Play: // 正式开始，出牌阶段
                robBtn.gameObject.SetActive(false);
                notRobBtn.gameObject.SetActive(false);
                callBtn.gameObject.SetActive(false);
                notCallBtn.gameObject.SetActive(false);
                if (msg.id == GameManager.id)
                {
                    GameManager.SyncDestroy(msg.id);
                    playBtn.gameObject.SetActive(true);
                    notPlayBtn.gameObject.SetActive(true);
                    if (GameManager.canPressNotPlayBtn)
                    {
                        notPlayBtn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        notPlayBtn.enabled = true;
                    }
                    else
                    {
                        notPlayBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                        notPlayBtn.enabled = false;
                    }
                }
                else
                {
                    playBtn.gameObject.SetActive(false);
                    notPlayBtn.gameObject.SetActive(false);
                }
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
        MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();

        if (msg.call)
        {
            GameManager.SyncDestroy(msg.id);
            GameManager.SyncGenerateActionObj(msg.id, "Word/Call");
        }
        else
        {
            GameManager.SyncDestroy(msg.id);
            GameManager.SyncGenerateActionObj(msg.id, "Word/NotCall");
        }

        // 地主出来了
        if (msg.result == 3)
        {
            GameManager.SyncDestroy(msg.id);
            SyncLandLord(msg.id);
            RevealCards(GameManager.threeCards.ToArray());
            GameManager.status = PlayerStatus.Play;
        }

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
                msgSwitchPlayer.round = 0;
                break;
            default:
                break;
        }

        NetManager.Send(msgSwitchPlayer);
    }

    /// <summary>
    /// 变成地主
    /// </summary>
    public void TurnLandLord()
    {
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("Image/LandLord");
        Sprite sprite = go.GetComponent<Image>().sprite;
        playerObj.transform.Find("PlayerImage").GetComponent<Image>().sprite = sprite;

        Card[] cards = new Card[20];
        Array.Copy(GameManager.cards.ToArray(), 0, cards, 0, 17);
        Array.Copy(GameManager.threeCards.ToArray(), 0, cards, 17, 3);

        GenerateCard(cards);
    }

    public void SyncLandLord(string id)
    {
        GameObject go = Resources.Load<GameObject>("Image/LandLord");
        Sprite sprite = go.GetComponent<Image>().sprite;

        if (GameManager.leftID == id)
        {
            GameManager.leftPlayerImage.GetComponent<Image>().sprite = sprite;

            Text countText = GameManager.leftActionObj.transform.parent.Find("CardCountText").GetComponent<Text>();
            countText.text = "20";
        }

        if (GameManager.rightID == id)
        {
            GameManager.rightPlayerImage.GetComponent<Image>().sprite = sprite;

            Text countText = GameManager.rightActionObj.transform.parent.Find("CardCountText").GetComponent<Text>();
            countText.text = "20";
        }
    }

    public void OnMsgReStart(MsgBase msgBase)
    {
        // 此消息中不包含任何内容，接不接收无所谓，这里就统一接收一下
        MsgReStart msg = msgBase as MsgReStart;

        Transform cardsTrans = playerObj.transform.Find("HandCards");
        for (int i = cardsTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsTrans.GetChild(i).gameObject);
        }
        GameManager.cards.Clear();
        GameManager.threeCards.Clear();
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
        {
            // 播放音频
            PlayBattleSFX("Man_Rob", 1, 4);

            // 同步销毁行动提示
            GameManager.SyncDestroy(msg.id);
            // 同步生成行动提示
            GameManager.SyncGenerateActionObj(msg.id, "Word/Rob");
        }
        else
        {
            // 播放音频
            PlayBattleSFX("Man_NoRob");

            GameManager.SyncDestroy(msg.id);
            GameManager.SyncGenerateActionObj(msg.id, "Word/NotRob");
        }

        SyncLandLord(msg.landLordID);

        // 地主出来了
        if (msg.landLordID != "")
        {
            RevealCards(GameManager.threeCards.ToArray());
            GameManager.status = PlayerStatus.Play;
            msgSwitchPlayer.round = 0;
        }

        // 如果自己是地主
        if (msg.landLordID == GameManager.id)
            TurnLandLord();

        if (msg.id != GameManager.id)
            return;

        if (!msg.needRob)
        {
            msgSwitchPlayer.round = 2;
            NetManager.Send(msgSwitchPlayer);
            return;
        }

        msgSwitchPlayer.round = 1;
        NetManager.Send(msgSwitchPlayer);
    }

    /// <summary>
    /// 揭示底牌
    /// </summary>
    /// <param name="cards">底牌数组</param>
    public void RevealCards(Card[] cards)
    {
        for (int i = 0; i < 3; i++)
        {
            string name = CardManager.GetName(cards[i]);
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            GameManager.threeCardsObj.transform.GetChild(i).GetComponent<Image>().sprite = sprite;

            GameManager.cards.Add(cards[i]);
        }
    }

    public void OnMsgPlayCards(MsgBase msgBase)
    {
        MsgPlayCards msg = msgBase as MsgPlayCards;
        //Debug.Log((CardManager.CardType)msg.cardType);
        GameManager.canPressNotPlayBtn = msg.canPressNotPlayBtn;

        if (msg.win == 2)
        {
            // 显示 WinPanel
            winText.transform.parent.gameObject.SetActive(true);
            if (GameManager.isLandLord)
            {
                winText.text = "地主胜利！";
                winText.color = Color.white;
            }
            else
            {
                winText.text = "农民失败！";
                winText.color = Color.red;
            }
        }

        if (msg.win == 1)
        {
            // 显示 WinPanel
            winText.transform.parent.gameObject.SetActive(true);
            if (GameManager.isLandLord)
            {
                winText.text = "地主失败！";
                winText.color = Color.red;
            }
            else
            {
                winText.text = "农民胜利！";
                winText.color = Color.white;
            }
        }

        if (msg.result)
        {
            if (msg.play)
            {
                Card[] cards = CardManager.GetCards(msg.cardsInfo);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);
                GameManager.SyncDestroy(msg.id);
                GameManager.SyncGenerateOthersHandCardsObj(msg.id, cards.Length);

                // 同步生成卡牌
                for (int i = 0; i < cards.Length; i++)
                {
                    GameManager.SyncGeneratePlayCardsObj(msg.id, CardManager.GetName(cards[i]));
                }
            }
            else
            {
                GameManager.SyncDestroy(msg.id);
                GameManager.SyncGenerateActionObj(msg.id, "Word/NotPlay");
            }
        }

        // 处理当前客户端
        if (GameManager.id != msg.id) return;

        if (msg.result)
        {
            if (msg.play)
            {
                Card[] cards = CardManager.GetCards(msg.cardsInfo);
                // 升序排序
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                // 删除客户端储存的牌
                for (int i = 0; i < cards.Length; i++)
                {
                    // 删除手牌列表中的牌
                    for (int j = GameManager.cards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.cards[j].Equals(cards[i]))
                            GameManager.cards.RemoveAt(j);
                    }

                    // 删除选中的牌
                    for (int j = GameManager.selectedCard.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.selectedCard[j].Equals(cards[i]))
                            GameManager.selectedCard.RemoveAt(j);
                    }
                }

                GenerateCard(GameManager.cards.ToArray());
            }

            MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();
            NetManager.Send(msgSwitchPlayer);
        }
    }

    private void PlayBattleSFX(string audioName)
    {
        string audioPath = "Sounds/";
        audioPath = audioPath + audioName;
        battleSFX.clip = Resources.Load<AudioClip>(audioPath);
        battleSFX.Play();
    }

    private void PlayBattleSFX(string audioName, int minIndex, int maxIndex)
    {
        string audioPath = "Sounds/";
        audioPath = audioPath + audioName + UnityEngine.Random.Range(minIndex, maxIndex);
        battleSFX.clip = Resources.Load<AudioClip>(audioPath);
        battleSFX.Play();
    }
}
