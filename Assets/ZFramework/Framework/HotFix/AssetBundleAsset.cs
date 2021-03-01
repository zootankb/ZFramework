using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.HotFix
{
    /// <summary>
    /// AB包的对象
    /// </summary>
    [System.Serializable]
    public class AssetBundleAsset
    {
        /// <summary>
        /// ab包的名字，同时也是ab包的manifest的名字
        /// </summary>
        public string assetName = null;
        /// <summary>
        /// ab包的crc
        /// </summary>
        public string crc = null;
    }

    /// <summary>
    /// AssetBundleAsset的比较器
    /// </summary>
    public class AssetBundleAssetComparer:IEqualityComparer<AssetBundleAsset>
    {
        public bool Equals(AssetBundleAsset x, AssetBundleAsset y)
        {
            return x.assetName == y.assetName && x.crc == y.crc;
        }

        public int GetHashCode(AssetBundleAsset obj)
        {
            return obj.assetName.GetHashCode() ^ obj.crc.GetHashCode();
        }
    }
}