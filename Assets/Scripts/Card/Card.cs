using System;

public enum Suit
{
    None,
    Club,
    Square,
    Heart,
    Spade,
}

public enum Rank
{
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    One,
    Two,
    SJoker,
    LJoker,
}

public class Card
{
    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit;
        this.rank = (Rank)rank;
    }

    /// <summary>
    /// 重写 Equals，用来比较两张牌是否相等
    /// </summary>
    /// <param name="obj">卡牌对象</param>
    /// <returns>返回是否相等</returns>
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        Card card = obj as Card;
        if (card == null) return false;

        return suit == card.suit && rank == card.rank;
    }

    /// <summary>
    /// 随重写 Equals 重写 GetHashCode
    /// （这部分我目前也不太明白，先跟着教程写了）
    /// </summary>
    /// <returns>返回哈希码是否相等</returns>
    public override int GetHashCode()
    {
        // 元组相等
        // 可查看官方文档解释
        return Tuple.Create(suit, rank).GetHashCode();
    }

    /// <summary>
    /// 获取卡牌信息
    /// </summary>
    /// <returns>返回卡牌信息</returns>
    public CardInfo GetCardInfo()
    {
        CardInfo info = new CardInfo();
        info.suit = (int)suit;
        info.rank = (int)rank;
        return info;
    }
}
