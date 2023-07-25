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
                if (s == listenfd)
                {
                    // 如果是服务端的 Socket，处理新的客户端连接请求
                    Accept(s);
                }
                else
                {
                    // 如果是客户端的 Socket，处理客户端发送的消息
                }
            }
        }
    }

    /// <summary>
    /// 接收客户端 Socket
    /// 处理新的客户端连接请求
    /// </summary>
    /// <param name="listenfd">服务端 Socket</param>
    public static void Accept(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd;
            Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Accept Fail " + ex.ToString());
        }
    }

    /// <summary>
    /// 服务端接收消息
    /// 接收并处理客户端发送的消息
    /// </summary>
    /// <param name="clientfd">发信息的客户端 Socket</param>
    public static void Receive(Socket clientfd)
    {
        ClientState state = clients[clientfd];
        ByteArray readBuff = state.readBuff;

        int count = 0;
        if (readBuff.Remain <= 0)
        {
            readBuff.MoveBytes();
        }
        if (readBuff.Remain <= 0)
        {
            Console.WriteLine("Receive Fall " + "数组长度不足");
            Close(state);
            return;
        }

        try
        {
            count = clientfd.Receive(readBuff.bytes, readBuff.writeIndex, readBuff.Remain, 0);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Receive Fail " + ex.ToString());
            Close(state);
            return;
        }

        // 关闭
        if (count <= 0)
        {
            Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
            Close(state);
            return;
        }

        readBuff.writeIndex += count;

        // 解码
        OnReceiveData(state);
        readBuff.MoveBytes();
    }

    /// <summary>
    /// 关闭与指定客户端的连接
    /// </summary>
    /// <param name="state">客户端的状态信息</param>
    public static void Close(ClientState state)
    {
        state.socket.Close();
        clients.Remove(state.socket);
    }

    /// <summary>
    /// 处理客户端发送的消息
    /// </summary>
    /// <param name="state">客户端的状态信息</param>
    public static void OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuff;
        byte[] bytes = readBuff.bytes;

        // 如果接收的消息长度不足以解析，直接返回
        if (readBuff.Length <= 2)
            return;
        // 解析消息体长度
        short bodyLength = (short)(bytes[readBuff.readIndex + 1] * 256 + bytes[readBuff.readIndex]);
        // 如果接收的消息长度小于消息体长度，直接返回
        if (readBuff.Length < bodyLength)
            return;
        // 跳过消息体长度字段
        readBuff.readIndex += 2;

        // 解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIndex, out nameCount);
        // 如果协议名解析失败，关闭连接并返回
        if (protoName == "")
        {
            Console.WriteLine("OnReceiveData Fail " + "解析协议名失败");
            Close(state);
            return;
        }

        // 解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIndex, bodyCount);
        // 跳过协议体字段
        readBuff.readIndex += bodyCount;
        // 移动剩余的消息到数组开头
        readBuff.MoveBytes();

        // 分发消息

        // 如果还有未处理的消息，继续处理
        if (readBuff.Length > 2)
        {
            OnReceiveData(state);
        }
    }

    /// <summary>
    /// 向客户端发送数据
    /// </summary>
    /// <param name="cs">目标客户端状态</param>
    /// <param name="msgBase">需要发送的消息</param>
    public static void Send(ClientState cs, MsgBase msgBase)
    {
        // 如果客户端状态为空或者客户端未连接，直接返回
        if (cs == null || cs.socket.Connected)
            return;

        // 对协议名进行编码
        byte[] nameBytes = MsgBase.EncodeName(msgBase);
        // 对协议体进行编码
        byte[] bodyBytes = MsgBase.Encode(msgBase);

        // 计算发送数据的总长度
        int len = nameBytes.Length + bodyBytes.Length;
        // 创建发送数据的字节数组，长度为数据总长度加上2字节的长度字段
        byte[] sendBytes = new byte[len + 2];
        // 将长度信息写入发送数据的前两个字节
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);

        // 将协议名和协议体的字节数组复制到发送数据的字节数组
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        // 尝试发送数据，如果发送失败则打印错误信息
        try
        {
            cs.socket.Send(sendBytes, 0, sendBytes.Length, 0);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Send Fail " + ex.ToString());
        }
    }
}
