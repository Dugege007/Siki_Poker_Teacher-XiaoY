using System;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    /// <summary>
    /// �����Ϸ����
    /// </summary>
    private GameObject playerObj;

    /// <summary>
    /// ����� ID
    /// </summary>
    private Text leftIDText;

    /// <summary>
    /// ����� ID
    /// </summary>
    private Text rightIDText;

    // ��ť
    private Button callBtn;
    private Button notCallBtn;
    private Button robBtn;
    private Button notRobBtn;
    private Button playBtn;
    private Button notPlayBtn;

    // ��ʼ�����
    public override void OnInit()
    {
        // �������·��
        skinPath = "Panel/BattlePanel";
        // �������㼶
        layer = PanelManager.Layer.Panel;
    }

    // �������ʾʱִ�еĲ���
    public override void OnShow(params object[] para)
    {
        // ��ȡ���
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

        GameManager.threeCardsObj = skin.transform.Find("ThreeCards").gameObject;

        // �ȹرհ�ť
        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);
        playBtn.gameObject.SetActive(false);
        notPlayBtn.gameObject.SetActive(false);

        // �������������¼�
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCall", OnMsgCall);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRob", OnMsgStartRob);
        NetManager.AddMsgListener("MsgRob", OnMsgRob);
        NetManager.AddMsgListener("MsgPlayCards", OnMsgPlayCards);

        // ������ť�¼�
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);
        playBtn.onClick.AddListener(OnPlayBtnClick);
        notPlayBtn.onClick.AddListener(OnNotPlayBtnClick);

        // ������Ϣ
        MsgGetPlayer msgGetPlayer = new MsgGetPlayer();
        NetManager.Send(msgGetPlayer);
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
        MsgGetStartPlayer msgGetStartPlayer = new MsgGetStartPlayer();
        NetManager.Send(msgGetStartPlayer);
    }

    // �����ر�ʱִ�еĲ���
    public override void OnClose()
    {
        // �Ƴ������¼�����
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

    // ����������ͻ�ȡ�����б���Ϣ
    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        // ���յ��Ŀ�����ӵ���Ϸ�������Ŀ����б���
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

        // ���ɿ���
        GenerateCard(GameManager.cards.ToArray());
    }

    /// <summary>
    /// ���ɿ���
    /// </summary>
    /// <param name="cards">��������</param>
    public void GenerateCard(Card[] cards)
    {
        Transform cardTrans = playerObj.transform.Find("HandCards");

        // ��ɾ������������
        for (int i = cardTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardTrans.GetChild(i).gameObject);
        }

        // �����������飬Ϊÿ�ſ��ƴ���һ����Ϸ���壬������������
        for (int i = 0; i < cards.Length; i++)
        {
            // ��ȡ��������
            string name = CardManager.GetName(cards[i]);
            // ����������Ϸ����
            GameObject cardObj = new GameObject(name);
            // ���ÿ�����Ϸ����ĸ�����Ϊ�����Ϸ����
            cardObj.transform.SetParent(cardTrans, false);
            // Ϊ����������� Image ���
            Image image = cardObj.AddComponent<Image>();
            // �������ƻ�ȡ����ͼƬ
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            // ���ÿ���ͼƬ
            image.sprite = sprite;
            // ���ÿ���ͼƬ�Ĵ�С
            image.rectTransform.sizeDelta = new Vector2(126, 163);
            // ���ÿ���ͼƬ�����ű���
            image.rectTransform.localScale = Vector3.one;
            // ���ÿ���ͼƬ�Ĳ㼶
            cardObj.layer = LayerMask.NameToLayer("UI");
            cardObj.AddComponent<CardUI>();
        }

        CardSort();
    }

    /// <summary>
    /// ����
    /// </summary>
    public void CardSort()
    {
        Transform cardsTrans = playerObj.transform.Find("HandCards");

        // ��������
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

    // ����������ͻ�ȡ��һ����ʼ����ҵ���Ϣ
    public void OnMsgGetStartPlayer(MsgBase msgBase)
    {
        MsgGetStartPlayer msg = msgBase as MsgGetStartPlayer;

        if (GameManager.id == msg.id)
        {
            callBtn.gameObject.SetActive(true);
            notCallBtn.gameObject.SetActive(true);
        }
    }

    // ��ť�¼�
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

    }

    public void OnMsgSwitchPlayer(MsgBase msgBase)
    {
        MsgSwitchPlayer msg = msgBase as MsgSwitchPlayer;
        switch (GameManager.status)
        {
            case PlayerStatus.Call: // �е���
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
            case PlayerStatus.Rob:  // ������
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
            case PlayerStatus.Play: // ��ʽ��ʼ�����ƽ׶�
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

        // ����������
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
            case 1: // ������
                MsgStartRob msgStartRob = new MsgStartRob();
                NetManager.Send(msgStartRob);
                break;
            case 2: // ����ϴ��
                MsgReStart msgReStart = new MsgReStart();
                NetManager.Send(msgReStart);
                break;
            case 3: // �Լ��ǵ���
                TurnLandLord();
                break;
            default:
                break;
        }

        MsgSwitchPlayer msgSwitchPlayer = new MsgSwitchPlayer();
        NetManager.Send(msgSwitchPlayer);
    }

    /// <summary>
    /// ��ɵ���
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
            GameManager.leftPlayerImage.GetComponent<Image>().sprite = sprite;

        if (GameManager.rightID == id)
            GameManager.rightPlayerImage.GetComponent<Image>().sprite = sprite;
    }

    public void OnMsgReStart(MsgBase msgBase)
    {
        // ����Ϣ�в������κ����ݣ��Ӳ���������ν�������ͳһ����һ��
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
            GameManager.SyncDestroy(msg.id);
            GameManager.SyncGenerateActionObj(msg.id, "Word/Rob");
        }
        else
        {
            GameManager.SyncDestroy(msg.id);
            GameManager.SyncGenerateActionObj(msg.id, "Word/NotRob");
        }

        SyncLandLord(msg.landLordID);

        // ����������
        if (msg.landLordID != "")
        {
            RevealCards(GameManager.threeCards.ToArray());
            GameManager.status = PlayerStatus.Play;
        }

        // ����Լ��ǵ���
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
    /// ��ʾ����
    /// </summary>
    /// <param name="cards">��������</param>
    public void RevealCards(Card[] cards)
    {
        for (int i = 0; i < 3; i++)
        {
            string name = CardManager.GetName(cards[i]);
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            GameManager.threeCardsObj.transform.GetChild(i).GetComponent<Image>().sprite = sprite;
        }
    }

    public void OnMsgPlayCards(MsgBase msgBase)
    {
        MsgPlayCards msg = msgBase as MsgPlayCards;
        //Debug.Log((CardManager.CardType)msg.cardType);
        GameManager.canPressNotPlayBtn = msg.canPressNotPlayBtn;

        if (msg.result)
        {
            if (msg.play)
            {
                Card[] cards = CardManager.GetCards(msg.cardsInfo);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);
                GameManager.SyncDestroy(msg.id);

                // ͬ�����ɿ���
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

        // ����ǰ�ͻ���
        if (GameManager.id != msg.id) return;

        if (msg.result)
        {
            if (msg.play)
            {
                Card[] cards = CardManager.GetCards(msg.cardsInfo);
                // ��������
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                // ɾ���ͻ��˴������
                for (int i = 0; i < cards.Length; i++)
                {
                    // ɾ�������б��е���
                    for (int j = GameManager.cards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.cards[j].Equals(cards[i]))
                            GameManager.cards.RemoveAt(j);
                    }

                    // ɾ��ѡ�е���
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
}
