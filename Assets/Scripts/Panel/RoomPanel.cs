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

    }
}
