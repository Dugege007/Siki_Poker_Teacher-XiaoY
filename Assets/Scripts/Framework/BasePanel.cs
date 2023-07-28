using UnityEngine;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 面板的资源路径
    /// 子类通过给这个变量赋值来指定面板的加载路径
    /// </summary>
    public string skinPath;

    /// <summary>
    /// 面板的游戏对象
    /// </summary>
    public GameObject skin;

    /// <summary>
    /// 面板的层级，用于控制面板的显示顺序
    /// </summary>
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    /// <summary>
    /// 初始化面板
    /// 加载面板的资源并实例化
    /// </summary>
    public void Init()
    {
        skin = Instantiate(Resources.Load<GameObject>(skinPath));
    }

    /// <summary>
    /// 面板初始化时调用
    /// 供子类重写
    /// </summary>
    public virtual void OnInit()
    {

    }

    /// <summary>
    /// 面板显示时调用
    /// 供子类重写
    /// </summary>
    public virtual void OnShow(params object[] para)
    {

    }

    /// <summary>
    /// 面板关闭时调用
    /// 供子类重写
    /// </summary>
    public virtual void OnClose()
    {

    }

    public void Close()
    {
        string name = GetType().Name;
        PanelManager.Close(name);
    }
}
