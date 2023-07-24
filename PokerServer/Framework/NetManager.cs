using System.Net;
using System.Net.Sockets;

#nullable disable
public static class NetManager
{
    /// <summary>
    /// 服务端的 Socket 对象
    /// 用于监听客户端的连接请求
    /// </summary>
    public static Socket listenfd;

    /// <summary>
    /// 存储所有客户端的字典
    /// 键为客户端的 Socket 对象，值为客户端的状态信息
    /// </summary>
    public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    /// <summary>
    /// 用于检测的 Socket 列表
    /// 存储所有需要进行状态检测的 Socket 对象
    /// </summary>
    private static List<Socket> sockets = new List<Socket>();

    /// <summary>
    /// 启动服务端
    /// 并开始监听客户端的连接请求
    /// </summary>
    /// <param name="ip">服务端的 IP 地址</param>
    /// <param name="port">服务端的端口号</param>
    public static void Connect(string ip, int port)
    {
        // 创建服务端的 Socket 对象，并绑定 IP 地址和端口号
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress iPAddress = IPAddress.Parse(ip);
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
        listenfd.Bind(iPEndPoint);
        listenfd.Listen(0); // 0 表示无限制 

        Console.WriteLine("[服务器] 启动成功");

        // 循环监听客户端的连接请求和消息
        while (true)
        {
            // 填充需要检测的 Socket 列表
            sockets.Clear();
            sockets.Add(listenfd);
            foreach (ClientState c in clients.Values)
            {
                sockets.Add(c.socket);
            }

            // 检测 Socket 的状态
            Socket.Select(sockets, null, null, 1000);

            // 处理每个 Socket 的状态
            for (int i = 0; i < sockets.Count; i++)
            {
                Socket s = sockets[i];
                if (s==listenfd)
                {
                    // 如果是服务端的 Socket，处理新的客户端连接请求
                }
                else
                {
                    // 如果是客户端的 Socket，处理客户端发送的消息
                }
            }
        }
    }
}
