using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 基础操作数据
    /// </summary>
    internal class SqliteDataTableBase
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string remark = null;
        /// <summary>
        /// 操作时间，时间格式为 yyyy-MM-DD HH:mm:ss:fff
        /// </summary>
        public string timestemp = null;

        public SqliteDataTableBase() : this(null)
        {
            // pass
        }

        public SqliteDataTableBase(string remark):this(remark, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"))
        {
            // pass
        }

        public SqliteDataTableBase(string remark, string timestemp)
        {
            this.remark = remark;
            this.timestemp = string.IsNullOrEmpty(timestemp) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") : timestemp;
        }

        /// <summary>
        /// 获取没有主键字段id的主键名字、类型和值，一次存到list中
        /// 需要在没有字段id的表类中重写，用来手动指定
        /// 有id字段的就不需要用此函数
        /// </summary>
        /// <returns></returns>
        public virtual List<object> GetPKWithNoIDPK()
        {
            return null;
        }
    }
}