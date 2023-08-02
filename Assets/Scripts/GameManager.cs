using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatus
{
    Call,
    Rob,
    Play,
}

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ��ǰ��� ID
    /// </summary>
    public static string id = "";

    /// <summary>
    /// ��ǰ����Ƿ��Ƿ���
    /// </summary>
    public static bool isHost = false;

    /// <summary>
    /// ��Ϸ�����Ŀ¼
    /// </summary>
    private Transform rootTrans;

    /// <summary>
    /// ��������б�
    /// </summary>
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// ���״̬
    /// Ĭ��Ϊ�е���
    /// </summary>
    public static PlayerStatus status = PlayerStatus.Call;

    /// <summary>
    /// ����� ID
    /// </summary>
    public static string leftID = "";

    /// <summary>
    /// ����� ID
    /// </summary>
    public static string rightID = "";

    private void Start()
    {
        //NetManager.Connect("127.0.0.1", 8888);

        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("OnMsgKick", OnMsgKick);
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();

        rootTrans = transform.Find("Root");

        CardManager.Init();
    }

    private void Update()
    {
        NetManager.Update();
    }

    public void OnConnectClose(string err)
    {
        PanelManager.Open<TipPanel>("�Ͽ�����");
    }

    public void OnMsgKick(MsgBase msgBase)
    {
        rootTrans.GetComponent<BasePanel>().Close();
        PanelManager.Open<TipPanel>("��������");
        PanelManager.Open<LoginPanel>();
    }
}
