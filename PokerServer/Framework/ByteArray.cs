
public class ByteArray
{
    /// <summary>
    /// 默认大小
    /// </summary>
    const int DEFAULT_SIZE = 1024;

    /// <summary>
    /// 初始大小
    /// </summary>
    private int initSize;

    /// <summary>
    /// 字节数组
    /// </summary>
    public byte[] bytes;

    /// <summary>
    /// 读的位置
    /// </summary>
    public int readIndex;

    /// <summary>
    /// 写的位置
    /// </summary>
    public int writeIndex;

    /// <summary>
    /// 容量
    /// </summary>
    private int capacity;

    /// <summary>
    /// 读写长度
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }

    /// <summary>
    /// 数组余量
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }

    /// <summary>
    /// 提供长度的构造函数
    /// </summary>
    /// <param name="size">数组长度</param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        initSize = size;
        capacity = size;
        readIndex = 0;
        writeIndex = 0;
    }

    /// <summary>
    /// 提供字节数组的构造函数
    /// </summary>
    /// <param name="defaultBytes">字节数组</param>
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        initSize = defaultBytes.Length;
        capacity = defaultBytes.Length;
        readIndex = 0;
        writeIndex = defaultBytes.Length;
    }

    /// <summary>
    /// 移动数据
    /// 将数据置于最前端
    /// </summary>
    public void MoveBytes()
    {
        if (Length > 0)
        {
            // Array.Copy(源数组, 从哪里开始复制, 目标数组, 从哪个位置开始接收复制的数据, 要复制的元素数量);
            Array.Copy(bytes, readIndex, bytes, 0, Length);
        }

        writeIndex = Length;
        readIndex = 0;
    }

    /// <summary>
    /// 重设数组尺寸
    /// </summary>
    /// <param name="size">数组尺寸</param>
    public void ReSize(int size)
    {
        if (size < Length) return;
        if (size < initSize) return;

        capacity = size;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIndex, newBytes, 0, Length);
        bytes = newBytes;

        writeIndex = Length;
        readIndex = 0;
    }
}
