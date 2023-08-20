using System.Collections.Generic;

public class CardManager
{
    /// <summary>
    /// ���� �����������
    /// </summary>
    public enum CardType
    {
        One,                // ����     3
        Two,                // ����     33
        Three,              // ����     333
        ThreeWithOne,       // ����һ   333 4
        ThreeWithTwo,       // ������   333 44
        Chain,              // ˳��     3 4 5 6 7
        PairChain,          // ����     33 44 55
        Airplane,           // �ɻ�     333 444
        AirplaneWithOne,    // �ɻ����� 333 444 5 6
        AirplaneWithTwo,    // �ɻ����� 333 444 55 66
        Bomb,               // ը��     3333
        FourWithTwo,        // �Ĵ���   3333 44
        JokerBomb,          // ��ը     SJoker LJoker
        Wrong               // ��������
    }

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

    /// <summary>
    /// Card ����ת CardInfo ����
    /// </summary>
    /// <param name="cards">��������</param>
    /// <returns>������Ϣ����</returns>
    public static CardInfo[] GetCardInfos(Card[] cards)
    {
        CardInfo[] infos = new CardInfo[cards.Length];
        for (int i = 0; i < infos.Length; i++)
        {
            infos[i] = cards[i].GetCardInfo();
        }

        return infos;
    }

    /// <summary>
    /// CardInfo ����ת Card ����
    /// </summary>
    /// <param name="cardsInfo">������Ϣ����</param>
    /// <returns>��������</returns>
    public static Card[] GetCards(CardInfo[] cardsInfo)
    {
        Card[] cards = new Card[cardsInfo.Length];
        for (int i = 0; i < cardsInfo.Length; i++)
        {
            cards[i] = new Card(cardsInfo[i].suit, cardsInfo[i].rank);
        }

        return cards;
    }
}
