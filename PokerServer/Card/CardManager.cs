
using System.ComponentModel.DataAnnotations;

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
        ThreeWithTwo,      // ������   333 44
        Airplane,           // �ɻ�     333 444
        AirplaneWithOne,    // �ɻ����� 333 444 5 6
        AirplaneWithTwo,    // �ɻ����� 333 444 55 66
        Chain,              // ˳��     3 4 5 6 7
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

        switch (len)
        {
            case 1:
                // ����
                cardType = CardType.One;
                break;

            case 2:
                // ����
                if (rank[0] == rank[1])
                    cardType = CardType.Two;
                // ��ը
                if (rank[0] + rank[1] == 27)
                    cardType = CardType.JokerBomb;
                break;

            case 3:
                // ����
                if (rank[0] == rank[1] && rank[0] == rank[2])
                    cardType = CardType.Three;
                break;

            case 4:
                // ը��
                if (rank[0] == rank[1] && rank[0] == rank[2] && rank[0] == rank[3])
                    cardType = CardType.Bomb;
                // ����һ
                else if (rank[0] == rank[1] && rank[0] == rank[2])
                    cardType = CardType.ThreeWithOne;
                else if (rank[1] == rank[2] && rank[1] == rank[3])
                    cardType = CardType.ThreeWithOne;
                break;

            case 5:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // ������ ��֧�ֶ���
                else if (rank[0] == rank[1] && rank[0] == rank[2] && rank[3] == rank[4])
                    cardType = CardType.ThreeWithTwo;
                else if (rank[0] == rank[1] && rank[2] == rank[3] && rank[2] == rank[4])
                    cardType = CardType.ThreeWithTwo;
                break;

            case 6:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 7:
                break;

            case 8:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 9:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                break;

            case 10:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 11:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                break;

            case 12:
                // ˳��
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 14:
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 16:
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 18:
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 20:
                // ����
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            default:
                cardType = CardType.Wrong;
                break;
        }

        return cardType;
    }

    // ���˳��
    private bool CheckChain(int[] rank, int len)
    {
        bool result = true;
        for (int i = 0; i < len - 1; i++)
        {
            if (rank[i] >= 12 || rank[i + 1] >= 12)
                result = false;
            if (rank[i] - rank[i + 1] != -1)
                result = false;
        }
        return result;
    }

    // �������
    private bool CheckPairChain(int[] rank, int len)
    {
        bool result = true;

        for (int i = 0; i < len; i += 2)
        {
            if (rank[i] != rank[i + 1])
                result = false;
            if (rank[i] == 12)
                result = false;
        }

        for (int i = 0; i < len - 2; i += 2)
        {
            if (rank[i] - rank[i + 2] != -1)
                result = false;
        }

        return result;
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
