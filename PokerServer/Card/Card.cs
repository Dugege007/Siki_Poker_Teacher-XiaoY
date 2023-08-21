
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

#nullable disable
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
    /// ��д Equals�������Ƚ��������Ƿ����
    /// </summary>
    /// <param name="obj">���ƶ���</param>
    /// <returns>�����Ƿ����</returns>
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        Card card = obj as Card;
        if (card == null) return false;

        return suit == card.suit && rank == card.rank;
    }

    /// <summary>
    /// ����д Equals ��д GetHashCode
    /// </summary>
    /// <returns>���ع�ϣ���Ƿ����</returns>
    public override int GetHashCode()
    {
        // Ԫ�����
        // �ɲ鿴�ٷ��ĵ�����
        return Tuple.Create(suit, rank).GetHashCode();
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
