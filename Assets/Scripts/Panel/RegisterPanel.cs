using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    // �����¼����е������ֶκͰ�ť
    private InputField idInput;
    private InputField pwInput;
    private InputField cfInput;
    private Button registBtn;
    private Button backBtn;
    private Text tipsText;

    // ������ʼ��ʱ����������Դ·���Ͳ㼶
    public override void OnInit()
    {
        skinPath = "Panel/RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }

    // �������ʾʱ��ȡ���������¼�����
    public override void OnShow(params object[] para)
    {
        // ��ȡ�����ֶκͰ�ť�����
        idInput = skin.transform.Find("InputBox/UserNameText/InputField").GetComponent<InputField>();
        pwInput = skin.transform.Find("InputBox/PasswordText/InputField").GetComponent<InputField>();
        cfInput = skin.transform.Find("InputBox/ConfirmText/InputField").GetComponent<InputField>();
        registBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        backBtn = skin.transform.Find("BGImage/BackBtn").GetComponent<Button>();
        tipsText = skin.transform.Find("TipsText").GetComponent<Text>();

        // Ϊ��ť��ӵ���¼�����
        registBtn.onClick.AddListener(OnRegisterClick);
        backBtn.onClick.AddListener(OnBackClick);

        // ���������Ϣ����
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }

    // �����ر�ʱִ�еĲ���
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }

    // ע�ᰴť����¼�����
    public void OnRegisterClick()
    {
        // ��������ֶ�Ϊ�ջ������������벻һ���򲻽��в���
        if (idInput.text == "" || pwInput.text == "" || cfInput.text == "")
        {
            PanelManager.Open<TipPanel>("�����ֶβ���Ϊ��");
            return;
        }

        if (pwInput.text != cfInput.text)
        {
            PanelManager.Open<TipPanel>("�����������벻һ��");
            tipsText.color = Color.red;
            tipsText.text = "�����������벻һ��";

            return;
        }

        tipsText.color = Color.yellow;
        tipsText.text = "����ע��...";

        // ����ע����Ϣ������
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);
    }

    // ���ذ�ť����¼�����
    public void OnBackClick()
    {
        // �ر�ע�����
        Close();
    }

    private void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = msgBase as MsgRegister;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("ע��ɹ�");
            tipsText.color = Color.green;
            tipsText.text = "ע��ɹ�";

            // ע��ɹ���ر�ע�����
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("ע��ʧ��");
        }
    }
}
