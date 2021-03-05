using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ZFramework.ClassExt;
using ZFramework.Res;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 资源更新需要的文件内容
    /// </summary>
    public static class HotFixConfig
    {
        #region Data
        /// <summary>
        /// 资源列表文件名
        /// </summary>
        public const string LocalResListFileName = "resList.json";

        /// <summary>
        /// 文件夹
        /// </summary>
        public static string HotFixConfigDir = LocalResPath.DIR_ASSETBUNDLE_PATH;

        /// <summary>
        /// 本地存储资源列表的名字
        /// </summary>
        public static string LocalResListFilePath = Path.Combine(HotFixConfigDir, LocalResListFileName);

        /// <summary>
        /// 本地存储已经下载好的资源列表的名字
        /// </summary>
        public static string downloadedResListFilePath = Path.Combine(HotFixConfigDir, "downloadedResList.json");

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
        /// 本地的资源列表的清单,不存在就new一个
        /// </summary>
        public static AssetBundleAssetList LocalResList
        {
            get
            {
                if (localResList == null || string.IsNullOrEmpty(localResList.copyRight) || string.IsNullOrEmpty(localResList.mainCrc))
                {
                    if (File.Exists(LocalResListFilePath))
                    {
                        string jsonContent = LocalResListFilePath.GetTextAssetContentStr();
                        localResList = jsonContent.JsonToTObject<AssetBundleAssetList>();
                    }
                    else
                    {
                        localResList = new AssetBundleAssetList() { assets = new List<AssetBundleAsset>() };
                        SaveLocalResList(localResList);
                    }
                }
                return localResList;
            }
        }

        /// <summary>
        /// 已经下载的资源列表的清单，不存在就new一个【版本号和crc不同时重新全部更新】
        /// </summary>
        public static AssetBundleAssetList DownloadedResList
        {
            get
            {
                if (downloadedResList == null || string.IsNullOrEmpty(downloadedResList.copyRight) || string.IsNullOrEmpty(downloadedResList.mainCrc))
                {
                    if (File.Exists(downloadedResListFilePath))
                    {
                        string jsonContent = downloadedResListFilePath.GetTextAssetContentStr();
                        downloadedResList = jsonContent.JsonToTObject<AssetBundleAssetList>();
                    }
                    else
                    {
                        downloadedResList = new AssetBundleAssetList() { assets = new List<AssetBundleAsset>() };
                        SaveDownloadedResList();
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
            string jsonContent = netResList.ToNewtonJson();
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
                var res = downloadedResList.assets.Where(p => p.assetName.Equals(record.assetName)).ToList();
                if(res.Count == 0)
                {
                    downloadedResList.assets.Add(record);
                }
                else
                {
                    res.First().crc = record.crc;
                }
                string jsonContent = downloadedResList.ToNewtonJson();
                downloadedResListFilePath.WriteTextAssetContentStr(jsonContent);
            }
        }

        /// <summary>
        /// 把更新完的资源并存储到本地
        /// </summary>
        public static void SaveDownloadedResList()
        {
            string jsonContent = downloadedResList.ToNewtonJson();
            downloadedResListFilePath.WriteTextAssetContentStr(jsonContent);
        }

        /// <summary>
        /// 比较两个列表资源的差异结果，并做返回
        /// </summary>
        /// <param name="leftList"></param>
        /// <param name="rightList"></param>
        /// <returns></returns>
        public static List<AssetBundleAsset> CheckDifference(List<AssetBundleAsset> leftList, List<AssetBundleAsset> rightList)
        {
            var res = leftList.Except(rightList, new AssetBundleAssetComparer());
            return res.ToList();
        }
        #endregion
    }
}