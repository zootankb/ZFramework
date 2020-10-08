using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace ZFramework.Editor
{
    /// <summary>
    /// 数据库的操作者
    /// </summary>
    public static class DBOperator
    {
        #region API

        /// <summary>
        /// 删除库中所有表的记录
        /// </summary>
        /// <param name="dbPaths"></param>
        /// <returns></returns>
        public static bool DeleteAllTableRecords(List<string> dbPaths)
        {
            bool res = false;
            try
            {
                foreach (var path in dbPaths)
                {
                    List<string> tableNames = GetAllDataTableNames(path);
                    foreach (var tableName in tableNames)
                    {
                        string cmdStr = string.Format("DELETE FROM {0};", tableName);
                        ExecuteNonQuery(cmdStr, path);
                    }
                }
                res = true;
            }
            catch(SqliteException e)
            {
                Debug.LogError("删除数据库中的所有表记录异常，异常原因：" + e.Message);
                res = false;
            }
            return res;
        }

        /// <summary>
        /// 删除所有库中所有的表
        /// </summary>
        /// <param name="dbPaths"></param>
        /// <returns></returns>
        public static bool DropAllTables(List<string> dbPaths)
        {
            bool res = false;
            try
            {
                foreach (var path in dbPaths)
                {
                    List<string> tableNames = GetAllDataTableNames(path);
                    foreach (var tableName in tableNames)
                    {
                        string cmdStr = string.Format("DROP TABLE {0};", tableName);
                        ExecuteNonQuery(cmdStr, path);
                    }
                }
                res = true;
            }
            catch(Exception e)
            {
                res = false;
                Debug.LogError("删除所有库中所有的表异常，原因：" + e.Message);
            }
            return res;
        }

        /// <summary>
        /// 更新表记录
        /// </summary>
        /// <param name="newFields"></param>
        /// <param name="oriFields"></param>
        /// <param name="dcc"></param>
        /// <param name="tableName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static int UpdateReord(List<object> newFields, List<object> oriFields, DataColumnCollection dcc, string tableName, string connectStr)
        {
            int autoKeyIndex = -1;
            if (HasAutoincrementKey(tableName, connectStr))
            {
                for (int i = 0; i < dcc.Count; i++)
                {
                    if (dcc[i].ToString().ToLower().Contains("id"))
                    {
                        autoKeyIndex = i;
                        break;
                    }
                }
            }
            List<string> newFieldValue = new List<string>();
            for (int i = 0; i < dcc.Count; i++)
            {
                if (autoKeyIndex != i)
                {
                    newFieldValue.Add(GetFieldTypeContentForSQlite(dcc[i].ColumnName, newFields[i], dcc[i].DataType));
                }
            }
            List<string> oriFieldValue = new List<string>();
            for (int i = 0; i < dcc.Count; i++)
            {
                oriFieldValue.Add(GetFieldTypeContentForSQlite(dcc[i].ToString(), oriFields[i], dcc[i].DataType));
            }
            string cmdStr = string.Format("update {0} set {1} where {2};", tableName, string.Join(",", newFieldValue), string.Join(" and ", oriFieldValue));
            return ExecuteNonQuery(cmdStr, connectStr);
        }

        /// <summary>
        /// 添加表记录
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dcc"></param>
        /// <param name="tableName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static int InsertRecord(List<object> fields, DataColumnCollection dcc, string tableName, string connectStr)
        {
            List<string> cmdName = new List<string>();
            List<string> cmdValue = new List<string>();
            int autoKeyIndex = -1;
            if(HasAutoincrementKey(tableName, connectStr))
            {
                for (int i = 0; i < dcc.Count; i++)
                {
                    if (dcc[i].ToString().ToLower().Contains("id"))
                    {
                        autoKeyIndex = i;
                        break;
                    }
                }
            }
            for (int i = 0; i < dcc.Count; i++)
            {
                if(autoKeyIndex != i)
                {
                    cmdName.Add(dcc[i].ToString());
                    cmdValue.Add(GetFieldTypeValueForSQlite(fields[i], dcc[i].DataType));
                }
            }
            string cmdStr = string.Format("Insert into {0}({1}) values ({2});", tableName, string.Join(",", cmdName), string.Join(",", cmdValue));
            return ExecuteNonQuery(cmdStr, connectStr);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="selectIndex"></param>
        /// <param name="tableName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static int DeleteRecords(DataTable dt, List<int> selectIndex, string tableName, string connectStr)
        {
            string cmdStr = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (selectIndex.Contains(i))
                {
                    string cmd = string.Format("delete from {0} where ", tableName);
                    List<string> keyvalue = new List<string>();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        keyvalue.Add(GetFieldTypeContentForSQlite(dt.Columns[j].ToString(), dt.Rows[i][j], dt.Columns[j].DataType));
                    }
                    cmd += string.Join(" and ", keyvalue) + ";";
                    cmdStr += cmd;
                }
            }
            return ExecuteNonQuery(cmdStr, connectStr);
        }

        /// <summary>
        /// 检查表是否有自增长主键，这里只把第一个字段作为主键
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static bool HasAutoincrementKey(string tableName, string connectStr)
        {
            string cmdStr = string.Format("SELECT sql FROM sqlite_master WHERE type='table' and name == '{0}' ORDER BY name;", tableName);
            DataTable dt = ExecuteQuery(cmdStr, connectStr);
            if(dt!=null && dt.Rows.Count > 0)
            {
                string resStr = dt.Rows[0][0].ToString();
                return resStr.Contains("INTEGER PRIMARY KEY AUTOINCREMENT");
            }
            return false;
        }


        /// <summary>
        /// 获取库中所有的表
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static List<string> GetAllDataTableNames(string connectStr)
        {
            string cmdStr = "SELECT name FROM sqlite_master WHERE type='table' and name != 'sqlite_sequence' ORDER BY name;";
            DataTable dt = ExecuteQuery(cmdStr, connectStr);
            List<string> tablenames = new List<string>();
            if(dt!=null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tablenames.Add(dt.Rows[i][0].ToString());
                }
            }
            return tablenames;
        }

        /// <summary>
        /// 获取某个表的所有内容
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static DataTable GetDataTabel(string tableName, string connectStr)
        {
            string cmdStr = string.Format("SELECT * FROM {0};", tableName);
            return ExecuteQuery(cmdStr, connectStr);
        }

        /// <summary>
        /// 执行自查询命令
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static DataTable GetDataTabelAuto(string cmdStr, string connectStr)
        {
            DataTable dt = null;
            try
            {
                dt = ExecuteQuery(cmdStr, connectStr); 
            }
            catch(SqliteException e)
            {
                Debug.LogError("自查询命令异常，原因：" + e.Message);
            }
            return dt;
        }


        #endregion

        #region Pri Func
        /// <summary>
        /// 根据内容和类型获取值在SQlite里面插入数据时需要显示的值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetFieldTypeValueForSQlite(object content, Type t)
        {
            string result = string.Empty;
            if (t == typeof(String))
            {
                result = content == null ? "''" : string.Format("'{0}'", content.ToString());
            }
            else if (t == typeof(bool))
            {
                result = content == null ? "false" : content.ToString();
            }
            else
            {
                // 找不到的就直接显示
                result = content == null ? "0" : content.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取拼接sql命令字符串
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="content"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetFieldTypeContentForSQlite(string fieldName, object content, Type t)
        {
            string result = string.Format("{0} = ", fieldName);
            result += GetFieldTypeValueForSQlite(content, t);
            return result;
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static DataTable ExecuteQuery(string cmdStr, string connectStr)
        {
            DataTable dt = new DataTable();
            SqliteConnectionStringBuilder scsb = new SqliteConnectionStringBuilder();
            scsb.DataSource = connectStr;
            using (SqliteConnection connection = new SqliteConnection(scsb.ToString()))
            {
                connection.Open();
                using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
                {
                    SqliteDataAdapter adapter = new SqliteDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                connection.Close();
            }
            return dt;
        }

        /// <summary>
        /// 执行非查询语句
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static int ExecuteNonQuery(string cmdStr, string connectStr)
        {
            int res = 0;
            SqliteConnectionStringBuilder scsb = new SqliteConnectionStringBuilder();
            scsb.DataSource = connectStr;
            using (SqliteConnection connection = new SqliteConnection(scsb.ToString()))
            {
                connection.Open();
                using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
                {
                    res = cmd.ExecuteNonQuery();

                }
                connection.Close();
            }
            return res;
        }

        #endregion
    }
}