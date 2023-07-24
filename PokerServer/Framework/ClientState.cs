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
}
