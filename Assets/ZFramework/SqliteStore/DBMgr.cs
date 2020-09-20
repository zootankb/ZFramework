using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 数据库管理器
    /// </summary>
    public static class DBMgr
    {
        /// <summary>
        /// 数据库存储器
        /// </summary>
        private static Dictionary<string, TableOperator> dbs = new Dictionary<string, TableOperator>();

        public static TableOperator DefaultDB = null;

        /// <summary>
        /// 默认自添加的数据库，库名字为 SqliteStore + Application.ProductName
        /// </summary>
        static DBMgr()
        {
            string ns = typeof(DBMgr).Namespace;
            string dbn = Application.productName;
            string dbFullName = GetDBKey(ns, dbn);
            DefaultDB = new TableOperator(dbFullName);
            dbs.Add(dbFullName, DefaultDB);
        }

        #region Pri Func

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static TableOperator Get(string ns, string dbname)
        {
            return Get(GetDBKey(ns, dbname));
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="dbFullName"></param>
        /// <returns></returns>
        public static TableOperator Get(string dbFullName)
        {
            if (dbs.ContainsKey(dbFullName))
            {
                return dbs[dbFullName];
            }
            return null;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="ns">namespace</param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static bool AddDB(string ns, string dbname)
        {
            return AddDB(GetDBKey(ns, dbname));
        }

        /// <summary>
        /// 添加并打开数据库
        /// </summary>
        /// <param name="dbFullName"></param>
        /// <returns></returns>
        public static bool AddDB(string dbFullName)
        {
            if (dbs.ContainsKey(dbFullName))
            {
                return false;
            }
            else
            {
                bool res = false;
                try
                {
                    TableOperator db = new TableOperator(dbFullName);
                    dbs.Add(dbFullName, db);
                }
                catch (SqliteException e)
                {
                    Debug.LogWarningFormat("添加数据库时异常：{0}", e.Message);
                }
                return res;
            }
        }

        /// <summary>
        /// 移除数据库，同时关闭数据库链接
        /// </summary>
        /// <param name="dbFullName"></param>
        /// <returns></returns>
        public static bool SubDB(string ns, string dbname)
        {
            return SubDB(GetDBKey(ns, dbname));
        }

        /// <summary>
        /// 移除数据库，同时关闭数据库链接
        /// </summary>
        /// <param name="dbFullName"></param>
        /// <returns></returns>
        public static bool SubDB(string dbFullName)
        {
            if (dbs.ContainsKey(dbFullName))
            {
                dbs[dbFullName].Close();
                dbs.Remove(dbFullName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 拼接数据库的名字，统一的方式，没有文件夹的路径
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="dbn"></param>
        /// <returns></returns>
        public static string GetDBKey(string ns, string dbn)
        {
            return string.Format("{0}.{1}.db", string.IsNullOrEmpty(ns) ? "NULL_DB_NS" : ns, string.IsNullOrEmpty(dbn) ? "NULL_DB_NAME" : dbn);
        }
        #endregion
    }
}