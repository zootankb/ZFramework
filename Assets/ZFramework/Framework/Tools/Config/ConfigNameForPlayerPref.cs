using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 框架中使用到的各种playerprefs的key值
    /// </summary>
    public static class ConfigNameForPlayerPref
    {
        /// <summary>
        /// 选择是从本地的prefab文件夹里直接获取，还是从ab包里获取
        /// </summary>
        public const string EDITOR_UI_USE_FROM_PREFAB = "EDITOR_UI_USE_FROM_PREFAB";

        /// <summary>
        /// 存储选取的物体的信息key值，由EditorPrefs使用
        /// </summary>
        public const string EDITOR_AB_INFO_KEY = "EDITOR_AB_INFO_KEY";
    }
}