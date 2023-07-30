
public class CardManager
{
    // ����һ���б����ڴ洢���еĿ���
    public static List<Card> cards = new List<Card>();

    /// <summary>
    /// ϴ��
    /// </summary>
    public static void Shuffle()
    {
        // ����һ���������˿��ƣ�����4�ֻ�ɫ��ÿ�ֻ�ɫ13����
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
}
