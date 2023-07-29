using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    // �����¼����е������ֶκͰ�ť
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button registBtn;

    // ������ʼ��ʱ����������Դ·���Ͳ㼶
    public override void OnInit()
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;

        NetManager.Connect("127.0.0.1", 8888);
    }

    // �������ʾʱ��ȡ���������¼�����
    public override void OnShow(params object[] para)
    {
        // ��ȡ�����ֶκͰ�ť�����
        idInput = skin.transform.Find("InputBox/UserNameText/InputField").GetComponent<InputField>();
        pwInput = skin.transform.Find("InputBox/PasswordText/InputField").GetComponent<InputField>();
        loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        registBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();

        // Ϊ��ť��ӵ���¼�����
        loginBtn.onClick.AddListener(OnLoginClick);
        registBtn.onClick.AddListener(OnRegisterClick);

        // ��������¼�����
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectFail);
        // ���������Ϣ����
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    // �����ر�ʱִ�еĲ���
    public override void OnClose()
    {
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectFail);

        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
    }

    // ��¼��ť����¼�����
    public void OnLoginClick()
    {
        // ��������ֶ�Ϊ���򲻽��в���
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("�û��������벻��Ϊ��");
            return;
        }

        // ������¼��Ϣ������
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = idInput.text;
        msgLogin.pw = pwInput.text;
        NetManager.Send(msgLogin);
    }

    // ע�ᰴť����¼�����
    public void OnRegisterClick()
    {
        // ��ע�����
        PanelManager.Open<RegisterPanel>();
    }

    // �����¼��Ϣ�Ļص�
    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if (msg.result)
        {
            // ��¼�ɹ�
            PanelManager.Open<TipPanel>("��¼�ɹ�");
            PanelManager.Open<RoomListPanel>();
            GameManager.id = msg.id;
            Close();
        }
        else
        {
            // ��¼ʧ��
            PanelManager.Open<TipPanel>("��¼ʧ��");
        }
    }

    public void OnConnectSucc(string err)
    {
        // ���ӳɹ�
        PanelManager.Open<TipPanel>("���ӳɹ�");
    }

    public void OnConnectFail(string err)
    {
        // ����ʧ��
        PanelManager.Open<TipPanel>(err);
    }
}
