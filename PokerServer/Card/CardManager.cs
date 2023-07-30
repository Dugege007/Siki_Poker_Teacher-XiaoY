
public class CardManager
{
    // 创建一个列表用于存储所有的卡牌
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// 洗牌
    /// </summary>
    public static void Shuffle()
    {
        // 生成一副完整的扑克牌，包括4种花色，每种花色13张牌
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
}
