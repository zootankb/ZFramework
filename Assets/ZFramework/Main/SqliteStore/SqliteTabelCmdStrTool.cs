using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 静态类：主要功能为数据库的命令拼接、类型转换字符、表明获取
    /// </summary>
    internal static class SqliteTabelCmdStrTool
    {
        #region 创建表
        /// <summary>
        /// 获取创建表的字符命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idSelfAdded">如果含有id的话，是否自增</param>
        /// <returns></returns>
        public static string GetCreateTableCmdStr<T>(bool idSelfAdded = false) where T : SqliteDataTableBase
        {
            return GetCreateTableCmdStr(typeof(T), idSelfAdded);
        }

        /// <summary>
        /// 获取创建表的字符命令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="setIdPK">如果含有id的话，设置id是否为主键</param>
        /// <param name="idSelfAdded">如果含有id的话，是否自增</param>
        /// <returns></returns>
        public static string GetCreateTableCmdStr(Type type, bool setIdPK = true, bool idSelfAdded = false)
        {
            string content = string.Format("create table {0}(", GetTableName(type));
            FieldInfo[] ifs = type.GetFields();
            for (int i = 0; i < ifs.Length; i++)
            {
                string fType = CSFieldTypeToDB(ifs[i].FieldType);
                string fName = ifs[i].Name;
                string tmp = string.Empty;
                string point = string.Empty;
                if (ifs.Length == 1)
                {
                    point = string.Empty;
                }
                else
                {
                    point = i + 1 < ifs.Length ? "," : string.Empty;
                }
                if (fName.ToLower().Equals("id"))
                {
                    if (setIdPK)
                    {
                        if (idSelfAdded)
                        {
                            // 使用代码实现默认不自增
                            tmp = string.Format("{0} INTEGER PRIMARY KEY AUTOINCREMENT {1}", fName, point);
                        }
                        else
                        {
                            // 使用代码实现自增
                            tmp = string.Format("{0} {1} PRIMARY KEY{2}", fName, fType, point);
                        }
                    }
                    else
                    {
                        tmp = string.Format("{0} {1} {2}", fName, fType, point);
                    }
                }
                else
                {
                    tmp = string.Format("{0} {1} {2}", fName, fType, point);
                }
                content += tmp;
            }
            content += ");";
            return content;
        }
        #endregion

        #region 表名字
        /// <summary>
        /// 统一获取表的名字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTableName<T>() where T : SqliteDataTableBase
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// 统一获取表的名字
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            return type.Name;
        }
        #endregion

        #region 删除空白页
        /// <summary>
        /// 自动删除空白页空间命令
        /// </summary>
        /// <returns></returns>
        public static string GetAutoVacuum()
        {
            return "PRAGMA auto_vacuum = 1;";
        }
        #endregion

        #region 检查表存在
        /// <summary>
        /// 检查是否存在表命令，不管表名字大小写，运行后存在就返回1，不存在就返回0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetCheckNameExist<T>() where T : SqliteDataTableBase
        {
            return GetCheckNameExist(GetTableName(typeof(T)));
        }

        /// <summary>
        /// 检查是否存在表命令，不管表名字大小写，运行后存在就返回1，不存在就返回0
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string GetCheckNameExist(string tablename)
        {
            return  string.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}' COLLATE NOCASE;", tablename);
        }
        #endregion

        #region 插入数据
        /// <summary>
        /// 获取插入数据的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="idSelfAdded">如果有id字段，是否是自增长</param>
        /// <returns></returns>
        public static string GetInsertStr<T>(T t, bool idSelfAdded = false) where T : SqliteDataTableBase
        {
            return GetInsertStr(new List<T> { t }, idSelfAdded);
        }

        /// <summary>
        /// 获取插入数据的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="idSelfAdded">如果有id字段，是否是自增长</param>
        /// <returns></returns>
        public static string GetInsertStr<T>(List<T> ts, bool idSelfAdded = false) where T : SqliteDataTableBase
        {
            List<string> fNames = typeof(T).GetFields().Select(t => string.Format("'{0}'", t.Name.ToLower())).ToList();
            // 有自增长字段id，就把id去掉
            if (idSelfAdded && fNames.Contains("'id'"))
            {
                fNames.Remove("'id'");
            }
            string content = string.Format("INSERT INTO {0}({1}) VALUES", GetTableName<T>(),string.Join(",", fNames));
            List<string> childRecordStr = new List<string>();
            foreach (var t in ts)
            {
                // 每一条记录的value
                List<string> childContent = new List<string>();
                FieldInfo[] childFis = t.GetType().GetFields();
                foreach (var childFi in childFis)
                {
                    // 特殊处理主键id
                    if (childFi.Name.ToLower().Equals("id"))
                    {
                        // 不是自增长就把值添加进去
                        if (!idSelfAdded)
                        {
                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                    }
                    else
                    {
                        if (childFi.FieldType == typeof(Int32))
                        {

                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                        else if (childFi.FieldType == typeof(String))
                        {
                            object valObj = childFi.GetValue(t);
                            if (valObj != null)
                                childContent.Add(string.Format("'{0}'", childFi.GetValue(t).ToString()));
                            else
                                childContent.Add("''");
                        }
                        else if (childFi.FieldType == typeof(Single))
                        {
                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                        else if (childFi.FieldType == typeof(Boolean))
                        {
                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                        else if (childFi.FieldType == typeof(Double))
                        {
                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                        else
                        {
                            // 默认以数字形式添加
                            childContent.Add(childFi.GetValue(t).ToString());
                        }
                    }
                }
                childRecordStr.Add(string.Format("({0})", string.Join(",", childContent)));
            }
            content += string.Join(",", childRecordStr) + ";";
            return content;
        }

        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="idSelfAdded">如果有id字段，是否是主键</param>
        /// <returns></returns>
        public static string GetDeleteStr<T>(T t, bool hasIdPK = false) where T : SqliteDataTableBase
        {
            return GetInsertStr(new List<T> { t }, hasIdPK);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="hasIdPK">如果有id字段，是否是主键</param>
        /// <returns></returns>
        public static string GetDeleteStr<T>(List<T> ts, bool hasIdPK = false) where T : SqliteDataTableBase
        {
            string content = string.Format("delete from {0} where ", GetTableName<T>());
            List<string> fNames = typeof(T).GetFields().Select(t => string.Format("{0}", t.Name.ToLower())).ToList();
            if(fNames.Contains("id") && hasIdPK)
            {
                // 只记录id就行
                List<string> ids = new List<string>();
                foreach (var t in ts)
                {
                    List<FieldInfo> fis = t.GetType().GetFields().Where(tt => tt.Name.ToLower().Equals("id")).Select(tc => tc).ToList();
                    if(fis != null && fis.Count > 0)
                    {
                        var fi = fis.First();
                        ids.Add(GetFieldValue(fi.Name, fi.FieldType, fi.GetValue(t)));
                    }
                }
                content += string.Format("id in ({0});", string.Join(",", ids));
            }
            else
            {
                // 存储所有实例中的所有字段名和值的集合
                List<string> recordsCmd = new List<string>();
                // 需要匹配各种属性的字段
                foreach (var t in ts)
                {
                    // 存储一个实例中所有字段名字和值的集合
                    List<string> everyFi = new List<string>();
                    FieldInfo[] fis = t.GetType().GetFields();
                    foreach (var fi in fis)
                    {
                        // 一个字段和值的存储
                        List<object> arr = t.GetPKWithNoIDPK();
                        everyFi.Add(GetFieldKeyValue(arr[0].ToString(), arr[1] as Type, arr[2]));
                    }

                    recordsCmd.Add(string.Format("{0}{1};", content, string.Join(" and ", everyFi)));
                }
                content = string.Join(string.Empty, recordsCmd);
            }
            return content;
        }


        #endregion

        #region 更新数据
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="hasIdPK">是否有主键id</param>
        /// <param name="args">当hasIdPK为false的时候，传的筛选条件，key为字段名字，value为字段值，值全部转为sqlite识别的数据类型</param>
        /// <returns></returns>
        public static string GetUpdateStr<T>(T t, bool hasIdPK = false, Dictionary<string, string> args = null) where T : SqliteDataTableBase
        {
            return GetUpdateStr(new List<T> { t }, hasIdPK, args);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="hasIdPK">是否有主键id</param>
        /// <param name="args">当hasIdPK为false的时候，传的筛选条件，key为字段名字，value为字段值，值全部转为sqlite识别的数据类型</param>
        /// <returns></returns>
        public static string GetUpdateStr<T>(List<T> ts, bool hasIdPK = false, Dictionary<string, string> args = null) where T : SqliteDataTableBase
        {
            string content = string.Empty;
            string tableName = GetTableName<T>();
            if (hasIdPK)
            {
                List<string> recordCmds = new List<string>();
                foreach (var t in ts)
                {
                    List<string> everyFi = new List<string>(0);
                    string idFi = string.Empty;
                    FieldInfo[] fis = t.GetType().GetFields();
                    foreach (var fi in fis)
                    {
                        // 排除id字段
                        if (fi.Name.ToLower().Equals("id"))
                        {
                            idFi = GetFieldKeyValue(fi.Name, fi.FieldType, fi.GetValue(t));
                        }
                        else
                        {
                            // 一个字段和值的存储
                            string result = GetFieldKeyValue(fi.Name, fi.FieldType, fi.GetValue(t));
                            everyFi.Add(result);
                        }
                    }
                    recordCmds.Add(string.Format("update {0} set {1} where {2};", tableName, string.Join(", ", everyFi), idFi));
                }
                content = string.Join(string.Empty, recordCmds);
            }
            else
            {
                string whereStr = string.Empty;
                if(args != null && args.Count > 0)
                {
                    whereStr = string.Join(" and ", args.Select(kv => string.Format("{0} = {1}", kv.Key, kv.Value)));
                }
                List<string> recordCmds = new List<string>();
                foreach (var t in ts)
                {
                    List<string> everyFi = new List<string>(0);
                    FieldInfo[] fis = t.GetType().GetFields();
                    foreach (var fi in fis)
                    {
                        // 一个字段和值的存储
                        everyFi.Add(GetFieldKeyValue(fi.Name, fi.FieldType, fi.GetValue(t)));
                    }
                    recordCmds.Add(string.Format("update {0} set {1} {2};", tableName, string.Join(", ", everyFi), string.IsNullOrEmpty(whereStr) ? string.Empty : string.Format(" where {0}", whereStr)));
                }
                content = string.Join(string.Empty, recordCmds);
            }
            return content;
        }
        #endregion

        #region 查询数据

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">筛选条件，条件里的值一定要先做好调整</param>
        /// <returns></returns>
        public static string GetSelectStr<T>(Dictionary<string, string> args = null) where T:SqliteDataTableBase
        {
            string tableName = GetTableName<T>();
            string content = string.Format("select * from {0} ", tableName);
            // 因为都要使用筛选条件，所以先处理筛选条件
            if(args != null && args.Count > 0)
            {
                content += string.Format("where {0};", string.Join(" and ", args.Select(arg => string.Format("{0} in ({1})", arg.Key, arg.Value))));
            }
            else
            {
                content += ";";
            }
            return content;
        }

        #endregion

        #region 检查是否存在主键自主增长字段id
        /// <summary>
        /// 获取查询是否存在主键自主增长字段id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetAutoIncrement<T>() where T : SqliteDataTableBase
        {
            return GetAutoIncrement(typeof(T));
        }

        /// <summary>
        /// 获取查询是否存在主键自主增长字段id
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAutoIncrement(Type type)
        {
            return GetAutoIncrement(GetTableName(type));
        }

        /// <summary>
        /// 获取查询是否存在主键自主增长字段id
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string GetAutoIncrement(string tablename)
        {
            return string.Format("Select sql From MAIN.[sqlite_master] WHERE name = '{0}';", tablename);
        }

        #endregion

        #region 转换类型
        /// <summary>
        /// 根据字段的类型，获取sqlite中的值
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="fType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string GetFieldValue(string fName, Type fType, object value)
        {
            string result = GetFieldKeyValue(fName,fType, value);
            return result.Split('=')[1].Trim();
        }

        /// <summary>
        /// 根据字段的类型，获取sqlite中的值
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="fType"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private static string GetFieldKeyValue(string fName, Type fType, object fieldValue)
        {
            string result = string.Format("{0} = ", fName);
            if (fType == typeof(Int32))
            {
                result += (int)fieldValue;
            }
            else if (fType == typeof(Int64))
            {
                result += (long)fieldValue;
            }
            else if (fType == typeof(String))
            {
                result += fieldValue == null ? "''" : string.Format("'{0}'", fieldValue.ToString());
            }
            else if (fType == typeof(Single))
            {
                result += (float)fieldValue;
            }
            else if (fType == typeof(Boolean))
            {
                result += (bool)fieldValue;
            }
            else if (fType == typeof(Double))
            {
                result += (double)fieldValue;
            }
            else
            {
                // 找不到的就直接显示
                result += fieldValue.ToString();
            }
            return result;
        }

        /// <summary>
        /// cs中的类型转为sqliteDB中的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CSFieldTypeToDB(Type type)
        {
            string result = "Text";
            if (type == typeof(Int32))
            {
                result = "Int";
            }
            else if (type == typeof(Int64))
            {
                result = "Integer";
            }
            else if (type == typeof(String))
            {
                result = "Text";
            }
            else if (type == typeof(Single))
            {
                result = "Float";
            }
            else if (type == typeof(Boolean))
            {
                result = "Bool";
            }
            else if (type == typeof(Double))
            {
                result = "Double";
            }
            return result;
        }

        /// <summary>
        /// 数据库字段类型转换为CS的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string DBFieldTypeToCS(Type type)
        {
            // TODO
            return null;
        }
        #endregion
    }
}