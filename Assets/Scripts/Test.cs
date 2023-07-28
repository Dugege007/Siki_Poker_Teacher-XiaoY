using UnityEngine;

// 用来测试的脚本
public class Test : MonoBehaviour
{
    private void Start()
    {
        // 连接到本地服务器的 8888 端口
        NetManager.Connect("127.0.0.1", 8888);

        PanelManager.Init();
        PanelManager.Open<TipPanel>("Hello");
    }

    private void Update()
    {
        NetManager.Update();
    }
}
