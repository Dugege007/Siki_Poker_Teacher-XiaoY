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
    /// ����
    /// </summary>
    public static List<Card> threeCards = new List<Card>();

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

    /// <summary>
    /// ��������ɵ���Ϸ����
    /// </summary>
    public static GameObject actionObj;

    /// <summary>
    /// ��������ɵ���Ϸ����
    /// </summary>
    public static GameObject leftActionObj;

    /// <summary>
    /// ��������ɵ���Ϸ����
    /// </summary>
    public static GameObject rightActionObj;

    /// <summary>
    /// ����ҳ�����
    /// </summary>
    public static GameObject playCardsObj;

    /// <summary>
    /// ����ҳ�����
    /// </summary>
    public static GameObject leftPlayCardsObj;

    /// <summary>
    /// ����ҳ�����
    /// </summary>
    public static GameObject rightPlayCardsObj;

    /// <summary>
    /// �����ͼƬ
    /// </summary>
    public static GameObject leftPlayerImage;

    /// <summary>
    /// �����ͼƬ
    /// </summary>
    public static GameObject rightPlayerImage;

    /// <summary>
    /// �����Ƿ��ǵ���
    /// </summary>
    public static bool isLandLord = false;

    /// <summary>
    /// ���ŵ���
    /// </summary>
    public static GameObject threeCardsObj;

    /// <summary>
    /// �Ƿ������ѡ��
    /// </summary>
    public static bool isPress;

    /// <summary>
    /// ��ѡ�������
    /// </summary>
    public static List<Card> selectedCard = new List<Card>();

    /// <summary>
    /// ������
    /// ���Ϊ true����ʾ����������ť�����Ϊ false������������ť��Ϊ����ѡ��
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
        PanelManager.Open<TipPanel>("�Ͽ�����");
    }

    public void OnMsgKick(MsgBase msgBase)
    {
        rootTrans.GetComponent<BasePanel>().Close();
        PanelManager.Open<TipPanel>("��������");
        PanelManager.Open<LoginPanel>();
    }

    /// <summary>
    /// ͬ��������Ϸ����
    /// �����ж���ʾ���ѳ�����
    /// </summary>
    /// <param name="thisTurnID">��ǰ�غϵ���� ID</param>
    public static void SyncDestroy(string thisTurnID)
    {
        // ����Ǹ����Ǳ����
        if (id == thisTurnID)
        {
            // ɾ���ж���ʾ
            for (int i = actionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(actionObj.transform.GetChild(i).gameObject);
            }

            // ɾ������
            for (int i = playCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(playCardsObj.transform.GetChild(i).gameObject);
            }
        }

        // ����������
        if (leftID == thisTurnID)
        {
            // ɾ���ж���ʾ
            for (int i = leftActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftActionObj.transform.GetChild(i).gameObject);
            }

            // ɾ������
            for (int i = leftPlayCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftPlayCardsObj.transform.GetChild(i).gameObject);
            }
        }

        // ����������
        if (rightID == thisTurnID)
        {
            // ɾ���ж���ʾ
            for (int i = rightActionObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightActionObj.transform.GetChild(i).gameObject);
            }

            // ɾ������
            for (int i = rightPlayCardsObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightPlayCardsObj.transform.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// ͬ�������ж���ʾ��Ϸ���壨����Դ�ļ����У�
    /// </summary>
    /// <param name="id">��ǰ�غϵ���� ID</param>
    /// <param name="name">�������������</param>
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
    /// ͬ�����ɳ������壨����Դ�ļ����У�
    /// </summary>
    /// <param name="id">��ǰ�غϵ���� ID</param>
    /// <param name="name">�������������</param>
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
