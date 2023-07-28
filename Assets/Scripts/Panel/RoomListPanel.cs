using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    // 定义面板中的各种组件
    private Text idText;
    private Text beanText;
    private Button createBtn;
    private Button refreshBtn;
    private Transform contentTrans;
    private GameObject roomObj;

    private Text roomIDText;

    // 在面板初始化时设置面板的资源路径和层级
    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    // 在面板显示时获取组件并添加事件监听
    public override void OnShow(params object[] para)
    {
        // 获取各种组件
        idText = skin.transform.Find("HeadImage/IDText/Text").GetComponent<Text>();
        beanText = skin.transform.Find("HeadImage/BeanImage/Text").GetComponent<Text>();
        createBtn = skin.transform.Find("JoinImage/InfoList/CreateBtn").GetComponent<Button>();
        refreshBtn = skin.transform.Find("JoinImage/InfoList/RefreshBtn").GetComponent<Button>();
        contentTrans = skin.transform.Find("RoomListImage/Scroll View/Viewport/Content");
        roomObj = skin.transform.Find("RoomImage").gameObject;

        // 隐藏房间对象
        roomObj.SetActive(false);
        // 设置id文本
        idText.text = GameManager.id;

        // 添加按钮点击事件监听
        createBtn.onClick.AddListener(OnCreateClick);
        refreshBtn.onClick.AddListener(OnRefreshClick);

        // 添加网络消息监听
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        // 发送获取成就和获取房间列表的消息
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    // 在面板关闭时移除网络消息监听
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }

    // 创建房间按钮点击事件处理
    public void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    // 刷新按钮点击事件处理
    public void OnRefreshClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    // 处理获取成就的消息
    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = msgBase as MsgGetAchieve;
        beanText.text = msg.bean.ToString();

        //TODO 豆子赋给进入游戏后的玩家
    }

    // 处理创建房间的消息
    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msg = msgBase as MsgCreateRoom;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("创建房间成功");
            //TODO 打开RoomPanel
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("创建房间失败");
        }
    }

    // 处理获取房间列表的消息
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

    // 处理进入房间的消息
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
            PanelManager.Open<TipPanel>("加入房间失败");
        }
    }

    // 生成房间
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
            statusText.text = "准备中";
        else
            statusText.text = "已开始";
        joinBtn.onClick.AddListener(OnJoinClick);
    }

    // 加入房间按钮点击事件
    public void OnJoinClick()
    {
        MsgEnterRoom msgEnterRoom = new MsgEnterRoom();
        msgEnterRoom.roomID = int.Parse(roomIDText.text);
        NetManager.Send(msgEnterRoom);
    }
}
