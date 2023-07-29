using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 当前玩家 ID
    /// </summary>
    public static string id = "";

    /// <summary>
    /// 当前玩家是否是房主
    /// </summary>
    public static bool isHost = false;

    private void Start()
    {
        //NetManager.Connect("127.0.0.1", 8888);

        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("OnMsgKick", OnMsgKick);
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }

    private void Update()
    {
        NetManager.Update();
    }

    public void OnConnectClose(string err)
    {
        PanelManager.Open<TipPanel>("断开连接");
    }

    public void OnMsgKick(MsgBase msgBase)
    {
        PanelManager.Open<TipPanel>("被踢下线，已断开连接");
    }


}
