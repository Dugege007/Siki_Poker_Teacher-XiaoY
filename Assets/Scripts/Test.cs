using System.Text;
using UnityEngine;

// 用来测试的脚本
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
    // 构造函数
    public MsgTest()
    {
        // 用类名作为协议名
        protoName = "MsgTest";
    }

    public string id = "";
}
