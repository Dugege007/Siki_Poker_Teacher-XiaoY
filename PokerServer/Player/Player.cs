#nullable disable
public class Player
{
    /// <summary>
    /// 玩家 ID
    /// </summary>
    public string id = "";

    /// <summary>
    /// 玩家对应的客户端
    /// </summary>
    public ClientState state;

    /// <summary>
    /// 玩家数据
    /// </summary>
    public PlayerData data;

    /// <summary>
    /// 是否是房主
    /// </summary>
    public bool isHost = false;

    /// <summary>
    /// 所在房间号，-1 表示不在房间中
    /// </summary>
    public int roomID = -1;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="state">玩家对应的客户端</param>
    public Player(ClientState state)
    {
        this.state = state;
    }

    /// <summary>
    /// 封装的发送消息
    /// </summary>
    /// <param name="msgBase">消息</param>
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
}
