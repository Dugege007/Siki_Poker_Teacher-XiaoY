using System;
using System.Text;
using UnityEngine;

public class MsgBase
{
    /// <summary>
    /// 协议名
    /// </summary>
    public string protoName = "";

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="msgBase">消息</param>
    /// <returns></returns>
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonUtility.ToJson(msgBase);
        return Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="protoName">协议名</param>
    /// <param name="bytes">字节数组</param>
    /// <param name="offset">起始位置</param>
    /// <param name="count">要转码的数量</param>
    /// <returns></returns>
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = Encoding.UTF8.GetString(bytes, offset, count);
        return JsonUtility.FromJson(s, Type.GetType(protoName)) as MsgBase;
    }

    /// <summary>
    /// 编码协议名
    /// </summary>
    /// <param name="msgBase">消息</param>
    /// <returns>返回协议名的字节数组</returns>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(msgBase.protoName);
        short len = (short)nameBytes.Length;
        byte[] bytes = new byte[len + 2];   // 多留两个位置为了存长度
        bytes[0] = (byte)(len % 256);       // 因为一个字节是 8 位，8 位可以存 256 个数字
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, bytes, 2, len);
        return bytes;
    }

    /// <summary>
    /// 解码协议名
    /// </summary>
    /// <param name="bytes">协议名字节数组</param>
    /// <param name="offset">开始位置</param>
    /// <param name="count">解码后的协议名的长度</param>
    /// <returns>返回解码后的协议名</returns>
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        if (offset + 2 > bytes.Length)
        {
            Debug.Log("无效信息");
            return "";
        }

        short len = (short)(bytes[offset + 1] * 256 + bytes[offset]);
        if (len <= 0)
        {
            Debug.Log("无效信息");
            return "";
        }

        count = len + 2;
        return Encoding.UTF8.GetString(bytes, offset + 2, len);
    }
}
