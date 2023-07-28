using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{
    /// <summary>
    /// 面板的层级枚举
    /// 用于控制面板的显示顺序
    /// </summary>
    public enum Layer
    {
        Panel,
        Tip,
    }

    /// <summary>
    /// 存储各个层级的 Transform 的字典
    /// </summary>
    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();

    /// <summary>
    /// 存储所有面板的字典
    /// </summary>
    private static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();

    /// <summary>
    /// 面板管理器的根节点
    /// </summary>
    private static Transform rootTrans;

    /// <summary>
    /// 画布的 Transform
    /// </summary>
    private static Transform canvasTrans;

    /// <summary>
    /// 初始化面板管理器，获取各个层级的 Transform 并存入字典
    /// </summary>
    public static void Init()
    {
        rootTrans = GameObject.Find("Root").transform;
        canvasTrans = rootTrans.Find("Canvas");
        layers.Add(Layer.Panel, canvasTrans.Find("Panel"));
        layers.Add(Layer.Tip, canvasTrans.Find("Tip"));
    }

    /// <summary>
    /// 打开指定类型的面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="para">面板对象</param>
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        // 是否已经打开
        string name = typeof(T).ToString();
        if (panels.ContainsKey(name))
            return;

        // 创建面板并初始化
        BasePanel panel = rootTrans.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();

        // 设置面板的父节点为对应的层级
        Transform layer = layers[panel.layer];
        panel.skin.transform.SetParent(layer, false);

        // 将面板添加到字典并显示
        panels.Add(name, panel);
        panel.OnShow(para);

        if (para.Length >= 1)
            Debug.Log(para[0]);
    }

    /// <summary>
    /// 关闭指定名称的面板
    /// </summary>
    /// <param name="name">面板名称</param>
    public static void Close(string name)
    {
        // 是否已经打开
        if (!panels.ContainsKey(name))
            return;

        // 获取面板并关闭
        BasePanel panel = panels[name];
        panel.OnClose();

        // 从字典中移除并销毁面板
        panels.Remove(name);
        GameObject.Destroy(panel.skin);
        GameObject.Destroy(panel);
    }
}
