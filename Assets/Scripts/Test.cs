using UnityEngine;

// �������ԵĽű�
public class Test : MonoBehaviour
{
    private void Start()
    {
        // ���ӵ����ط������� 8888 �˿�
        NetManager.Connect("127.0.0.1", 8888);

        PanelManager.Init();
        PanelManager.Open<TipPanel>("Hello");
    }

    private void Update()
    {
        NetManager.Update();
    }
}
