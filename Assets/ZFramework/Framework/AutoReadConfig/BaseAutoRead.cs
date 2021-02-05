using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.ClassExt;
using System.IO;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自动读取配置结构的行为
    /// </summary>
    public class BaseAutoRead<T> : BaseAutoReadDir<T>where T : class, new()
    {
        /// <summary>
        /// 本层中实现
        /// </summary>
        public static string FileName { get { return typeof(T).Name; } }

        /// <summary>
        /// 子类中实现
        /// </summary>
        public static string FileNameWithExtension { get { return string.Format("{0}.{1}",FileName, extension); } }

        /// <summary>
        /// 
        /// </summary>
        public static string RelativePath { get { return Path.Combine( ConfigDir, dirName, FileNameWithExtension); } }

        /// <summary>
        /// 本层中实现
        /// </summary>
        public static string AbsPath { get { return Path.Combine(platformPath, RelativePath); } }

        /// <summary>
        /// 是否存在此实例类型的json文件
        /// </summary>
        public static bool HasFile { get { return File.Exists(AbsPath); } }

    }
}