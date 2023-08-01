
/// <summary>
/// ���ƻ�ɫ
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
/// �������
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
    /// ���캯��
    /// ���ݻ�ɫ����ŵ�ö�ٴ�������
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="rank"></param>
    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    /// <summary>
    /// ���캯��
    /// ���ݻ�ɫ����ŵ�����ֵ��������
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="rank"></param>
    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit;
        this.rank = (Rank)rank;
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    /// <returns>���ؿ�����Ϣ</returns>
    public CardInfo GetCardInfo()
    {
        CardInfo info = new CardInfo();
        info.suit = (int)suit;
        info.rank = (int)rank;
        return info;
    }
}
