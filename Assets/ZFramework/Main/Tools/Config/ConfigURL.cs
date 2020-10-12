using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// url的路径内容
    /// </summary>
    public class ConfigURL 
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string APIHost = "http://127.0.0.1:8099";
        /// <summary>
        /// 服务器中ab包的主文件
        /// </summary>
        public string ManifestHost = "http://127.0.0.1:8000/static/Assetbundle.manifext";
        /// <summary>
        /// 资源服务器路径
        /// </summary>
        public string ResHost = "http://127.0.0.1:8000/static";
    }
}