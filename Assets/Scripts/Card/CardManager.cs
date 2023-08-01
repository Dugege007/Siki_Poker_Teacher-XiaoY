using System.Collections.Generic;

public class CardManager
{
    // 用于存储一副完整的扑克牌，键为牌的名称，值为牌的对象
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
}
