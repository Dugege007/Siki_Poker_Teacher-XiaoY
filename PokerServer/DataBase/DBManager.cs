﻿using MySqlConnector;
using System.Text.RegularExpressions;

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
        string s = string.Format("Database = {0}; Data Source = {1}; Port = {2}; User = {3}; Password = {4};", db, ip, port, user, pw);
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
        string s = string.Format("SELECT * FROM ACCOUNT WHERE id = {0}", id);

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
    /// 注册
    /// </summary>
    /// <param name="id">玩家 ID</param>
    /// <param name="pw">玩家密码</param>
    /// <returns></returns>
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
        string s = string.Format("INSERT INTO ACCOUNT SET id = {0}, pw = {1}", id, pw);

        try
        {
            // 创建一个新的 MySQL 命令对象
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            // 执行插入操作
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
}
