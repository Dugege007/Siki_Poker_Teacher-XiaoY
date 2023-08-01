
/// <summary>
/// 卡牌花色
/// </summary>
public enum Suit
{
    None,
    Club,
    Square,
    Heart,
    Spade,
}

/// <summary>
/// 卡牌序号
/// </summary>
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

    /// <summary>
    /// 构造函数
    /// 根据花色和序号的枚举创建卡牌
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="rank"></param>
    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    /// <summary>
    /// 构造函数
    /// 根据花色和序号的整型值创建卡牌
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="rank"></param>
    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit;
        this.rank = (Rank)rank;
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
