using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 下载的当前进度详情
    /// </summary>
    public class CurrentProgress
    {
        /// <summary>
        /// 总文件大小
        /// </summary>
        public int totalFileCount = 0;
        /// <summary>
        /// 已经下载多少
        /// </summary>
        public int downloadedCount = 0;
        /// <summary>
        /// 当前ab包的名字
        /// </summary>
        public string currentAssetName = null;
        /// <summary>
        /// 已经下载的大小
        /// </summary>
        public int downloadedSize = 0;
        /// <summary>
        /// 文件总共大小
        /// </summary>
        public int totalSize = 0;
        /// <summary>
        /// 当前网速
        /// </summary>
        public float currentNetSpeed = 0;
    }
}