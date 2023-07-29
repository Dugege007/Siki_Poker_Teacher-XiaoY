using System.Collections;
using System.Collections.Generic;
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



        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);

    }

    public void OnStartClick()
    {

    }

    public void OnPrepareClick()
    {

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
            statusText.text = "已准备";
        else
            statusText.text = "未准备";
    }

    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = msgBase as MsgLeaveRoom;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("退出房间");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("退出房间失败");
        }
    }
}
