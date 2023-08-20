
public class CardManager
{
    /// <summary>
    /// �����б�
    /// </summary>
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// �����������
    /// </summary>
    public enum CardType
    {
        One,                // ����     3
        Two,                // ����     33
        Three,              // ����     333
        ThreeWithOne,       // ����һ   333 4
        ThreeWithWith,      // ������   333 44
        Airplane,           // �ɻ�     333 444
        AirplaneWithOne,    // �ɻ����� 333 444 5 6
        AirplaneWithTwo,    // �ɻ����� 333 444 55 66
        Chain,              // ˳��     3 4 5 6
        PairChain,          // ����     33 44 55
        Bomb,               // ը��     3333
        FourWithTwo,        // �Ĵ���   3333 44
        JokerBomb,          // ��ը     SJoker LJoker
        Wrong               // ��������
    }

    public CardType GetCardType(Card[] cards)
    {
        int[] rank = new int[20];
        int len = cards.Length;

        for (int i = 0; i < len; i++)
        {
            rank[i] = (int)cards[i].rank;
        }

        // ������
        // ֻ�Ŵ���� cards �ĳ��ȣ���Ϊ���涼�� 0
        Array.Sort(rank, 0, len);
        // ����Ĭ��ֵ
        CardType cardType = CardType.Wrong;
        if (len == 1)
        {
            cardType = CardType.One;
        }
        if (len == 2)
        {
            if (rank[0] == rank[1])
                cardType = CardType.Two;
            if (rank[0] + rank[1] == 27)
                cardType = CardType.JokerBomb;
        }

        return cardType;
    }

    /// <summary>
    /// ϴ��
    /// </summary>
    public static void Shuffle()
    {
        // ��տ����б���������ϴ��ʱ���һ����
        cards.Clear();

        // ����һ���������˿��ƣ����� 4 �ֻ�ɫ��ÿ�ֻ�ɫ 13 ����
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card card = new Card(i, j);
                cards.Add(card);
            }
        }

        // ��Ӵ�С��
        cards.Add(new Card(Suit.None, Rank.SJoker));
        cards.Add(new Card(Suit.None, Rank.LJoker));

        // ����һ������������ʱ�洢ϴ�ƹ����еĿ���
        Queue<Card> cardQueue = new Queue<Card>();

        // ���ѡ��һ�ſ��ƣ������ԭ�б����Ƴ���Ȼ����ӵ�������
        for (int i = 0; i < 54; i++)
        {
            Random random = new Random();
            int index = random.Next(0, cards.Count);
            cardQueue.Enqueue(cards[index]);
            cards.RemoveAt(index);
        }

        // �������еĿ���������ӵ��б��У����ϴ��
        for (int i = 0; i < 54; i++)
        {
            cards.Add(cardQueue.Dequeue());
        }
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
}
