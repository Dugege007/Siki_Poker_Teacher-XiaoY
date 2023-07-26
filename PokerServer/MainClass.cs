namespace PokerServer
{
    public class MainClass
    {
        /// <summary>
        /// 主程序入口
        /// </summary>
        private static void Main()
        {
            // 连接到本地服务器的 8888 端口
            NetManager.Connect("127.0.0.1", 8888);
        }
    }
}
