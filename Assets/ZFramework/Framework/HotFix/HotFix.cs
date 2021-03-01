using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Net;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 热更新的主要代码
    /// </summary>
    public static class HotFix
    {
        #region Data
        /// <summary>
        /// 当前更新的进度
        /// </summary>
        private static UnityAction<CurrentProgress> curResultProgress = null;

        /// <summary>
        /// 本地的资源列表的清单
        /// </summary>
        public static AssetBundleAssetList localResList = null;

        /// <summary>
        /// 服务器中的资源列表的清单
        /// </summary>
        public static AssetBundleAssetList netResList = null;

        /// <summary>
        /// 已经下载的资源列表的清单【版本号和crc不同时重新全部更新】
        /// </summary>
        public static AssetBundleAssetList downloadedResList = null;
        #endregion

        #region API
        /// <summary>
        /// 添加进度事件
        /// </summary>
        /// <param name="curProgress"></param>
        public static void AddProgressListenr(UnityAction<CurrentProgress> curProgress)
        {
            curResultProgress += curProgress;
        }

        /// <summary>
        /// 移除进度事件
        /// </summary>
        /// <param name="curProgress"></param>
        public static void RemoveProgressListenr(UnityAction<CurrentProgress> curProgress)
        {
            curResultProgress -= curProgress;
        }

        /// <summary>
        /// 开始更新
        /// </summary>
        public static void StartHotFix()
        {
            // TODO
            // 准备一个服务器、打包好的ab包资源、资源列表清单
        }
        #endregion

        #region Func


        /// <summary>
        /// 获取服务器中文件的大小
        /// </summary>
        /// <param name="downloadUrl"></param>
        /// <returns></returns>
        private static long GetNetFileLength(string downloadUrl)
        {
            long length = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);

                request.MaximumAutomaticRedirections = 4;

                request.MaximumResponseHeadersLength = 4;

                request.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                length = response.ContentLength;

                response.Close();
            }
            catch (Exception e)
            {
                ZFramework.Log.LogOperator.AddResErrorRecord("下载资源时出错", e.Message);
            }
            return length;
        }
        #endregion
    }
}