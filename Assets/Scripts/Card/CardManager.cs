using System.Collections.Generic;

public class CardManager
{
    public static Dictionary<string, Card> nameCards = new Dictionary<string, Card>();

    public static void Init()
    {
        // ����һ���������˿��ƣ����� 4 �ֻ�ɫ��ÿ�ֻ�ɫ 13 ����
        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card card = new Card(i, j);
                string name = ((Suit)i).ToString() + ((Rank)j).ToString();
                nameCards.Add(name, card);
            }
        }

        // ��Ӵ�С��
        nameCards.Add("SJoker", new Card(Suit.None, Rank.SJoker));
        nameCards.Add("LJoker", new Card(Suit.None, Rank.LJoker));
    }

    public static string GetName(Card card)
    {
        foreach (string name in nameCards.Keys)
        {
            if (nameCards[name].suit == card.suit && nameCards[name].rank == card.rank)
            {
                return name;
            }
        }

        return "";
    }
}
