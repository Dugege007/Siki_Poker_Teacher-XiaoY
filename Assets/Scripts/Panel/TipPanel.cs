using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    // 提示面板中的文本组件
    private Text text;

    private void Start()
    {
        // 开始执行提示面板的动画效果
        StartCoroutine(Effect());
    }

    // 在面板初始化时设置面板的资源路径和层级
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    // 在面板显示时获取组件并设置提示文本
    public override void OnShow(params object[] para)
    {
        text = skin.transform.Find("Text").GetComponent<Text>();

        if (para.Length >= 1)
        {
            string message = (string)para[0];
            text.text = message;

            // 根据消息内容设置字体颜色
            if (message.Contains("成功"))
            {
                // 积极的消息显示为绿色
                text.color = Color.black;
            }
            else if (message.Contains("失败")
                || message.Contains("不")
                || message.Contains("断开"))
            {
                // 消极的消息显示为红色
                text.color = Color.red;
            }
            else
            {
                // 其他消息显示为白色
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
