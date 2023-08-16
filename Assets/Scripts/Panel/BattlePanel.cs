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

    /// <summary>
    /// �е��� ��ť
    /// </summary>
    private Button callBtn;

    /// <summary>
    /// ���� ��ť
    /// </summary>
    private Button notCallBtn;

    /// <summary>
    /// ������ ��ť
    /// </summary>
    private Button robBtn;

    /// <summary>
    /// ���� ��ť
    /// </summary>
    private Button notRobBtn;

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
        leftIDText = skin.transform.Find("LeftPlayer/IDText/Text").GetComponent<Text>();
        rightIDText = skin.transform.Find("RightPlayer/Text").GetComponent<Text>();

        GameManager.leftActionObj = skin.transform.Find("LeftPlayer/Action").gameObject;
        GameManager.rightActionObj = skin.transform.Find("RightPlayer/Action").gameObject;

        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);

        // �������������¼�
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCall", OnMsgCall);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRob", OnMsgStartRob);
        NetManager.AddMsgListener("MsgRob", OnMsgRob);

        // ������ť�¼�
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);

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
    }

    // ����������ͻ�ȡ�����б���Ϣ
    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        // ���յ��Ŀ�����ӵ���Ϸ�������Ŀ����б���
        for (int i = 0; i < 17; i++)
        {
            Card card = new Card(msg.cardInfos[i].suit, msg.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        // ���ɿ���
        GenerateCard(GameManager.cards.ToArray());
    }

    // ���ɿ��Ƶķ���
    public void GenerateCard(Card[] cards)
    {
        // �����������飬Ϊÿ�ſ��ƴ���һ����Ϸ���壬������������
        for (int i = 0; i < cards.Length; i++)
        {
            // ��ȡ��������
            string name = CardManager.GetName(cards[i]);
            // ����������Ϸ����
            GameObject cardObj = new GameObject(name);
            // ���ÿ�����Ϸ����ĸ�����Ϊ�����Ϸ����
            cardObj.transform.SetParent(playerObj.transform.Find("Cards"), false);
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
        }

        CardSort();
    }

    /// <summary>
    /// ����
    /// </summary>
    public void CardSort()
    {
        Transform cardsTrans = playerObj.transform.Find("Cards");

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
            case PlayerStatus.Call: // �е���
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
            case PlayerStatus.Rob:  // ������
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
            case PlayerStatus.Play: // ��ʽ��ʼ
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

    public void TurnLandLord()
    {
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("Image/LandLord");
        Sprite sprite = go.GetComponent<Image>().sprite;
        playerObj.transform.Find("PlayerImage").GetComponent<Image>().sprite = sprite;
    }

    public void OnMsgReStart(MsgBase msgBase)
    {
        // ����Ϣ�в������κ����ݣ��Ӳ���������ν�������ͳһ����һ��
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
