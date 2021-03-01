using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 资源更新需要的文件内容
    /// </summary>
    public static class HotFixConfig
    {
        #region Data
        /// <summary>
        /// 文件夹
        /// </summary>
        private const string HotFixConfigDir = "HotFix";

        /// <summary>
        /// 本地存储资源列表的名字
        /// </summary>
        private static string LocalResListFilePath = Path.Combine(Application.persistentDataPath, HotFixConfigDir, "resList.json");

        /// <summary>
        /// 本地存储已经下载好的资源列表的名字
        /// </summary>
        private static string downloadedResListFilePath = Path.Combine(Application.persistentDataPath, HotFixConfigDir, "downloadedResList.json");

        /// <summary>
        /// 本地存储的资源列表【json】
        /// </summary>
        private static AssetBundleAssetList localResList = null;

        /// <summary>
        /// 本地存储的已经下载好的资源列表【json】
        /// </summary>
        private static AssetBundleAssetList downloadedResList = null;
        #endregion

        #region API
        /// <summary>
        /// 本地的资源列表的清单
        /// </summary>
        public static AssetBundleAssetList LocalResList
        {
            get
            {
                if (localResList == null)
                {
                    if (File.Exists(LocalResListFilePath))
                    {
                        string jsonContent = LocalResListFilePath.GetTextAssetContentStr();
                        localResList = jsonContent.JsonToTObject<AssetBundleAssetList>();
                    }
                }
                return localResList;
            }
        }

        /// <summary>
        /// 已经下载的资源列表的清单【版本号和crc不同时重新全部更新】
        /// </summary>
        public static AssetBundleAssetList DownloadedResList
        {
            get
            {
                if (downloadedResList == null)
                {
                    if (File.Exists(downloadedResListFilePath))
                    {
                        string jsonContent = downloadedResListFilePath.GetTextAssetContentStr();
                        downloadedResList = jsonContent.JsonToTObject<AssetBundleAssetList>();
                    }
                }
                return downloadedResList;
            }
        }

        /// <summary>
        /// 更新好后把服务器中的资源列表存储到内存中，并存储到本地
        /// </summary>
        /// <param name="netResList"></param>
        public static void SaveLocalResList(AssetBundleAssetList netResList)
        {
            // 把服务器中的资源列表赋值给localResList，并存储到本地
            localResList = netResList;
            string jsonContent = localResList.ToNewtonJson();
            LocalResListFilePath.WriteTextAssetContentStr(jsonContent);
        }

        /// <summary>
        /// 查询是否存在此资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        public static bool HasDownloadFile(string assetName, string crc)
        {
            if (DownloadedResList != null && DownloadedResList.assets != null)
            {
                int count = DownloadedResList.assets.Where(p => p.assetName.Equals(assetName) && p.crc.Equals(crc)).Count();
                return count > 0;
            }
            return false;
        }

        /// <summary>
        /// 每次更新好一个文件后，把更新到的新的资源列表存储到内存中，并存储到本地
        /// </summary>
        /// <param name="record"></param>
        public static void SaveDownloadedResList(AssetBundleAsset record)
        {
            if (downloadedResList != null)
            {
                if(downloadedResList.assets == null)
                {
                    downloadedResList.assets = new List<AssetBundleAsset>();
                }
                downloadedResList.assets.Add(record);
                string jsonContent = downloadedResList.ToNewtonJson();
                downloadedResListFilePath.WriteTextAssetContentStr(jsonContent);
            }
        }

        /// <summary>
        /// 检查网络资源和本地资源差异结果，并做返回，下载差异文件
        /// </summary>
        /// <param name="netList"></param>
        /// <returns></returns>
        public static List<AssetBundleAsset> CheckDifferenceFromLoadResList(AssetBundleAssetList netResList)
        {
            if (DownloadedResList != null && DownloadedResList.assets!=null)
            {
                var res = netResList.assets.Except(DownloadedResList.assets, new AssetBundleAssetComparer());
                return res.ToList();
            }
            else
            {
                return netResList.assets;
            }
        }
        #endregion
    }
}