using System.Net.Sockets;

#nullable disable
public class ClientState
{
    /// <summary>
    /// 客户端的 Socket 对象
    /// 用于与客户端进行网络通信
    /// </summary>
    public Socket socket;

    /// <summary>
    /// 客户端的字节数组
    /// 用于存储从客户端接收到的数据
    /// </summary>
    public ByteArray readBuff = new ByteArray();

    /// <summary>
    /// 记录最后一次收到 Ping 的时间
    /// </summary>
    public long lastPingTime = 0;   // 由于服务端可能会开启很长时间，所以这里使用较大的整型 long
}
