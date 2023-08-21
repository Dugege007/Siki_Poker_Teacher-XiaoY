using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    /// 本玩家生成的游戏物体
    /// </summary>
    public static GameObject actionObj;

    /// <summary>
    /// 左玩家生成的游戏物体
    /// </summary>
    public static GameObject leftActionObj;

    /// <summary>
    /// 右玩家生成的游戏物体
    /// </summary>
    public static GameObject rightActionObj;

    /// <summary>
    /// 本玩家出的牌
    /// </summary>
    public static GameObject playCardsObj;

    /// <summary>
    /// 左玩家出的牌
    /// </summary>
    public static GameObject leftPlayCardsObj;

    /// <summary>
    /// 右玩家出的牌
    /// </summary>
    public static GameObject rightPlayCardsObj;

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
    public static bool canPressNotPlayBtn;

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

    /// <summary>
    /// 同步销毁游戏物体
    /// 包括行动提示和已出的牌
    /// </summary>
    /// <param name="thisTurnID">当前回合的玩家 ID</param>
    public static void SyncDestroy(string thisTurnID)
    {
        // 如果是该轮是本玩家
        if (id == thisTurnID)
        {
            // 删除行动提示
            for (int i = actionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(actionObj.transform.GetChild(i).gameObject);
            }

            // 删除出牌
            for (int i = playCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(playCardsObj.transform.GetChild(i).gameObject);
            }
        }

        // 如果是左玩家
        if (leftID == thisTurnID)
        {
            // 删除行动提示
            for (int i = leftActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftActionObj.transform.GetChild(i).gameObject);
            }

            // 删除出牌
            for (int i = leftPlayCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftPlayCardsObj.transform.GetChild(i).gameObject);
            }
        }

        // 如果是右玩家
        if (rightID == thisTurnID)
        {
            // 删除行动提示
            for (int i = rightActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightActionObj.transform.GetChild(i).gameObject);
            }

            // 删除出牌
            for (int i = rightPlayCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightPlayCardsObj.transform.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 同步生成行动提示游戏物体（从资源文件夹中）
    /// </summary>
    /// <param name="id">当前回合的玩家 ID</param>
    /// <param name="name">生成物体的名称</param>
    public static void SyncGenerateActionObj(string thisTurnID, string name)
    {
        GameObject resource = Resources.Load<GameObject>(name);

        if (id == thisTurnID)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(actionObj.transform, false);
        }

        if (leftID == thisTurnID)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(leftActionObj.transform, false);
        }

        if (rightID == thisTurnID)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(rightActionObj.transform, false);
        }
    }

    /// <summary>
    /// 同步生成出牌物体（从资源文件夹中）
    /// </summary>
    /// <param name="id">当前回合的玩家 ID</param>
    /// <param name="name">生成物体的名称</param>
    public static void SyncGeneratePlayCardsObj(string thisTurnID, string name)
    {
        name = "Card/" + name;
        Sprite sprite = Resources.Load<Sprite>(name);

        GameObject go = new GameObject(name);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.rectTransform.sizeDelta = new Vector2(61, 80);
        image.rectTransform.localScale = Vector3.one;

        if (leftID == thisTurnID)
            go.transform.SetParent(leftPlayCardsObj.transform, false);

        if (rightID == thisTurnID)
            go.transform.SetParent(rightPlayCardsObj.transform, false);

        if (id == thisTurnID)
            go.transform.SetParent(playCardsObj.transform, false);
    }
}
