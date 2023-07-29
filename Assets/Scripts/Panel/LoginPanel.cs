using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    // 定义登录面板中的输入字段和按钮
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button registBtn;

    // 在面板初始化时设置面板的资源路径和层级
    public override void OnInit()
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;

        NetManager.Connect("127.0.0.1", 8888);
    }

    // 在面板显示时获取组件并添加事件监听
    public override void OnShow(params object[] para)
    {
        // 获取输入字段和按钮的组件
        idInput = skin.transform.Find("InputBox/UserNameText/InputField").GetComponent<InputField>();
        pwInput = skin.transform.Find("InputBox/PasswordText/InputField").GetComponent<InputField>();
        loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        registBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();

        // 为按钮添加点击事件监听
        loginBtn.onClick.AddListener(OnLoginClick);
        registBtn.onClick.AddListener(OnRegisterClick);

        // 添加网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectFail);
        // 添加网络消息监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    // 在面板关闭时执行的操作
    public override void OnClose()
    {
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectFail);

        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
    }

    // 登录按钮点击事件处理
    public void OnLoginClick()
    {
        // 如果输入字段为空则不进行操作
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }

        // 创建登录消息并发送
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = idInput.text;
        msgLogin.pw = pwInput.text;
        NetManager.Send(msgLogin);
    }

    // 注册按钮点击事件处理
    public void OnRegisterClick()
    {
        // 打开注册面板
        PanelManager.Open<RegisterPanel>();
    }

    // 处理登录消息的回调
    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if (msg.result)
        {
            // 登录成功
            PanelManager.Open<TipPanel>("登录成功");
            PanelManager.Open<RoomListPanel>();
            GameManager.id = msg.id;
            Close();
        }
        else
        {
            // 登录失败
            PanelManager.Open<TipPanel>("登录失败");
        }
    }

    public void OnConnectSucc(string err)
    {
        // 连接成功
        PanelManager.Open<TipPanel>("连接成功");
    }

    public void OnConnectFail(string err)
    {
        // 连接失败
        PanelManager.Open<TipPanel>(err);
    }
}
