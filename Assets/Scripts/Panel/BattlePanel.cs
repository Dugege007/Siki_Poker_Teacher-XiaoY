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
        playerObj = skin.transform.Find("PlayerImage").gameObject;
        callBtn = skin.transform.Find("CallButtonList/CallBtn").GetComponent<Button>();
        notCallBtn = skin.transform.Find("CallButtonList/NotCallBtn").GetComponent<Button>();
        robBtn = skin.transform.Find("RobButtonList/RobBtn").GetComponent<Button>();
        notRobBtn = skin.transform.Find("RobButtonList/NotRobBtn").GetComponent<Button>();

        callBtn.gameObject.SetActive(false);
        notCallBtn.gameObject.SetActive(false);
        robBtn.gameObject.SetActive(false);
        notRobBtn.gameObject.SetActive(false);

        // �������������¼�
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchPlayer", OnMsgSwitchPlayer);

        // ������ť�¼�
        callBtn.onClick.AddListener(OnCallBtnClick);
        notCallBtn.onClick.AddListener(OnNotCallBtnClick);
        robBtn.onClick.AddListener(OnRobBtnClick);
        notRobBtn.onClick.AddListener(OnNotRobBtnClick);

        // ������Ϣ
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
            image.SetNativeSize();
            image.rectTransform.localScale = Vector3.one * 1.75f;
            // ���ÿ���ͼƬ�Ĳ㼶
            cardObj.layer = LayerMask.NameToLayer("UI");
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
