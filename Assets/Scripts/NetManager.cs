using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    /// <summary>
    /// 客户端 Socket
    /// </summary>
    private static Socket socket;

    /// <summary>
    /// 字节数组
    /// 用于存储接收到的数据
    /// </summary>
    private static ByteArray byteArray;

    /// <summary>
    /// 写入队列
    /// 用于存储待发送的数据
    /// </summary>
    private static Queue<ByteArray> writeQueue;

    /// <summary>
    /// 是否正在连接
    /// 用于防止在连接过程中再次发起连接
    /// </summary>
    private static bool isConnecting;

    /// <summary>
    /// 是否正在关闭
    /// 用于在关闭连接时等待所有待发送的数据发送完毕
    /// </summary>
    private static bool isClosing;

    /// <summary>
    /// 消息列表
    /// 客户端向服务端发送的消息列表
    /// </summary>
    private static List<MsgBase> msgList = new List<MsgBase>();

    /// <summary>
    /// 消息列表长度
    /// 由于子线程和主线程可能同时操作列表，直接用 msgList.Count 求出来的可能是错误的
    /// </summary>
    private static int msgCount = 0;

    /// <summary>
    /// 连接状态枚举
    /// </summary>
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }

    #region 事件相关
    /// <summary>
    /// 事件监听委托
    /// </summary>
    /// <param name="err">消息</param>
    public delegate void EventListener(string err);

    /// <summary>
    /// 事件监听列表
    /// 用于存储各种网络事件的监听器
    /// </summary>
    public static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    /// <summary>
    /// 添加事件监听器
    /// </summary>
    /// <param name="netEvent">连接状态</param>
    /// <param name="listener">事件监听</param>
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        // 如果已经存在该事件的监听器，则添加到已有的监听器上
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        // 否则，新建一个监听器
        else
        {
            eventListeners.Add(netEvent, listener);
        }
    }

    /// <summary>
    /// 删除事件监听器
    /// </summary>
    /// <param name="netEvent">连接状态</param>
    /// <param name="listener">事件监听</param>
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    /// <summary>
    /// 分发事件
    /// 调用对应事件的所有监听器
    /// </summary>
    /// <param name="netEvent">连接状态</param>
    /// <param name="err"></param>
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }
    #endregion

    #region 消息相关
    /// <summary>
    /// 消息监听委托
    /// </summary>
    /// <param name="msgBase">消息</param>
    public delegate void MsgListener(MsgBase msgBase);

    /// <summary>
    /// 消息监听列表
    /// 用于存储各种消息的监听器
    /// </summary>
    public static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    /// <summary>
    /// 添加消息监听器
    /// </summary>
    /// <param name="msgName">消息名称</param>
    /// <param name="listener">消息监听</param>
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        // 如果已经存在该消息的监听器，则添加到已有的监听器上
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        // 否则，新建一个监听器
        else
        {
            msgListeners.Add(msgName, listener);
        }
    }

    /// <summary>
    /// 删除消息监听器
    /// </summary>
    /// <param name="msgName">消息名称</param>
    /// <param name="listener">消息监听</param>
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }

    /// <summary>
    /// 分发消息
    /// 调用对应消息的所有监听器
    /// </summary>
    /// <param name="msgName">消息名称</param>
    /// <param name="msgBase">消息</param>
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }
    #endregion

    #region 连接相关
    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口号</param>
    public static void Connect(string ip, int port)
    {
        // 如果已经连接，则返回
        if (socket != null && socket.Connected)
        {
            Debug.Log("连接失败，已经连接过了");
            return;
        }

        // 如果正在连接，则返回
        if (isConnecting)
        {
            Debug.Log("正在连接中......");
            return;
        }

        // 如果一切正常
        // 初始化变量，并开始异步连接
        Init();
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallBack, socket);
    }

    /// <summary>
    /// 连接回调
    /// 连接成功或失败时调用
    /// </summary>
    /// <param name="ar">异步操作的信息</param>
    private static void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("连接成功");
            isConnecting = false;
            FireEvent(NetEvent.ConnectSucc, "");

            // 连接成功后，开始接收数据
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("连接失败：" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    /// <summary>
    /// 初始化
    /// 创建新的Socket和相关变量
    /// </summary>
    private static void Init()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byteArray = new ByteArray();
        writeQueue = new Queue<ByteArray>();
        isConnecting = false;
        isClosing = false;

        msgList = new List<MsgBase>();
        msgCount = 0;
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void Close()
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }

        // 如果还有待发送的数据，则等待数据发送完毕后再关闭
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        // 否则，直接关闭
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }
    #endregion

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    public static void Send(MsgBase msg)
    {
        // 检查连接状态
        if (socket == null || !socket.Connected)
            return;
        if (isConnecting)
            return;
        if (isClosing)
            return;

        // 编码消息
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.EncodeName(msg);

        int len = nameBytes.Length + bodyBytes.Length;
        // len + 2 前面留两个字节，来处理粘包和分包问题
        byte[] sendBytes = new byte[len + 2];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);

        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, nameBytes.Length);

        ByteArray byteArray = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(byteArray);
            count = writeQueue.Count;
        }
        if (count == 1)
        {
            // 发送
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }

    /// <summary>
    /// 发送回调
    /// 发送完成后调用
    /// </summary>
    /// <param name="ar">异步操作的信息</param>
    public static void SendCallback(IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        if (socket == null || !socket.Connected)
            return;
        int count = socket.EndSend(ar);

        ByteArray byteArray;
        lock (writeQueue)
        {
            byteArray = writeQueue.First();
        }

        byteArray.readIndex += count;

        // 如果 readIndex 和 writeIndex 重合了（也就是消息发送完了）
        if (byteArray.Length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                byteArray = writeQueue.First();
            }
        }

        // 如果没发送完毕
        if (byteArray != null)
        {
            // 继续发送
            socket.BeginSend(byteArray.bytes, byteArray.readIndex, byteArray.Length, 0, SendCallback, socket);
        }
        // 是否正在关闭
        else if (isClosing)
        {
            socket.Close();
        }
    }

    /// <summary>
    /// 接收回调
    /// </summary>
    /// <param name="ar">异步操作的信息</param>
    public static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndSend(ar);

            // 断开连接
            if (count == 0)
            {
                Close();
                return;
            }

            // 成功接收
            byteArray.writeIndex = count;

            // 消息处理
            OnReceiveData();
            // 如果 byteArray 长度过小，进行扩容
            if (byteArray.Remain < 8)
            {
                byteArray.MoveBytes();
                byteArray.ReSize(byteArray.Length * 2);
            }
            // 继续接收消息
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("接收失败" + ex.ToString());
        }
    }

    /// <summary>
    /// 处理接收来的消息
    /// </summary>
    public static void OnReceiveData()
    {

    }
}
