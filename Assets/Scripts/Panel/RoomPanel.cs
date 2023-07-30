using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Button startBtn;
    private Button prepareBtn;
    private Button closeBtn;
    private Transform contentTrans;
    private GameObject playerObj;

    public override void OnInit()
    {
        skinPath = "RoomPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        closeBtn = skin.transform.Find("PlayerListImage/CloseBtn").GetComponent<Button>();
        startBtn = skin.transform.Find("PlayerListImage/StartBtn").GetComponent<Button>();
        prepareBtn = skin.transform.Find("PlayerListImage/PrepareBtn").GetComponent<Button>();
        contentTrans = skin.transform.Find("PlayerListImage/Scroll View/Viewport/Content");
        playerObj = skin.transform.Find("PlayerImage").gameObject;

        playerObj.SetActive(false);

        startBtn.onClick.AddListener(OnStartClick);
        prepareBtn.onClick.AddListener(OnPrepareClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgPrepare", OnMsgPrepare);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);

        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgPrepare", OnMsgPrepare);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
    }

    public void OnStartClick()
    {
        MsgStartBattle msgStartBattle = new MsgStartBattle();
        NetManager.Send(msgStartBattle);
    }

    public void OnPrepareClick()
    {
        MsgPrepare msgPrepare = new MsgPrepare();
        NetManager.Send(msgPrepare);
    }

    public void OnCloseClick()
    {
        MsgLeaveRoom msgLeaveRoom = new MsgLeaveRoom();
        NetManager.Send(msgLeaveRoom);
    }

    public void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msg = msgBase as MsgGetRoomInfo;
        for (int i = contentTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(contentTrans.GetChild(i).gameObject);
        }

        if (msg.players == null)
            return;

        for (int i = 0; i < msg.players.Length; i++)
        {
            GeneratePlayerInfo(msg.players[i]);
        }
    }

    public void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        GameObject obj = Instantiate(playerObj);
        obj.transform.SetParent(contentTrans);
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one;

        Transform trans = obj.transform;
        Text idText = trans.Find("IDText/Text").GetComponent<Text>();
        Text beanText = trans.Find("BeanImage/Text").GetComponent<Text>();
        Text statusText = trans.Find("StatusText/Text").GetComponent<Text>();

        idText.text = playerInfo.id;
        beanText.text = playerInfo.bean.ToString();

        if (playerInfo.isPrepare)
            statusText.text = "��׼��";
        else
            statusText.text = "δ׼��";

        if (playerInfo.isHost)
            statusText.text = "����";

        if (playerInfo.id == GameManager.id)
        {
            GameManager.isHost = playerInfo.isHost;

            if (GameManager.isHost)
            {
                startBtn.gameObject.SetActive(true);
                prepareBtn.gameObject.SetActive(false);
            }
            else
            {
                startBtn.gameObject.SetActive(false);
                prepareBtn.gameObject.SetActive(true);
            }
        }
    }

    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = msgBase as MsgLeaveRoom;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("�˳�����");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("�˳�����ʧ��");
        }
    }

    public void OnMsgPrepare(MsgBase msgBase)
    {
        MsgPrepare msg = msgBase as MsgPrepare;

        if (msg.isPrepare == false)
            return;

        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);
    }

    public void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = msgBase as MsgStartBattle;
        switch (msg.result)
        {
            case 0:
                PanelManager.Open<TipPanel>("�������㣬�޷���ʼ");
                break;
            case 1:
                PanelManager.Open<TipPanel>("��ʼ��Ϸ");
                PanelManager.Open<BattlePanel>();
                Close();
                break;
            case 2:
                PanelManager.Open<TipPanel>("�����δ׼�����޷���ʼ");
                break;
            case 3:
                PanelManager.Open<TipPanel>("���ڵ�ǰ���䣬�޷���ʼ");
                break;
            default:
                PanelManager.Open<TipPanel>("δ֪����");
                break;
        }
    }
}
