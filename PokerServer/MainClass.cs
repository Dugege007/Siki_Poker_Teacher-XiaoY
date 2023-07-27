public class MainClass
{
    /// <summary>
    /// 主程序入口
    /// </summary>
    private static void Main()
    {
        if (!DBManager.Connect("Game", "127.0.0.1", 3306, "root", "dhb351080175"))
            return;

        // 创建玩家数据对象
        PlayerData playerData = new PlayerData();
        playerData.bean = 256;

        // 测试数据库功能
        DBManager.UpdatePlayerData("000", playerData);
        PlayerData result = DBManager.GetPlayerData("000");
        Console.WriteLine(result.bean);

        // 连接到本地服务器的 8888 端口
        NetManager.Connect("127.0.0.1", 8888);
    }
}
