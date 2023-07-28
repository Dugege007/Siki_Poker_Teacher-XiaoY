using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    private Text idText;
    private Text beanText;
    private Button createBtn;
    private Button refreshBtn;
    private Transform contentTrans;
    private GameObject roomObj;

    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        idText = skin.transform.Find("HeadImage/IDText/Text").GetComponent<Text>();
        beanText = skin.transform.Find("HeadImage/BeanImage/Text").GetComponent<Text>();
        createBtn = skin.transform.Find("JoinImage/InfoList/CreateBtn").GetComponent<Button>();
        refreshBtn = skin.transform.Find("JoinImage/InfoList/RefreshBtn").GetComponent<Button>();
        contentTrans = skin.transform.Find("RoomListImage/Scroll View/Viewport/Content");
        roomObj = skin.transform.Find("RoomImage").gameObject;

        roomObj.SetActive(false);
        idText.text = GameManager.id;

        createBtn.onClick.AddListener(OnCreateClick);
        refreshBtn.onClick.AddListener(OnRefreshClick);
    }

    public void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    public void OnRefreshClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }
}
