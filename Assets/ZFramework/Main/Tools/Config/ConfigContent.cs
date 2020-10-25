using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework
{
    /// <summary>
    /// 配置内容
    /// </summary>
    public static class ConfigContent
    {
        /// <summary>
        /// 固定json文件的名字
        /// </summary>
        public readonly static string NET_URL_FILE_PATH = string.Format("{0}/NET_URL_CONFIG.json", Application.streamingAssetsPath);

        /// <summary>
        /// 配置文件所在文件夹
        /// </summary>
        public static readonly string CONFIG_FILE_PATH = string.Format("{0}/FRAME_DIR_CONFIG.json", Application.streamingAssetsPath);

        /// <summary>
        /// 资源配置路径
        /// </summary>
        public readonly static ConfigPath configPath = null;

        /// <summary>
        /// 服务器地址配置路径
        /// </summary>
        public readonly static ConfigURL configURL = null;

        /// <summary>
        /// 框架支持的全部平台
        /// </summary>
        public readonly static string[] platforms = new string[] { "Windows", "Andriod", "IOS" };

        static ConfigContent()
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                string jsonStr = CONFIG_FILE_PATH.GetTextAssetContentStr();
                configPath = jsonStr.ToNewtonObjectT<ConfigPath>();
            }
            else
            {
                configPath = new ConfigPath();
            }
            if (File.Exists(NET_URL_FILE_PATH))
            {
                string jsonStr = NET_URL_FILE_PATH.GetTextAssetContentStr();
                configURL = jsonStr.ToNewtonObjectT<ConfigURL>();
            }
            else
            {
                configURL = new ConfigURL();
            }
        }

        /// <summary>
        /// 当前运行的平台,默认为windows开发平台
        /// </summary>
        public static string CurrPlatform
        {
            get
            {
                string platform = null;
#if UNITY_EDITOR
                platform = platforms[0];
#elif UNITY_STANDALONE_WIN
                platform = platforms[0];
#elif UNITY_ANDROID
                platform = platforms[1];
#elif UNITY_IPHONE
                platform = platforms[2];
#endif
                return platform;
            }
        }

        /// <summary>
        /// 获取ab包的路径
        /// </summary>
        /// <returns></returns>
        public static string GetABDir()
        {
            return string.Format("{0}/{1}", configPath.AssetbundlePath, CurrPlatform);
        }
    }
}
