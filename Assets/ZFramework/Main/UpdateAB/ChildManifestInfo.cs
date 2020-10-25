using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UpdateAB
{
    /// <summary>
    /// 子ab包的信息（主要解析ab包的manifest内容）
    /// </summary>
    public class ChildManifestInfo
    {
        /// <summary>
        /// info的名字，格式为Info_XXX
        /// </summary>
        public string infoName;
        /// <summary>
        /// ab包的名字，格式为xxx.ab
        /// </summary>
        public string name;
        /// <summary>
        /// ab包依赖的资源名字，暂时以字符串存储，后续有需要再做处理
        /// </summary>
        public string dependencies;

        public ChildManifestInfo(string infoName = null, string name = null, string dependencies = null)
        {
            this.infoName = infoName;
            this.name = name;
            this.dependencies = dependencies;
        }
    }
}