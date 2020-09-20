using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Res
{
    /// <summary>
    /// 本地资源路径
    /// </summary>
    public static class LocalResPath 
    {
        /// <summary>
        /// ab包存储
        /// </summary>
        public readonly static string DIR_ASSETBUNDLE_PATH = string.Format("{0}/Res/Assetbundles/", Application.persistentDataPath);

        /// <summary>
        /// texture2d和sprite存储存储
        /// </summary>
        public readonly static string DIR_TEXTURE2D_PATH = string.Format("{0}/Res/Textures/", Application.persistentDataPath);

        /// <summary>
        /// 文本文件（txt、json、xml、）的资源存储
        /// </summary>
        public readonly static string DIR_TEXTASSET_PATH = string.Format("{0}/Res/TextAssets/", Application.persistentDataPath);

        /// <summary>
        /// 声音存储
        /// </summary>
        public readonly static string DIR_AUDIOCLIP_PATH = string.Format("{0}/Res/AudioClips/", Application.persistentDataPath);

        /// <summary>
        /// 视频存储
        /// </summary>
        public readonly static string DIR_VIDEOCLIP_PATH = string.Format("{0}/Res/VideoClips/", Application.persistentDataPath);
    }
}