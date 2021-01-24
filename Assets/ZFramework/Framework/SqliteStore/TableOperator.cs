using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZFramework.Log;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 库的操纵者，库必须放在Application.persistentDataPath下的直接文件夹下
    /// </summary>
    public class TableOperator
    {
        #region Field
        /// <summary>
        /// 数据库链接
        /// </summary>
        private SqliteConnection connection = null;

        /// <summary>
        /// 数据库链接结构
        /// </summary>
        private SqliteConnectionStringBuilder scsb = null;

        /// <summary>
        /// 域（命名空间）
        /// </summary>
        private string assembly = null;

        /// <summary>
        /// 数据库锁
        /// </summary>
        private object _dbLocker = new object();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="assembly"></param>
        public TableOperator(string dbName = null, string assembly = null)
        {
            if (assembly != null)
            {
                this.assembly = assembly;
            }
            else
            {
                this.assembly = this.GetType().Namespace;
            }
            string dbn = Application.productName;
            if (!string.IsNullOrEmpty(dbName))
            {
                dbn = dbName;
            }
            string dbPath = string.Format("{0}/{1}.{2}.db", Application.persistentDataPath, this.assembly, dbn);
            try
            {
                scsb = new SqliteConnectionStringBuilder();
                scsb.DataSource = dbPath;

                if (!File.Exists(dbPath))
                {
                    CreateDB(dbPath);
                    Debug.LogFormat("创建数据库：{0}", dbPath);
                }
                else
                {
                    // 初始化所有的表，并创建没有的表
                    Open();
                    InitAllTables();
                    Close();
                }
            }
            catch (SqliteException e)
            {
                connection = null;
                Debug.LogErrorFormat("创建数据库时有异常：{0}", e.Message);
            }
        }

        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <param name="dbFullName"></param>
        public TableOperator(string dbFullName)
        {
            try
            {
                dbFullName = dbFullName.ToLower().EndsWith(".db") ? dbFullName : string.Format("{0}.db", dbFullName);
                string[] sp = dbFullName.Split(',');
                this.assembly = sp.Length >= 2 ? sp[0] : this.GetType().Namespace;
                dbFullName = string.Format("{0}/{1}", Application.persistentDataPath, dbFullName);
                scsb = new SqliteConnectionStringBuilder();
                scsb.DataSource = dbFullName;

                if (!File.Exists(dbFullName))
                {
                    CreateDB(dbFullName);
                    Debug.LogFormat("创建数据库：{0}", dbFullName);
                }
                else
                {
                    // 初始化所有的表，并创建没有的表
                    InitAllTables();
                }
            }
            catch (SqliteException e)
            {
                connection = null;
                LogOperator.AddWarnningRecord("创建数据库时有异常", e.Message);
            }
        }

        #region 外部API
        /// <summary>
        /// 链接数据库
        /// </summary>
        /// <returns></returns>
        internal bool Open()
        {
            try
            {
                if(connection != null && connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                else
                {
                    connection = new SqliteConnection(scsb.ToString());
                    connection.Open();
                }
                return true;
            }
            catch (Exception e)
            {
                LogOperator.AddWarnningRecord("链接数据库时有异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭数据库链接
        /// </summary>
        /// <returns></returns>
        internal bool Close()
        {
            try
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                return true;
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("关闭数据库时有异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool Insert<T>(T t) where T : SqliteDataTableBase
        {
            return Insert(new List<T> { t });
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        internal bool Insert<T>(List<T> ts) where T : SqliteDataTableBase
        {
            Open();
            bool hasIdIncre = HasAutoIncrementID<T>();
            string cmdStr = SqliteTabelCmdStrTool.GetInsertStr(ts, hasIdIncre);

            Debug.Log(cmdStr);

            SqliteTransaction st = connection.BeginTransaction();
            try
            {
                int res = 0;
                lock (_dbLocker)
                {
                    res = OperateRecords(cmdStr);
                    if (res > 0)
                    {
                        st.Commit();
                    }
                }
                return res > 0;
            }
            catch (SqliteException e)
            {
                st.Rollback();
                LogOperator.AddWarnningRecord("插入数据时异常", e.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool Delete<T>(T t) where T : SqliteDataTableBase
        {
            return Delete(new List<T> { t });
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        internal bool Delete<T>(List<T> ts) where T : SqliteDataTableBase
        {
            Open();
            bool hasIdPk = HasIdPK<T>();
            string cmdStr = SqliteTabelCmdStrTool.GetDeleteStr(ts, hasIdPk);
            SqliteTransaction st = connection.BeginTransaction();
            try
            {
                int res = 0;
                lock (_dbLocker)
                {
                    res = OperateRecords(cmdStr);
                    if (res > 0)
                    {
                        st.Commit();
                    }
                }
                return res > 0;
            }
            catch (SqliteException e)
            {
                st.Rollback();
                LogOperator.AddWarnningRecord("删除记录时异常", e.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args">存的是一个实例中的字段名与值，key为字段名字，value的List是实例的字段名字、类型和值</param>
        /// <returns></returns>
        internal bool Update<T>(T t, Dictionary<string, List<object>> args = null) where T : SqliteDataTableBase
        {
            return Update(new List<T> { t }, args);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="args">存的是一个实例中的字段名与值，key为字段名字，value的List是实例的字段名字、类型和值</param>
        /// <returns></returns>
        internal bool Update<T>(List<T> ts, Dictionary<string, List<object>> args = null) where T : SqliteDataTableBase
        {
            Open();
            bool hasIdPk = HasIdPK<T>();
            Dictionary<string, string> argsStr = args != null ? args.Select(arg => new { key = arg.Key, value = SqliteTabelCmdStrTool.GetFieldValue(arg.Value[0].ToString(), arg.Value[1] as Type, arg.Value[2]) }).ToDictionary(p => p.key, p => p.value) : null;
            string cmdStr = SqliteTabelCmdStrTool.GetUpdateStr(ts, hasIdPk, argsStr);
            SqliteTransaction st = connection.BeginTransaction();
            try
            {
                int res = 0;
                lock (_dbLocker)
                {
                    res = OperateRecords(cmdStr);
                    if (res > 0)
                    {
                        st.Commit();
                    }
                }
                return res > 0;
            }
            catch (SqliteException e)
            {
                st.Rollback();
                LogOperator.AddWarnningRecord("更新记录时异常", e.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 条件查询，默认查询所有
        /// 查询条件结构解释：key为字段名字，value的List是实例的字段名字、类型和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">存的是一个实例中的字段名与值，key为字段名字，value的List是实例的字段名字、类型和值</param>
        /// <returns></returns>
        internal List<T> Select<T>(Dictionary<string, List<object>> args = null) where T : SqliteDataTableBase, new()
        {
            Open();
            Dictionary<string, string> argsStr = (args == null || args.Count == 0) ?
                null : args.Select(arg => new { key = arg.Key, value = SqliteTabelCmdStrTool.GetFieldValue(arg.Value[0].ToString(), arg.Value[1] as Type, arg.Value[2]) })
                .ToDictionary(p => p.key, p => p.value);
            string cmdStr = SqliteTabelCmdStrTool.GetSelectStr<T>(argsStr);
            SqliteTransaction st = connection.BeginTransaction();
            try
            {
                List<T> ts = new List<T>();
                lock (_dbLocker)
                {
                    DataTable dt = OperateQuery(cmdStr);
                    if (dt.Rows.Count > 0)
                    {
                        Dictionary<string, FieldInfo> fNameType = new Dictionary<string, FieldInfo>();
                        FieldInfo[] fis = typeof(T).GetFields();
                        foreach (var fi in fis)
                        {
                            fNameType.Add(fi.Name, fi);
                        }
                        
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            T t = new T();

                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                string cName = dt.Columns[j].ColumnName;
                                Type type = dt.Columns[j].DataType;
                                if (type == typeof(Int64) || type == typeof(Int32))
                                {
                                    var v = dt.Rows[i][j];
                                    fNameType[cName].SetValue(t, Convert.ToInt32(v));
                                }
                                else if (type == typeof(String))
                                {
                                    var value = dt.Rows[i][j];
                                    fNameType[cName].SetValue(t, value == null ? null : value.ToString());
                                }
                                else if (type == typeof(Single))
                                {
                                    fNameType[cName].SetValue(t, Convert.ToSingle(dt.Rows[i][j]));
                                }
                                else if (type == typeof(Boolean))
                                {
                                    fNameType[cName].SetValue(t, Convert.ToBoolean(dt.Rows[i][j]));
                                }
                                else if (type == typeof(Double))
                                {
                                    fNameType[cName].SetValue(t, Convert.ToDouble(dt.Rows[i][j]));
                                }
                                else
                                {
                                    // 找不到的就直接赋值
                                    fNameType[cName].SetValue(t, dt.Rows[i][j]);
                                }
                            }
                            ts.Add(t);
                        }
                        return ts;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqliteException e)
            {
                st.Rollback();
                LogOperator.AddWarnningRecord("查询记录时异常", e.Message);
                return null;
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region 内部使用

        /// <summary>
        /// 结构是否存在自增长字段id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal bool HasAutoIncrementID<T>() where T : SqliteDataTableBase
        {
            return HasAutoIncrementID(typeof(T));
        }

        /// <summary>
        /// 结构是否存在自增长字段id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool HasAutoIncrementID(Type t)
        {
            try
            {
                string cmdStr = SqliteTabelCmdStrTool.GetAutoIncrement(t);
                DataTable dt = OperateQuery(cmdStr);
                return dt.Rows[0][0].ToString().Contains("INTEGER PRIMARY KEY AUTOINCREMENT");
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("执行查询结构是否存在自增长字段id时异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 是否有主键id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal bool HasIdPK<T>() where T : SqliteDataTableBase
        {
            return HasIdPK(typeof(T));
        }

        /// <summary>
        /// 是否有主键id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool HasIdPK(Type t)
        {
            try
            {
                string cmdStr = SqliteTabelCmdStrTool.GetAutoIncrement(t);
                DataTable dt = OperateQuery(cmdStr);
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    return dt.Rows[0][0].ToString().ToUpper().Contains("ID INTEGER PRIMARY KEY");
                }
                {
                    return false;
                }
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("执行查询结构是否存在自增长字段id时异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbPath"></param>
        internal void CreateDB(string dbPath)
        {
            connection = new SqliteConnection(scsb.ToString());
            lock (_dbLocker)
            {
                connection.Open();
                // 设置自动删除空白页
                OperateRecords(SqliteTabelCmdStrTool.GetAutoVacuum());
                // 创建表
                InitAllTables();
                connection.Close();
            }
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal bool CheckTable(Type type)
        {
            return CheckTable(SqliteTabelCmdStrTool.GetTableName(type));
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <returns></returns>
        internal bool CheckTable(string tablename)
        {
            try
            {
                int res = 0;
                lock (_dbLocker)
                {
                    string cmdStr = SqliteTabelCmdStrTool.GetCheckNameExist(tablename);
                    DataTable data = OperateQuery(cmdStr);
                    res = int.Parse(data.Rows[0][0].ToString());
                }
                return res == 1;
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("检查表存在出现异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 初始化所有的表并创建没有的表
        /// </summary>
        /// <returns></returns>
        internal bool InitAllTables()
        {
            Open();
            try
            {
                Type[] ts = this.GetType().Assembly.GetTypes();
                List<Type> types = ts.Where(t => t.BaseType != null && t.BaseType.Namespace.Equals(assembly) && t.BaseType.Name.Equals(typeof(SqliteDataTableBase).Name) && t.IsClass)
                    .Select(tt => tt).ToList();
                foreach (var t in types)
                {
                    string tablename = SqliteTabelCmdStrTool.GetTableName(t);
                    if (!CheckTable(tablename))
                    {

                        string cmdStr = SqliteTabelCmdStrTool.GetCreateTableCmdStr(t, true, true);
                        OperateRecords(cmdStr);
                    }
                }
                return true;
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("检查所有的表并创建没有的表时异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        internal DataTable OperateQuery(string cmdStr)
        {
            try
            {
                DataTable data = new DataTable();
                using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
                {
                    SqliteDataAdapter adapter = new SqliteDataAdapter(cmd);
                    adapter.Fill(data);
                }
                return data;
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("执行查询命令出现异常", e.Message);
                return null;
            }
        }

        /// <summary>
        /// 执行sql非查询语句
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        internal int OperateRecords(string cmdStr)
        {
            try
            {
                int res = 0;
                using (SqliteCommand cmd = new SqliteCommand(cmdStr, connection))
                {
                    res = cmd.ExecuteNonQuery();
                }
                return res;
            }
            catch (SqliteException e)
            {
                LogOperator.AddWarnningRecord("执行非查询语句时异常", e.Message);
                return 0;
            }
        }

        #endregion
    }
}