using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    // ��ʾ����е��ı����
    private Text text;

    private void Start()
    {
        // ��ʼִ����ʾ���Ķ���Ч��
        StartCoroutine(Effect());
    }

    // ������ʼ��ʱ����������Դ·���Ͳ㼶
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    // �������ʾʱ��ȡ�����������ʾ�ı�
    public override void OnShow(params object[] para)
    {
        text = skin.transform.Find("Text").GetComponent<Text>();

        if (para.Length >= 1)
        {
            string message = (string)para[0];
            text.text = message;

            // ������Ϣ��������������ɫ
            if (message.Contains("�ɹ�"))
            {
                // ��������Ϣ��ʾΪ��ɫ
                text.color = Color.black;
            }
            else if (message.Contains("ʧ��")
                || message.Contains("��")
                || message.Contains("�Ͽ�"))
            {
                // ��������Ϣ��ʾΪ��ɫ
                text.color = Color.red;
            }
            else
            {
                // ������Ϣ��ʾΪ��ɫ
                text.color = Color.white;
            }
        }
    }

    public override void OnClose()
    {

    }

    private IEnumerator Effect()
    {
        for (int i = 0; i < 100; i++)
        {
            skin.transform.position += Vector3.up * 0.5f;
            text.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        Close();
    }
}
