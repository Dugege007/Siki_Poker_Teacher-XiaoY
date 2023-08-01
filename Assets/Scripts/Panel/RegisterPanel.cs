using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    // 定义登录面板中的输入字段和按钮
    private InputField idInput;
    private InputField pwInput;
    private InputField cfInput;
    private Button registBtn;
    private Button backBtn;
    private Text tipsText;

    // 在面板初始化时设置面板的资源路径和层级
    public override void OnInit()
    {
        skinPath = "Panel/RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }

    // 在面板显示时获取组件并添加事件监听
    public override void OnShow(params object[] para)
    {
        // 获取输入字段和按钮的组件
        idInput = skin.transform.Find("InputBox/UserNameText/InputField").GetComponent<InputField>();
        pwInput = skin.transform.Find("InputBox/PasswordText/InputField").GetComponent<InputField>();
        cfInput = skin.transform.Find("InputBox/ConfirmText/InputField").GetComponent<InputField>();
        registBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        backBtn = skin.transform.Find("BGImage/BackBtn").GetComponent<Button>();
        tipsText = skin.transform.Find("TipsText").GetComponent<Text>();

        // 为按钮添加点击事件监听
        registBtn.onClick.AddListener(OnRegisterClick);
        backBtn.onClick.AddListener(OnBackClick);

        // 添加网络消息监听
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }

    // 在面板关闭时执行的操作
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }

    // 注册按钮点击事件处理
    public void OnRegisterClick()
    {
        // 如果输入字段为空或两次密码输入不一致则不进行操作
        if (idInput.text == "" || pwInput.text == "" || cfInput.text == "")
        {
            PanelManager.Open<TipPanel>("输入字段不能为空");
            return;
        }

        if (pwInput.text != cfInput.text)
        {
            PanelManager.Open<TipPanel>("两次密码输入不一致");
            tipsText.color = Color.red;
            tipsText.text = "两次密码输入不一致";

            return;
        }

        tipsText.color = Color.yellow;
        tipsText.text = "正在注册...";

        // 创建注册消息并发送
        MsgRegister msg = new MsgRegister();
        msg.id = idInput.text;
        msg.pw = pwInput.text;
        NetManager.Send(msg);
    }

    // 返回按钮点击事件处理
    public void OnBackClick()
    {
        // 关闭注册面板
        Close();
    }

    private void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = msgBase as MsgRegister;
        if (msg.result)
        {
            PanelManager.Open<TipPanel>("注册成功");
            tipsText.color = Color.green;
            tipsText.text = "注册成功";

            // 注册成功后关闭注册面板
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("注册失败");
        }
    }
}
