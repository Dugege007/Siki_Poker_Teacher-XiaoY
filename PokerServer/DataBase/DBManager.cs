using MySqlConnector;

#nullable disable
public class DBManager
{
    /// <summary>
    /// 数据库对象
    /// </summary>
    public static MySqlConnection mysql;

    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <param name="db">数据表</param>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口号</param>
    /// <param name="user">用户名</param>
    /// <param name="pw">密码</param>
    /// <returns></returns>
    public static bool Connect(string db, string ip, int port,string user, string pw)
    {
        mysql = new MySqlConnection();
        string s = string.Format("Database = {0}; Data Source = {1}; Port = {2}; User = {3}; Password = {4};", db, ip, port, user, pw);
        mysql.ConnectionString = s;

        try
        {
            mysql.Open();
            Console.WriteLine("[数据库] 启动成功");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[数据库] 启动失败：" + ex.Message);
            return false;
        }
    }
}
