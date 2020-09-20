using System.Collections;
using System.Collections.Generic;

namespace ZFramework.SqliteStore
{
    /// <summary>
    /// 记录下载文件的表
    /// </summary>
    internal class NetFileTable : SqliteDataTableBase
    {
        #region Enum
        /// <summary>
        /// 存储到本地的文件类型
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// ab包
            /// </summary>
            Assetbundle = 1,
            /// <summary>
            /// 图片和精灵图都为此
            /// </summary>
            Texture2D,
            /// <summary>
            /// Textasset,包括json、xml、txt等
            /// </summary>
            TextAsset,
            /// <summary>
            /// 声音
            /// </summary>
            AudioClip,
            /// <summary>
            /// 视频
            /// </summary>
            VideoClip
        }

        public enum FileDownloadState
        {
            /// <summary>
            ///未下载
            /// </summary>
            Undownload = 1,
            /// <summary>
            /// 正在下载
            /// </summary>
            Downloading,
            /// <summary>
            /// 已经下载
            /// </summary>
            Downloaded,
        }
        #endregion Enum

        #region Field
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int id;
        /// <summary>
        /// 文件类型，对应枚举文件类型的状态
        /// </summary>
        public int filetype;
        /// <summary>
        /// 存储路径
        /// </summary>
        public string path;
        /// <summary>
        /// 版本
        /// </summary>
        public string copyright;
        /// <summary>
        /// 下载状态，对应枚举状态
        /// </summary>
        public int state;
        #endregion

        #region Constructor

        public NetFileTable() : base()
        {

        }

        public NetFileTable(int id) :this(id, 0)
        {

        }

        public NetFileTable(int id, int filetype) : this(id, filetype, null)
        {
        }

        public NetFileTable(int id, int filetype, string path) : this(id, filetype, path, null)
        {
        }

        public NetFileTable(int id, int filetype, string path, string copyright) : this(id, filetype, path, copyright, 0)
        {
        }

        public NetFileTable(int id, int filetype, string path, string copyright, int state) : this(id, filetype, path, copyright, state, null)
        {
        }

        public NetFileTable(int id, int filetype, string path, string copyright, int state, string remark) : this(id, filetype, path, copyright, state, remark, null)
        {
        }

        public NetFileTable(int id, int filetype, string path, string copyright , int state, string remark , string timestemp)
            : base(remark, timestemp)
        {
            this.id = id;
            this.filetype = filetype;
            this.path = path;
            this.copyright = copyright;
            this.state = state;
        }

        #endregion
    }
}