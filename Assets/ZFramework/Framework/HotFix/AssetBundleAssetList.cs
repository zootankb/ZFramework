using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 要下载的ab包的清单
    /// </summary>
    public class AssetBundleAssetList
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string copyRight = null;
        /// <summary>
        /// 主要的crc
        /// </summary>
        public string mainCrc = null;
        /// <summary>
        /// 资源列表
        /// </summary>
        public List<AssetBundleAsset> assets = null;
    }
}