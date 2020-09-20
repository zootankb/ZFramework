using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Mono.Data.Sqlite;
using System;
using System.Data;

namespace ZFramework
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public static class DBEditor
    {
        /// <summary>
        /// 操作项名字
        /// </summary>
        private const string MenuName = "数据库操作/";

        [MenuItem(MenuName + "打开db所在文件夹")]
        public static void OpenDBDir()
        {
            string path = string.Format("{0}/{1}.db", Application.persistentDataPath, Application.productName);
            if (!File.Exists(path))
            {
                path = Application.persistentDataPath +"/Unity";
                if (!Directory.Exists(path))
                {
                    path = Application.persistentDataPath;
                }
            }
            Debug.Log("db路径："+ path);
            EditorUtility.RevealInFinder(path);
        }

        [MenuItem(MenuName + "删除数据库")]
        public static void DeleteDB()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
            if(files!= null && files.Length > 0)
            {
                try
                {
                    foreach (var path in files)
                    {
                        File.Delete(path);
                    }
                    Debug.LogFormat("成功删除数据库:{0}", string.Join(", ", files));
                }
                catch(Exception e)
                {
                    Debug.LogWarningFormat("删除数据库时异常：{0}", e.Message);
                }
            }
            else
            {
                Debug.LogWarningFormat("没有数据库db文件");
            }
        }

        [MenuItem(MenuName + "尝试断开数据库所有链接")]
        public static void TryCloseDBConnect()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                foreach (var path in files)
                {
                    SqliteConnectionStringBuilder scsb = new SqliteConnectionStringBuilder();
                    scsb.DataSource = path;
                    try
                    {
                        using (SqliteConnection sc = new SqliteConnection(scsb.ToString()))
                        {
                            sc.Close();
                        }
                    }catch(SqliteException e)
                    {
                        Debug.LogWarningFormat("尝试断开数据库 {0} 链接异常：{1}", e.Message, path);
                    }
                }
                Debug.LogFormat("已经关闭所有数据库的链接： {0}", string.Join(", ", files));
            }
            else
            {
                Debug.LogWarningFormat("没有数据库db文件");
            }
        }

        [MenuItem(MenuName + "清除所有库中所有的表")]
        public static void ClearAllTables()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                foreach (var path in files)
                {
                    SqliteConnectionStringBuilder scsb = new SqliteConnectionStringBuilder();
                    scsb.DataSource = path;
                    try
                    {
                        using (SqliteConnection connection = new SqliteConnection(scsb.ToString()))
                        {
                            connection.Open();
                            DataTable data = ExecuteQuery("SELECT name FROM sqlite_master WHERE type='table' and name != 'sqlite_sequence' ORDER BY name;", connection);
                            for (int i = 0; i < data.Rows.Count; i++)
                            {
                                string cmdStr = string.Format("DROP TABLE {0};", data.Rows[i][0].ToString());
                                ExecuteNonQuery(cmdStr, connection);
                            }
                            connection.Close();
                        }
                    }
                    catch (SqliteException e)
                    {
                        Debug.LogWarningFormat("执行清除库中所有的表时 {0} 异常：{1}", path, e.Message);
                    }
                }
                Debug.LogFormat("已经清除所有库中所有的表： {0}", string.Join(", ", files));
            }
            else
            {
                Debug.LogWarningFormat("没有数据库db文件");
            }
        }

        [MenuItem(MenuName + "清除所有表的记录")]
        public static void ClearAllRecords()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                foreach (var path in files)
                {
                    SqliteConnectionStringBuilder scsb = new SqliteConnectionStringBuilder();
                    scsb.DataSource = path;
                    try
                    {
                        using (SqliteConnection connection = new SqliteConnection(scsb.ToString()))
                        {
                            connection.Open();
                            DataTable data = ExecuteQuery("SELECT name FROM sqlite_master WHERE type='table' and name != 'sqlite_sequence' ORDER BY name;", connection);
                            for (int i = 0; i < data.Rows.Count; i++)
                            {
                                string cmdStr = string.Format("delete from {0};", data.Rows[i][0].ToString());
                                ExecuteNonQuery(cmdStr, connection);
                            }
                            connection.Close();
                        }
                    }
                    catch (SqliteException e)
                    {
                        Debug.LogWarningFormat("执行清除所有库中所有表的记录时 {0} 异常：{1}", path, e.Message);
                    }
                }
                Debug.LogFormat("已经清除所有库中清除所有表的记录： {0}", string.Join(", ", files));
            }
            else
            {
                Debug.LogWarningFormat("没有数据库db文件");
            }
        }

        #region Pri Func
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static DataTable ExecuteQuery(string cmdStr, SqliteConnection connection)
        {
            DataTable dt = new DataTable();
            using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
            {
                SqliteDataAdapter adapter = new SqliteDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 执行非查询语句
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static int ExecuteNonQuery(string cmdStr, SqliteConnection connection)
        {
            int res = 0;
            using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
            {
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }

        #endregion
    }
}