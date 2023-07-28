using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{
    /// <summary>
    /// ���Ĳ㼶ö��
    /// ���ڿ���������ʾ˳��
    /// </summary>
    public enum Layer
    {
        Panel,
        Tip,
    }

    /// <summary>
    /// �洢�����㼶�� Transform ���ֵ�
    /// </summary>
    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();

    /// <summary>
    /// �洢���������ֵ�
    /// </summary>
    private static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();

    /// <summary>
    /// ���������ĸ��ڵ�
    /// </summary>
    private static Transform rootTrans;

    /// <summary>
    /// ������ Transform
    /// </summary>
    private static Transform canvasTrans;

    /// <summary>
    /// ��ʼ��������������ȡ�����㼶�� Transform �������ֵ�
    /// </summary>
    public static void Init()
    {
        rootTrans = GameObject.Find("Root").transform;
        canvasTrans = rootTrans.Find("Canvas");
        layers.Add(Layer.Panel, canvasTrans.Find("Panel"));
        layers.Add(Layer.Tip, canvasTrans.Find("Tip"));
    }

    /// <summary>
    /// ��ָ�����͵����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="para">������</param>
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        // �Ƿ��Ѿ���
        string name = typeof(T).ToString();
        if (panels.ContainsKey(name))
            return;

        // ������岢��ʼ��
        BasePanel panel = rootTrans.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();

        // �������ĸ��ڵ�Ϊ��Ӧ�Ĳ㼶
        Transform layer = layers[panel.layer];
        panel.skin.transform.SetParent(layer, false);

        // �������ӵ��ֵ䲢��ʾ
        panels.Add(name, panel);
        panel.OnShow(para);

        if (para.Length >= 1)
            Debug.Log(para[0]);
    }

    /// <summary>
    /// �ر�ָ�����Ƶ����
    /// </summary>
    /// <param name="name">�������</param>
    public static void Close(string name)
    {
        // �Ƿ��Ѿ���
        if (!panels.ContainsKey(name))
            return;

        // ��ȡ��岢�ر�
        BasePanel panel = panels[name];
        panel.OnClose();

        // ���ֵ����Ƴ����������
        panels.Remove(name);
        GameObject.Destroy(panel.skin);
        GameObject.Destroy(panel);
    }
}
