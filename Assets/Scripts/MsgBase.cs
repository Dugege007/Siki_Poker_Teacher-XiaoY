using System;
using System.Text;
using UnityEngine;

public class MsgBase
{
    /// <summary>
    /// Э����
    /// </summary>
    public string protoName = "";

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="msgBase">��Ϣ</param>
    /// <returns></returns>
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonUtility.ToJson(msgBase);
        return Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="protoName">Э����</param>
    /// <param name="bytes">�ֽ�����</param>
    /// <param name="offset">��ʼλ��</param>
    /// <param name="count">Ҫת�������</param>
    /// <returns></returns>
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = Encoding.UTF8.GetString(bytes, offset, count);
        return JsonUtility.FromJson(s, Type.GetType(protoName)) as MsgBase;
    }

    /// <summary>
    /// ����Э����
    /// </summary>
    /// <param name="msgBase">��Ϣ</param>
    /// <returns>����Э�������ֽ�����</returns>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(msgBase.protoName);
        short len = (short)nameBytes.Length;
        byte[] bytes = new byte[len + 2];   // ��������λ��Ϊ�˴泤��
        bytes[0] = (byte)(len % 256);       // ��Ϊһ���ֽ��� 8 λ��8 λ���Դ� 256 ������
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, bytes, 2, len);
        return bytes;
    }

    /// <summary>
    /// ����Э����
    /// </summary>
    /// <param name="bytes">Э�����ֽ�����</param>
    /// <param name="offset">��ʼλ��</param>
    /// <param name="count">������Э�����ĳ���</param>
    /// <returns>���ؽ�����Э����</returns>
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        if (offset + 2 > bytes.Length)
        {
            Debug.Log("��Ч��Ϣ");
            return "";
        }

        short len = (short)(bytes[offset + 1] * 256 + bytes[offset]);
        if (len <= 0)
        {
            Debug.Log("��Ч��Ϣ");
            return "";
        }

        count = len + 2;
        return Encoding.UTF8.GetString(bytes, offset + 2, len);
    }
}
