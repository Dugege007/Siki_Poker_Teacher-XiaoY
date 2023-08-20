using System.Collections.Generic;

public class CardManager
{
    /// <summary>
    /// 牌型 卡牌组合类型
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

    /// <summary>
    /// 扑克牌字典
    /// 键为牌的名称，值为牌的对象
    /// </summary>
    public static Dictionary<string, Card> nameCards = new Dictionary<string, Card>();

    /// <summary>
    /// 初始化扑克牌
    /// </summary>
    public static void Init()
    {
        // 生成一副完整的扑克牌，包括 4 种花色，每种花色 13 张牌
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                // 创建一张牌
                Card card = new Card(i, j);
                // 生成牌的名称，由花色和点数组成
                string name = ((Suit)i).ToString() + ((Rank)j).ToString();
                // 将牌的名称和牌的对象添加到字典中
                nameCards.Add(name, card);
            }
        }

        // 添加大小王
        nameCards.Add("SJoker", new Card(Suit.None, Rank.SJoker));
        nameCards.Add("LJoker", new Card(Suit.None, Rank.LJoker));
    }

    /// <summary>
    /// 根据牌的对象获取牌的名称
    /// </summary>
    /// <param name="card">牌的对象</param>
    /// <returns>牌的名称</returns>
    public static string GetName(Card card)
    {
        // 遍历字典的键，即牌的名称
        foreach (string name in nameCards.Keys)
        {
            // 如果字典中的牌的花色和点数与参数中的牌的花色和点数相同，则返回牌的名称
            if (nameCards[name].suit == card.suit && nameCards[name].rank == card.rank)
            {
                return name;
            }
        }

        // 如果没有找到匹配的牌，则返回空字符串
        return "";
    }

    /// <summary>
    /// 根据卡牌名称获取卡牌
    /// </summary>
    /// <param name="name">卡牌名称</param>
    /// <returns>卡牌对象</returns>
    public static Card GetCard(string name)
    {
        if (nameCards.ContainsKey(name))
            return nameCards[name];

        return null;
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

    /// <summary>
    /// CardInfo 数组转 Card 数组
    /// </summary>
    /// <param name="cardsInfo">卡牌信息数组</param>
    /// <returns>卡牌数组</returns>
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
