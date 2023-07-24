using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    /// <summary>
    /// �ͻ��� Socket
    /// </summary>
    private static Socket socket;

    /// <summary>
    /// �ֽ�����
    /// ���ڴ洢���յ�������
    /// </summary>
    private static ByteArray byteArray;

    /// <summary>
    /// д�����
    /// ���ڴ洢�����͵�����
    /// </summary>
    private static Queue<ByteArray> writeQueue;

    /// <summary>
    /// �Ƿ���������
    /// ���ڷ�ֹ�����ӹ������ٴη�������
    /// </summary>
    private static bool isConnecting;

    /// <summary>
    /// �Ƿ����ڹر�
    /// �����ڹر�����ʱ�ȴ����д����͵����ݷ������
    /// </summary>
    private static bool isClosing;

    /// <summary>
    /// ��Ϣ�б�
    /// �ͻ��������˷��͵���Ϣ�б�
    /// </summary>
    private static List<MsgBase> msgList = new List<MsgBase>();

    /// <summary>
    /// ��Ϣ�б���
    /// �������̺߳����߳̿���ͬʱ�����б�ֱ���� msgList.Count ������Ŀ����Ǵ����
    /// </summary>
    private static int msgCount = 0;

    /// <summary>
    /// ÿһ֡�д���������Ϣ����
    /// ��������ÿ֡�������Ϣ��������ֹ���������Ϣ���µ���Ϸ����
    /// </summary>
    private static int processMsgCount = 10;

    /// <summary>
    /// �Ƿ�������������
    /// </summary>
    public static bool isUsePing = true;

    /// <summary>
    /// �������
    /// </summary>
    public static int pingInterval = 30;

    /// <summary>
    /// ��һ�η��� Ping Э���ʱ��
    /// </summary>
    private static float lastPingTime = 0;

    /// <summary>
    /// ��һ���յ� Pong Э���ʱ��
    /// </summary>
    private static float lastPongTime = 0;

    /// <summary>
    /// ����״̬ö��
    /// </summary>
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }

    #region �¼����
    /// <summary>
    /// �¼�����ί��
    /// </summary>
    /// <param name="err">��Ϣ</param>
    public delegate void EventListener(string err);

    /// <summary>
    /// �¼������б�
    /// ���ڴ洢���������¼��ļ�����
    /// </summary>
    public static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    /// <summary>
    /// ����¼�������
    /// </summary>
    /// <param name="netEvent">����״̬</param>
    /// <param name="listener">�¼�����</param>
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        // ����Ѿ����ڸ��¼��ļ�����������ӵ����еļ�������
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        // �����½�һ��������
        else
        {
            eventListeners.Add(netEvent, listener);
        }
    }

    /// <summary>
    /// ɾ���¼�������
    /// </summary>
    /// <param name="netEvent">����״̬</param>
    /// <param name="listener">�¼�����</param>
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
    /// �ַ��¼�
    /// ���ö�Ӧ�¼������м�����
    /// </summary>
    /// <param name="netEvent">����״̬</param>
    /// <param name="err"></param>
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }
    #endregion

    #region ��Ϣ���
    /// <summary>
    /// ��Ϣ����ί��
    /// </summary>
    /// <param name="msgBase">��Ϣ</param>
    public delegate void MsgListener(MsgBase msgBase);

    /// <summary>
    /// ��Ϣ�����б�
    /// ���ڴ洢������Ϣ�ļ�����
    /// </summary>
    public static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    /// <summary>
    /// �����Ϣ������
    /// </summary>
    /// <param name="msgName">��Ϣ����</param>
    /// <param name="listener">��Ϣ����</param>
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        // ����Ѿ����ڸ���Ϣ�ļ�����������ӵ����еļ�������
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        // �����½�һ��������
        else
        {
            msgListeners.Add(msgName, listener);
        }
    }

    /// <summary>
    /// ɾ����Ϣ������
    /// </summary>
    /// <param name="msgName">��Ϣ����</param>
    /// <param name="listener">��Ϣ����</param>
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
    /// �ַ���Ϣ
    /// ���ö�Ӧ��Ϣ�����м�����
    /// </summary>
    /// <param name="msgName">��Ϣ����</param>
    /// <param name="msgBase">��Ϣ</param>
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }
    #endregion

    #region �������
    /// <summary>
    /// ���ӷ�����
    /// </summary>
    /// <param name="ip">IP ��ַ</param>
    /// <param name="port">�˿ں�</param>
    public static void Connect(string ip, int port)
    {
        // ����Ѿ����ӣ��򷵻�
        if (socket != null && socket.Connected)
        {
            Debug.Log("����ʧ�ܣ��Ѿ����ӹ���");
            return;
        }

        // ����������ӣ��򷵻�
        if (isConnecting)
        {
            Debug.Log("����������......");
            return;
        }

        // ���һ������
        // ��ʼ������������ʼ�첽����
        Init();
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallBack, socket);
    }

    /// <summary>
    /// ���ӻص�
    /// ���ӳɹ���ʧ��ʱ����
    /// </summary>
    /// <param name="ar">�첽��������Ϣ</param>
    private static void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("���ӳɹ�");
            isConnecting = false;
            FireEvent(NetEvent.ConnectSucc, "");

            // ���ӳɹ��󣬿�ʼ��������
            // ���￪ʼ�첽�������ݣ��������ݵ���ʱ�����Զ�����ReceiveCallBack����
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ�ܣ�" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    /// <summary>
    /// ��ʼ��
    /// �����µ�Socket����ر���
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

        lastPingTime = Time.time;
        lastPongTime = Time.time;

        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    /// <summary>
    /// �ر�����
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

        // ������д����͵����ݣ���ȴ����ݷ�����Ϻ��ٹر�
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        // ����ֱ�ӹر�
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }
    #endregion

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="msg">��Ϣ</param>
    public static void Send(MsgBase msg)
    {
        // �������״̬
        if (socket == null || !socket.Connected)
            return;
        if (isConnecting)
            return;
        if (isClosing)
            return;

        // ������Ϣ
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.EncodeName(msg);

        int len = nameBytes.Length + bodyBytes.Length;
        // len + 2 ǰ���������ֽڣ�������ճ���ͷְ�����
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
            // ����
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }

    /// <summary>
    /// ���ͻص�
    /// ������ɺ����
    /// </summary>
    /// <param name="ar">�첽��������Ϣ</param>
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

        // ��� readIndex �� writeIndex �غ��ˣ�Ҳ������Ϣ�������ˣ�
        if (byteArray.Length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                byteArray = writeQueue.First();
            }
        }

        // ���û�������
        if (byteArray != null)
        {
            // ��������
            socket.BeginSend(byteArray.bytes, byteArray.readIndex, byteArray.Length, 0, SendCallback, socket);
        }
        // �Ƿ����ڹر�
        else if (isClosing)
        {
            socket.Close();
        }
    }

    /// <summary>
    /// ���ջص�
    /// </summary>
    /// <param name="ar">�첽��������Ϣ</param>
    public static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndSend(ar);

            // ������յ������ݳ���Ϊ0��˵���Է��Ѿ��ر����ӣ���ʱӦ�ر��Լ�������
            if (count == 0)
            {
                Close();
                return;
            }

            // �ɹ����յ����ݣ�����д��λ��
            byteArray.writeIndex = count;
            // ������յ�������
            OnReceiveData();

            // ���ʣ��ռ䲻�㣬�ƶ����ݲ�����
            if (byteArray.Remain < 8)
            {
                byteArray.MoveBytes();
                byteArray.ReSize(byteArray.Length * 2);
            }

            // �����������ݣ��������ݵ���ʱ�����Զ�����ReceiveCallBack����
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ��" + ex.ToString());
        }
    }

    /// <summary>
    /// �������������Ϣ
    /// </summary>
    public static void OnReceiveData()
    {
        // ������ݳ���С��2��˵�����ݲ��������ȴ����ո�������
        if (byteArray.Length <= 2)
            return;

        int readIndex = byteArray.readIndex;
        byte[] bytes = byteArray.bytes;

        // ��ȡ���ݳ���
        // bytes[readIndex] % 256   bytes[readIndex] / 256
        short bodyLength = (short)(bytes[readIndex + 1] * 256 + bytes[readIndex]);

        // ������ݳ���С����Ϣ���ȣ�˵�����ݲ��������ȴ����ո�������
        if (byteArray.Length < bodyLength + 2)
            return;

        // ������Ϣ��
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(byteArray.bytes, byteArray.readIndex, out nameCount);

        if (protoName == "")
        {
            Debug.Log("����ʧ��");
            return;
        }
        byteArray.readIndex += nameCount;

        // ������Ϣ��
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, byteArray.bytes, byteArray.readIndex, bodyCount);
        byteArray.readIndex += bodyCount;
        byteArray.MoveBytes();

        // ������������Ϣ��ӵ���Ϣ�б���
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCount++;

        // ��� byteArray ���ȴ��� 2��˵�����滹����Ϣ��Ӧ�ü�������
        if (byteArray.Length > 2)
        {
            OnReceiveData();
        }
    }

    /// <summary>
    /// ��Ϣ����
    /// ����Ϣ�б���ȡ����������Ϣ��ÿ֡�������Ϣ���������� processMsgCount
    /// </summary>
    public static void MsgUpdate()
    {
        // ���û�д��������Ϣ��ֱ�ӷ���
        if (msgCount == 0)
            return;

        for (int i = 0; i < processMsgCount; i++)
        {
            // ÿ֡���� processMsgCount ����Ϣ
            MsgBase msgBase = null;

            // ����Ϣ�б���ȡ��һ����Ϣ
            lock (msgList)
            {
                if (msgCount > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }

            // ���ȡ������Ϣ��Ϊ�գ��������Ϣ
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            }
            else
            {
                // �����ϢΪ�գ�˵����Ϣ�Ѿ������꣬����ѭ��
                break;
            }
        }
    }

    /// <summary>
    /// ���� Ping Э��
    /// </summary>
    private static void PingUpdate()
    {
        if (!isUsePing)
            return;

        // ������﷢�ͼ����������Ϣ
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msg = new MsgPing();
            Send(msg);
            lastPingTime = Time.time;
        }

        // ��� pingInterval * 4 ʱ���ڽ��ղ��� Pong �ͶϿ�
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }

    /// <summary>
    /// ÿ֡���·���
    /// ����Ϸ��ÿһ֡�е��ã����ڴ����յ�����Ϣ
    /// ע�⣺���������Ҫ������Ϸѭ������ĳ�� MonoBehaviour �� Update �����е���
    /// </summary>
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    /// <summary>
    /// ���յ� Pong ��Ϣ��
    /// </summary>
    /// <param name="msgBase">��Ϣ</param>
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}
