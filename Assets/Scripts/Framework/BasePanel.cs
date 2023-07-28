using UnityEngine;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// ������Դ·��
    /// ����ͨ�������������ֵ��ָ�����ļ���·��
    /// </summary>
    public string skinPath;

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public GameObject skin;

    /// <summary>
    /// ���Ĳ㼶�����ڿ���������ʾ˳��
    /// </summary>
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    /// <summary>
    /// ��ʼ�����
    /// ����������Դ��ʵ����
    /// </summary>
    public void Init()
    {
        skin = Instantiate(Resources.Load<GameObject>(skinPath));
    }

    /// <summary>
    /// ����ʼ��ʱ����
    /// ��������д
    /// </summary>
    public virtual void OnInit()
    {

    }

    /// <summary>
    /// �����ʾʱ����
    /// ��������д
    /// </summary>
    public virtual void OnShow(params object[] para)
    {

    }

    /// <summary>
    /// ���ر�ʱ����
    /// ��������д
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
