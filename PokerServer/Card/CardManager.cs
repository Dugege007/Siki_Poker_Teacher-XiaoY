
using System.ComponentModel.DataAnnotations;

public class CardManager
{
    /// <summary>
    /// 卡牌列表
    /// </summary>
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// 卡牌组合类型
    /// </summary>
    public enum CardType
    {
        One,                // 单张     3
        Two,                // 对子     33
        Three,              // 三张     333
        ThreeWithOne,       // 三带一   333 4
        ThreeWithTwo,       // 三带二   333 44
        Chain,              // 顺子     3 4 5 6 7
        PairChain,          // 连对     33 44 55
        Airplane,           // 飞机     333 444
        AirplaneWithOne,    // 飞机带单 333 444 5 6
        AirplaneWithTwo,    // 飞机带对 333 444 55 66
        Bomb,               // 炸弹     3333
        FourWithTwo,        // 四带二   3333 44
        JokerBomb,          // 王炸     SJoker LJoker
        Wrong               // 错误类型
    }

    public CardType GetCardType(Card[] cards)
    {
        int[] rank = new int[20];
        int len = cards.Length;

        for (int i = 0; i < len; i++)
        {
            rank[i] = (int)cards[i].rank;
        }

        // 先排序
        // 只排传入的 cards 的长度，因为后面都是 0
        Array.Sort(rank, 0, len);
        // 设置默认值
        CardType cardType = CardType.Wrong;

        switch (len)
        {
            case 1:
                // 单张
                cardType = CardType.One;
                break;

            case 2:
                // 对子
                if (rank[0] == rank[1])
                    cardType = CardType.Two;
                // 王炸
                if (rank[0] + rank[1] == 27)
                    cardType = CardType.JokerBomb;
                break;

            case 3:
                // 三张
                if (rank[0] == rank[1] && rank[0] == rank[2])
                    cardType = CardType.Three;
                break;

            case 4:
                // 炸弹
                if (rank[0] == rank[1] && rank[0] == rank[2] && rank[0] == rank[3])
                    cardType = CardType.Bomb;
                // 三带一
                else if (rank[0] == rank[1] && rank[0] == rank[2])
                    cardType = CardType.ThreeWithOne;
                else if (rank[1] == rank[2] && rank[1] == rank[3])
                    cardType = CardType.ThreeWithOne;
                break;

            case 5:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 三带二 不支持对王
                else if (rank[0] == rank[1] && rank[0] == rank[2] && rank[3] == rank[4])
                    cardType = CardType.ThreeWithTwo;
                else if (rank[0] == rank[1] && rank[2] == rank[3] && rank[2] == rank[4])
                    cardType = CardType.ThreeWithTwo;
                break;

            case 6:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机
                if (CheckAirplane(rank, len))
                    cardType = CardType.Airplane;
                break;

            case 7:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                break;

            case 8:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机带一
                if (CheckAirplaneWithOne(rank, len))
                    cardType = CardType.AirplaneWithOne;
                break;

            case 9:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 飞机
                if (CheckAirplane(rank, len))
                    cardType = CardType.Airplane;
                break;

            case 10:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 11:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                break;

            case 12:
                // 顺子
                if (CheckChain(rank, len))
                    cardType = CardType.Chain;
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机
                if (CheckAirplane(rank, len))
                    cardType = CardType.Airplane;
                // 飞机带一
                if (CheckAirplaneWithOne(rank, len))
                    cardType = CardType.AirplaneWithOne;
                break;

            case 14:
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                break;

            case 15:
                // 飞机
                if (CheckAirplane(rank, len))
                    cardType = CardType.Airplane;
                break;

            case 16:
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机带一
                if (CheckAirplaneWithOne(rank, len))
                    cardType = CardType.AirplaneWithOne;
                break;

            case 18:
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机
                if (CheckAirplane(rank, len))
                    cardType = CardType.Airplane;
                break;

            case 20:
                // 连对
                if (CheckPairChain(rank, len))
                    cardType = CardType.PairChain;
                // 飞机带一
                if (CheckAirplaneWithOne(rank, len))
                    cardType = CardType.AirplaneWithOne;
                break;

            default:
                cardType = CardType.Wrong;
                break;
        }

        return cardType;
    }

    // 检查顺子
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

    // 检查连对
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

    // 检查飞机
    private bool CheckAirplane(int[] rank, int len)
    {
        bool result = true;

        for (int i = 0; i < len; i += 3)
        {
            if (rank[i] != rank[i + 1] || rank[i] != rank[i + 2])
                result = false;
            if (rank[i] == 12)
                result = false;
        }

        for (int i = 0; i < len - 3; i += 3)
        {
            if (rank[i] - rank[i + 3] != -1)
                result = false;
        }

        return result;
    }

    // 检查飞机带一
    private bool CheckAirplaneWithOne(int[] rank, int len)
    {
        bool result = true;
        int planeLen = len / 4;
        int[] arr = new int[planeLen];
        int index = 0;

        for (int i = 0; i < len; i++)
        {
            if (rank[i] == rank[i + 1] && rank[i] == rank[i + 2])
            {
                arr[index++] = rank[i];
                i += 2;
            }
        }

        if (planeLen == index)
        {
            for (int i = 0; i < planeLen - 1; i++)
            {
                if (arr[i] == 12 || arr[i + 1] == 12)
                    result = false;
                if (arr[i] - arr[i + 1] != -1)
                    result = false;
            }
        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public static void Shuffle()
    {
        // 清空卡牌列表，避免重新洗牌时多出一副牌
        cards.Clear();

        // 生成一副完整的扑克牌，包括 4 种花色，每种花色 13 张牌
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card card = new Card(i, j);
                cards.Add(card);
            }
        }

        // 添加大小王
        cards.Add(new Card(Suit.None, Rank.SJoker));
        cards.Add(new Card(Suit.None, Rank.LJoker));

        // 创建一个队列用于临时存储洗牌过程中的卡牌
        Queue<Card> cardQueue = new Queue<Card>();

        // 随机选择一张卡牌，将其从原列表中移除，然后添加到队列中
        for (int i = 0; i < 54; i++)
        {
            Random random = new Random();
            int index = random.Next(0, cards.Count);
            cardQueue.Enqueue(cards[index]);
            cards.RemoveAt(index);
        }

        // 将队列中的卡牌重新添加到列表中，完成洗牌
        for (int i = 0; i < 54; i++)
        {
            cards.Add(cardQueue.Dequeue());
        }
    }

    /// <summary>
    /// Card 数组转 CardInfo 数组
    /// </summary>
    /// <param name="cards">卡牌数组</param>
    /// <returns>卡牌信息数组</returns>
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
