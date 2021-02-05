using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自读取配置结构的行为
    /// </summary>
    public interface IAutoReadConfigFunc<T> where T : class, new()
    {
        /// <summary>
        /// 创建实例文件并存储，有的话就覆盖
        /// </summary>
        void SaveTIfExist();
    }
}