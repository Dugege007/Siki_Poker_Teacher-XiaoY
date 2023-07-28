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

        // ���������Ϣ����
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    // �����ر�ʱִ�еĲ���
    public override void OnClose()
    {

    }

    // ��¼��ť����¼�����
    public void OnLoginClick()
    {
        // ��������ֶ�Ϊ���򲻽��в���
        if (idInput.text == "" || pwInput.text == "")
            return;

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

    }

    // �����¼��Ϣ�Ļص�
    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = new MsgLogin();
        if (msg.result)
        {
            Debug.Log("��¼�ɹ�");
        }
        else
        {
            Debug.Log("��¼ʧ��");
        }
    }
}
