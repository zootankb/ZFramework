using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 配置路径
    /// </summary>
    public class ConfigPath 
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string FrameworkNamespace = "ZFramework.App";

        /// <summary>
        /// ui预制体路径
        /// </summary>
        public string UIPrePath = "Art/UI";

        /// <summary>
        /// ui脚本路径
        /// </summary>
        public string UIScriptPath = "Scripts/UI";

        /// <summary>
        /// ab包路径
        /// </summary>
        public string AssetbundlePath = "StreamingAssets/Assetbundles";
    }
}