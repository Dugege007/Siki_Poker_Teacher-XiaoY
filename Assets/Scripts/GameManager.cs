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
    /// 当前玩家 ID
    /// </summary>
    public static string id = "";

    /// <summary>
    /// 当前玩家是否是房主
    /// </summary>
    public static bool isHost = false;

    /// <summary>
    /// 游戏物体根目录
    /// </summary>
    private Transform rootTrans;

    /// <summary>
    /// 玩家手牌列表
    /// </summary>
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// 底牌
    /// </summary>
    public static List<Card> threeCards = new List<Card>();

    /// <summary>
    /// 玩家状态
    /// 默认为叫地主
    /// </summary>
    public static PlayerStatus status = PlayerStatus.Call;

    /// <summary>
    /// 左玩家 ID
    /// </summary>
    public static string leftID = "";

    /// <summary>
    /// 右玩家 ID
    /// </summary>
    public static string rightID = "";

    /// <summary>
    /// 左玩家生成的游戏物体
    /// </summary>
    public static GameObject leftActionObj;

    /// <summary>
    /// 右玩家生成的游戏物体
    /// </summary>
    public static GameObject rightActionObj;

    /// <summary>
    /// 左玩家图片
    /// </summary>
    public static GameObject leftPlayerImage;

    /// <summary>
    /// 右玩家图片
    /// </summary>
    public static GameObject rightPlayerImage;

    /// <summary>
    /// 自身是否是地主
    /// </summary>
    public static bool isLandLord = false;

    /// <summary>
    /// 三张底牌
    /// </summary>
    public static GameObject threeCardsObj;

    /// <summary>
    /// 是否按下鼠标选牌
    /// </summary>
    public static bool isPress;

    /// <summary>
    /// 已选择的手牌
    /// </summary>
    public static List<Card> selectedCard = new List<Card>();

    /// <summary>
    /// 允许不出
    /// 如果为 true，显示“不出”按钮；如果为 false，“不出”按钮改为不可选择
    /// </summary>
    public static bool canNotPlay;

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
        PanelManager.Open<TipPanel>("断开连接");
    }

    public void OnMsgKick(MsgBase msgBase)
    {
        rootTrans.GetComponent<BasePanel>().Close();
        PanelManager.Open<TipPanel>("被踢下线");
        PanelManager.Open<LoginPanel>();
    }

    public static void SyncDestroy(string id)
    {
        if(leftID == id)
        {
            for (int i = leftActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftActionObj.transform.GetChild(i).gameObject);
            }
        }

        if(rightID == id)
        {
            for (int i = rightActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightActionObj.transform.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 同步生成游戏物体（从资源文件夹中）
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <param name="name">生成物体的名称</param>
    public static void SyncGenerate(string id, string name)
    {
        GameObject resource = Resources.Load<GameObject>(name);
        if (leftID == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(leftActionObj.transform, false);
        }
        if (rightID == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(rightActionObj.transform, false);
        }
    }
}
