using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 记录本地数据的表
    /// </summary>
    internal class LocalDataTable : SqliteDataTableBase
    {
        #region Enum
        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// 整形
            /// </summary>
            Int = 1,
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// byte
            /// </summary>
            Byte,
            /// <summary>
            /// 单精度
            /// </summary>
            Float,
            /// <summary>
            /// 布尔值
            /// </summary>
            Bool,
            /// <summary>
            /// 双精度
            /// </summary>
            Double
        }

        #endregion

        #region Field
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string idStr;
        /// <summary>
        /// 内容
        /// </summary>
        public string content;
        /// <summary>
        /// 数据类型
        /// </summary>
        public int dataType;

        #endregion

        #region Constructor

        public LocalDataTable() : this(null) 
        {

        }

        public LocalDataTable(string idStr) : this(idStr, null)
        {
        }

        public LocalDataTable(string idStr, string content) : this(idStr, content, 0)
        {
        }

        public LocalDataTable(string idStr, string content, int dataType) : this(idStr, content, 0, null)
        {
        }

        public LocalDataTable(string idStr, string content, int dataType, string remark) : this(idStr, content, 0, remark, null)
        {
        }

        public LocalDataTable(string idStr, string content, int dataType, string remark, string timestemp):base(remark, timestemp)
        {
            this.idStr = idStr;
            this.content = content;
            this.dataType = dataType;
        }

        #endregion

        /// <summary>
        /// 获取没有主键字段id的主键名字、类型和值，一次存到list中
        /// 需要在没有字段id的表类中重写，用来手动指定
        /// </summary>
        /// <returns></returns>
        public override List<object> GetPKWithNoIDPK()
        {
            return new List<object> { "idStr", idStr.GetType(), idStr };
        }
    }
}