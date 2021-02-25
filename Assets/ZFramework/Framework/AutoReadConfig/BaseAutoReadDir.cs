using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自读取配置文件的文件夹主路径
    /// Editor模式下为StreamingAssets
    /// 真机模式下为PersistentDataPath
    /// </summary>
    [Serializable]
    public class BaseAutoReadDir<T> where T : class, new()
    {
        /// <summary>
        /// 单例中的实例
        /// </summary>
        protected static T instance = null;

        /// <summary>
        /// 主文件路径下所有配置文件所在文件夹的文件夹名字
        /// </summary>
        protected const string ConfigDir = "ZFrameworkConfig";

        /// <summary>
        /// 相同类型文件的文件夹名字
        /// </summary>
        protected static string dirName = null;

        /// <summary>
        /// 后缀名
        /// </summary>
        protected static string extension = null;

        /// <summary>
        /// 不同平台下的路径
        /// </summary>
        protected static string platformPath = null;
    }
}