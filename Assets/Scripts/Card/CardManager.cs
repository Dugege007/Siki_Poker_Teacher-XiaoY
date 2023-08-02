using System.Collections.Generic;

public class CardManager
{
    /// <summary>
    /// �˿����ֵ�
    /// ��Ϊ�Ƶ����ƣ�ֵΪ�ƵĶ���
    /// </summary>
    public static Dictionary<string, Card> nameCards = new Dictionary<string, Card>();

    /// <summary>
    /// ��ʼ���˿���
    /// </summary>
    public static void Init()
    {
        // ����һ���������˿��ƣ����� 4 �ֻ�ɫ��ÿ�ֻ�ɫ 13 ����
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                // ����һ����
                Card card = new Card(i, j);
                // �����Ƶ����ƣ��ɻ�ɫ�͵������
                string name = ((Suit)i).ToString() + ((Rank)j).ToString();
                // ���Ƶ����ƺ��ƵĶ�����ӵ��ֵ���
                nameCards.Add(name, card);
            }
        }

        // ��Ӵ�С��
        nameCards.Add("SJoker", new Card(Suit.None, Rank.SJoker));
        nameCards.Add("LJoker", new Card(Suit.None, Rank.LJoker));
    }

    /// <summary>
    /// �����ƵĶ����ȡ�Ƶ�����
    /// </summary>
    /// <param name="card">�ƵĶ���</param>
    /// <returns>�Ƶ�����</returns>
    public static string GetName(Card card)
    {
        // �����ֵ�ļ������Ƶ�����
        foreach (string name in nameCards.Keys)
        {
            // ����ֵ��е��ƵĻ�ɫ�͵���������е��ƵĻ�ɫ�͵�����ͬ���򷵻��Ƶ�����
            if (nameCards[name].suit == card.suit && nameCards[name].rank == card.rank)
            {
                return name;
            }
        }

        // ���û���ҵ�ƥ����ƣ��򷵻ؿ��ַ���
        return "";
    }

    /// <summary>
    /// ���ݿ������ƻ�ȡ����
    /// </summary>
    /// <param name="name">��������</param>
    /// <returns>���ƶ���</returns>
    public static Card GetCard(string name)
    {
        if (nameCards.ContainsKey(name))
            return nameCards[name];

        return null;
    }
}
