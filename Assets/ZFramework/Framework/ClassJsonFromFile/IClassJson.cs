using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.ClassJsonFromFile
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IClassJson<T> where T:class,new()
    {
        /// <summary>
        /// 直接从json文件中获取指定类型实例
        /// </summary>
        /// <returns></returns>
        T GetT { get; }

        /// <summary>
        /// 文件的名字
        /// </summary>
        string FileName
        {
            get;
        }

        /// <summary>
        /// 文件的名字带后缀
        /// </summary>
        string FileNameWithExtension
        {
            get;
        }

        /// <summary>
        /// 是否存在此实例类型的json文件
        /// </summary>
        /// <returns></returns>
        bool HasFile();

        /// <summary>
        /// 创建实例文件并存储，有的话就覆盖
        /// </summary>
        void SaveTIfExist();
    }
}