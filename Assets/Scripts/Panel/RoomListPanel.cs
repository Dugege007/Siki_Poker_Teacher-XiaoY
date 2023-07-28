using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    // ��������еĸ������
    private Text idText;
    private Text beanText;
    private Button createBtn;
    private Button refreshBtn;
    private Transform contentTrans;
    private GameObject roomObj;

    private Text roomIDText;

    // ������ʼ��ʱ����������Դ·���Ͳ㼶
    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    // �������ʾʱ��ȡ���������¼�����
    public override void OnShow(params object[] para)
    {
        // ��ȡ�������
        idText = skin.transform.Find("HeadImage/IDText/Text").GetComponent<Text>();
        beanText = skin.transform.Find("HeadImage/BeanImage/Text").GetComponent<Text>();
        createBtn = skin.transform.Find("JoinImage/InfoList/CreateBtn").GetComponent<Button>();
        refreshBtn = skin.transform.Find("JoinImage/InfoList/RefreshBtn").GetComponent<Button>();
        contentTrans = skin.transform.Find("RoomListImage/Scroll View/Viewport/Content");
        roomObj = skin.transform.Find("RoomImage").gameObject;

        // ���ط������
        roomObj.SetActive(false);
        // ����id�ı�
        idText.text = GameManager.id;

        // ��Ӱ�ť����¼�����
        createBtn.onClick.AddListener(OnCreateClick);
        refreshBtn.onClick.AddListener(OnRefreshClick);

        // ���������Ϣ����
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        // ���ͻ�ȡ�ɾͺͻ�ȡ�����б����Ϣ
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    // �����ر�ʱ�Ƴ�������Ϣ����
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }

    // �������䰴ť����¼�����
    public void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    // ˢ�°�ť����¼�����
    public void OnRefreshClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    // �����ȡ�ɾ͵���Ϣ
    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = msgBase as MsgGetAchieve;
        beanText.text = msg.bean.ToString();

        //TODO ���Ӹ���������Ϸ������
    }

    // �������������Ϣ
    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msg = msgBase as MsgCreateRoom;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("��������ɹ�");
            //TODO ��RoomPanel
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("��������ʧ��");
        }
    }

    // �����ȡ�����б����Ϣ
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msg = msgBase as MsgGetRoomList;
        for (int i = contentTrans.childCount - 1; i <= 0; i--)
        {
            Destroy(contentTrans.GetChild(i).gameObject);
        }

        if(msg.rooms == null)
            return;

        for (int i = 0; i < msg.rooms.Length; i++)
        {
            GenerateRoom(msg.rooms[i]);
        }
    }

    // ������뷿�����Ϣ
    public void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg = msgBase as MsgEnterRoom;
        if(msg.result)
        {
            //TODO PanelManager.Open<>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("���뷿��ʧ��");
        }
    }

    // ���ɷ���
    public void GenerateRoom(RoomInfo roomInfo)
    {
        GameObject obj = Instantiate(roomObj);
        obj.transform.SetParent(contentTrans);
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one;

        Transform trans = obj.transform;
        roomIDText = trans.Find("IDText/Text").GetComponent<Text>();
        Text countText = trans.Find("CountText/Text").GetComponent<Text>();
        Text statusText = trans.Find("StatusText/Text").GetComponent<Text>();
        Button joinBtn = trans.Find("JoinBtn").GetComponent<Button>();

        roomIDText.text = roomInfo.id.ToString();
        countText.text = roomInfo.count.ToString();
        if (roomInfo.isPrepare)
            statusText.text = "׼����";
        else
            statusText.text = "�ѿ�ʼ";
        joinBtn.onClick.AddListener(OnJoinClick);
    }

    // ���뷿�䰴ť����¼�
    public void OnJoinClick()
    {
        MsgEnterRoom msgEnterRoom = new MsgEnterRoom();
        msgEnterRoom.roomID = int.Parse(roomIDText.text);
        NetManager.Send(msgEnterRoom);
    }
}
