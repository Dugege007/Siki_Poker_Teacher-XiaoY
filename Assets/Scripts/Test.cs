using System.Text;
using UnityEngine;

// �������ԵĽű�
public class Test : MonoBehaviour
{
    MsgTest msg = new MsgTest();

    private void Start()
    {
        msg.id = "123";
        //byte[] bytes = MsgBase.Encode(msg);
        byte[] bytes = MsgBase.EncodeName(msg);
        //Debug.Log(Encoding.UTF8.GetString(bytes));

        //MsgTest msgTest = MsgBase.Decode("MsgTest", bytes, 0, bytes.Length) as MsgTest;
        int count = 0;
        Debug.Log(MsgBase.DecodeName(bytes, 0, out count));
        Debug.Log(count);
    }
}

public class MsgTest : MsgBase
{
    // ���캯��
    public MsgTest()
    {
        // ��������ΪЭ����
        protoName = "MsgTest";
    }

    public string id = "";
}
