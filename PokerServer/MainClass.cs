namespace PokerServer
{
    public class MainClass
    {
        /// <summary>
        /// 主程序入口
        /// </summary>
        private static void Main()
        {
            if (!DBManager.Connect("Game", "127.0.0.1", 3306, "root", "dhb351080175"))
                return;

            // 测试 IsAccountExist 方法
            Console.WriteLine(DBManager.IsAccountExist("1234"));

            // 连接到本地服务器的 8888 端口
            NetManager.Connect("127.0.0.1", 8888);
        }
    }
}
