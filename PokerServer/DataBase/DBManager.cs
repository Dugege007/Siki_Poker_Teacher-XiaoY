using MySqlConnector;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
    /// <param name="db">数据库名称</param>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口号</param>
    /// <param name="user">用户名</param>
    /// <param name="pw">密码</param>
    /// <returns>如果连接成功，返回 true；否则，返回 false</returns>
    public static bool Connect(string db, string ip, int port, string user, string pw)
    {
        // 创建一个新的数据库连接对象
        mysql = new MySqlConnection();
        // 设置连接字符串，包括数据库名称、服务器地址、端口、用户名和密码
        string s = string.Format("Database = '{0}'; Data Source = '{1}'; Port = {2}; User = '{3}'; Password = '{4}';", db, ip, port, user, pw);
        mysql.ConnectionString = s;

        try
        {
            // 尝试打开数据库连接
            mysql.Open();
            Console.WriteLine("[数据库] 启动成功");
            return true;
        }
        catch (Exception ex)
        {
            // 如果连接失败，打印错误信息
            Console.WriteLine("[数据库] 启动失败：" + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 判断字符串是否安全
    /// 以防止 SQL 注入攻击
    /// </summary>
    /// <param name="str">需要判断的字符串</param>
    /// <returns>如果字符串是安全的（不包含可能用于 SQL 注入的字符），返回 true；否则，返回 false</returns>
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\[|\]|\{|\}|%|@|\*|!|\']");

        //TODO 可以加入更多的防止 SQL 注入攻击规则
    }

    /// <summary>
    /// 检查指定的账号是否存在
    /// </summary>
    /// <param name="id">要检查的账号的 ID</param>
    /// <returns>如果账号存在，返回 true；否则，返回 false</returns>
    public static bool IsAccountExist(string id)
    {
        // 检查 ID 是否安全
        if (!IsSafeString(id))
            return true;

        // 创建 SQL 查询语句，用于查找指定 ID 的账号
        string s = string.Format("SELECT * FROM account WHERE id = '{0}'", id);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行查询并获取结果
            MySqlDataReader dataReader = cmd.ExecuteReader();
            // 检查查询结果是否包含任何行（如果包含，说明账号存在）
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception ex)
        {
            // 如果查询失败，打印错误信息
            Console.WriteLine("[数据库] IsAccountExist Fail " + ex.Message);
            return true;
        }
    }

    /// <summary>
    /// 注册新用户
    /// </summary>
    /// <param name="id">新用户 ID</param>
    /// <param name="pw">新用户密码</param>
    /// <returns>如果注册成功，返回 true；否则，返回 false</returns>
    public static bool Register(string id, string pw)
    {
        // 检查用户输入是否安全，防止 SQL 注入攻击
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 注册失败，ID 不安全");
            return false;
        }

        if (!IsSafeString(pw))
        {
            Console.WriteLine("[数据库] 注册失败，密码不安全");
            return false;
        }

        // 检查账号是否已存在
        if (IsAccountExist(id))
        {
            Console.WriteLine("[数据库] 注册失败，账号已存在");
            return false;
        }

        // 创建 SQL 插入语句，用于在 ACCOUNT 表中插入新用户的 ID 和密码
        string s = string.Format("INSERT INTO account SET id = '{0}', pw = '{1}'", id, pw);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行插入操作
            // .ExecuteNonQuery() 方法可用于 增 删 改
            cmd.ExecuteNonQuery();
            Console.WriteLine("[数据库] 注册成功！");
            return true;
        }
        catch (Exception ex)
        {
            // 如果插入操作失败，打印错误信息
            Console.WriteLine("[数据库] 注册失败 " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 创建新的玩家角色
    /// </summary>
    /// <param name="id">新角色的 ID</param>
    /// <returns>如果创建成功，返回 true；否则，返回 false</returns>
    public static bool CreatePlayer(string id)
    {
        // 检查用户输入是否安全，防止 SQL 注入攻击
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 创建角色失败，ID 不安全");
            return false;
        }

        // 创建新的 PlayerData 对象，用于存储新玩家的数据
        PlayerData playerData = new PlayerData();
        // 将 PlayerData 对象序列化为 JSON 格式的字符串
        string data = JsonConvert.SerializeObject(playerData);
        // 创建 SQL 插入语句，用于在 player 表中插入新玩家的 ID 和数据
        string s = string.Format("INSERT INTO player SET id = '{0}', data = '{1}'", id, data);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行插入操作
            cmd.ExecuteNonQuery();
            Console.WriteLine("[数据库] 创建角色成功！");
            return true;
        }
        catch (Exception ex)
        {
            // 如果插入操作失败，打印错误信息
            Console.WriteLine("[数据库] 创建角色失败 " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 检查用户 ID 和密码是否匹配
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="pw">用户密码</param>
    /// <returns>如果 ID 和密码匹配，返回 true；否则，返回 false</returns>
    public static bool CheckPassword(string id, string pw)
    {
        // 检查用户输入是否安全，防止 SQL 注入攻击
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 检查 ID 失败，ID 不安全");
            return false;
        }

        if (!IsSafeString(pw))
        {
            Console.WriteLine("[数据库] 检查密码失败，密码不安全");
            return false;
        }

        // 创建 SQL 查询语句，用于在 account 表中查找匹配的 ID 和密码
        string s = string.Format("SELECT * FROM account WHERE id = '{0}' AND pw = '{1}'", id, pw);
        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行查询并获取结果
            // .ExecuteReader() 方法可用于 查
            MySqlDataReader dataReader = cmd.ExecuteReader();
            // 检查查询结果是否包含任何行（如果包含，说明 ID 和密码匹配）
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception ex)
        {
            // 如果查询操作失败，打印错误信息
            Console.WriteLine("[数据库] 检查密码失败 " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 获取指定 ID 的玩家数据
    /// </summary>
    /// <param name="id">玩家角色 ID</param>
    /// <returns></returns>
    public static PlayerData GetPlayerData(string id)
    {
        // 检查用户输入是否安全，防止 SQL 注入攻击
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] 获取角色信息失败，ID 不安全");
            return null;
        }

        // 创建 SQL 查询语句，用于在 player 表中查找指定 ID 的玩家数据
        string s = string.Format("SELECT * FROM player WHERE id = '{0}'", id);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行查询并获取结果
            MySqlDataReader dataReader = cmd.ExecuteReader();
            // 检查查询结果是否包含任何行（如果包含，说明找到了玩家）
            bool hasRows = dataReader.HasRows;
            if (!hasRows)
            {
                dataReader.Close();
                return null;
            }

            // 读取玩家数据
            dataReader.Read();
            string data = dataReader.GetString("data");

            // 将 JSON 格式的字符串反序列化为 PlayerData 对象
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception ex)
        {
            // 如果查询操作失败，打印错误信息
            Console.WriteLine("[数据库] 获取角色信息失败 " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新指定 ID 的玩家数据
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <param name="playerData">新的玩家数据</param>
    /// <returns>如果更新成功，返回 true；否则，返回 false</returns>
    public static bool UpdatePlayerData(string id, PlayerData playerData)
    {
        // 将 PlayerData 对象序列化为 JSON 格式的字符串
        string data = JsonConvert.SerializeObject(playerData);

        // 创建 SQL 更新语句，用于在 player 表中更新指定 ID 的玩家数据
        string s = string.Format("UPDATE player SET data = '{0}' WHERE id = '{1}'", data, id);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行更新操作
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            // 如果操作失败，打印错误信息
            Console.WriteLine("[数据库] 更新玩家数据失败 " + ex.Message);
            return false;
        }
    }
}
