using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;
using ZFramework.ClassExt;
using ZFramework.Res;

namespace ZFramework.HotFix
{
    /// <summary>
    /// 热更新的主要代码
    /// </summary>
    public static class HotFix
    {
        #region Private Data

        /// <summary>
        /// 服务器中的资源列表的清单
        /// </summary>
        public static AssetBundleAssetList netResList = null;

        /// <summary>
        /// 本地的资源列表的清单
        /// </summary>
        public static AssetBundleAssetList localResList = null;

        /// <summary>
        /// 需要下载的资源列表
        /// </summary>
        public static AssetBundleAssetList needToDownloadResList = null;

        /// <summary>
        /// 已经下载的资源列表的清单【版本号和crc不同时重新全部更新】
        /// </summary>
        public static AssetBundleAssetList downloadedResList = null;
        #endregion

        #region Public Data
        public static bool isDone = false;                  // 是否更新完

        public static bool readToHotfix = false;            // 是否准备好更新

        public static long downloadedCount = 0;             // 已经下载了多少文件
        public static long downloadedSize = 0;              // 已经总共下载了多大
        public static long needToDownloadCount = 0;         // 总共多少文件
        public static long needToDownloadTotalSize = 0;     // 总共多大
        public static double totalProgress = 0.0f;          // 总的进度，包括正在下载的部分

        public static string curDownloadAssetName = null;   // 当前文件名字
        public static long curDownloadAssetSize = 0;        // 当前文件已经下载多大
        public static long curDownloadAssetTotalSize = 0;   // 当前文件总共多大
        public static long curDownloadAssetIndex = 0;       // 当前文件为第几个
        public static double currDownloadProgress = 0.0f;   // 当前文件的进度

        #endregion

        #region API
        /// <summary>
        /// 开始更新
        /// </summary>
        public static void StartHotFix()
        {
            // 在线程里面做列表下载、比对、资源下载等一系列操作
            // 在主线程里初始化HotFixConfig
            Debug.Log("主线程里初始化服务器地址：" + HotFixConfig.LocalResListFilePath);
            Thread t = new Thread(HotfixEnter);
            t.IsBackground = true;
            t.Name = typeof(HotFix).Name;
            t.Start();
        }
        #endregion

        #region Func
        /// <summary>
        /// 更新线程进口
        /// </summary>
        private static void HotfixEnter()
        {
            // Debug.Log(ConfigContent.configURL.ManifestHost);

            // 先下载服务器资源列表
            string jsonContent = GetHttpDownloadFile(ConfigContent.configURL.ManifestHost);
            if(!string.IsNullOrEmpty(jsonContent))
            {
                netResList = jsonContent.ToNewtonObjectT<AssetBundleAssetList>();
                Debug.Log(netResList.copyRight);
                // 版本不同需要下载
                if (!netResList.copyRight.Equals(HotFixConfig.LocalResList.copyRight))
                {
                    // 找出差异文件
                    List<AssetBundleAsset> res = HotFixConfig.CheckDifference(netResList.assets, HotFixConfig.LocalResList.assets);
                    Debug.LogFormat("需要下载 {0} 个文件", res.Count);
                    if(res.Count == 0)
                    {
                        // 已经下载完，把文件存储下来
                        HotFixConfig.SaveLocalResList(netResList);
                        isDone = true;
                    }
                    else
                    {
                        // 还没下载完，就要和已经下载的比对下还有哪些没有下载
                        Debug.Log("还没下载完，已经下载了: " + HotFixConfig.DownloadedResList.assets.Count);
                        res = HotFixConfig.CheckDifference(res, HotFixConfig.DownloadedResList.assets);
                        Debug.LogFormat("还有 {0} 个没有下载",res.Count);

                        if(res.Count == 0)
                        {
                            // 已经下载完，把文件存储下来
                            HotFixConfig.SaveLocalResList(netResList);
                            isDone = true;
                        }
                        else
                        {
                            // 没有下载完，准备好继续下载
                            readToHotfix = true;

                            // 有多少个文件、多大
                            needToDownloadCount = res.Count;
                            foreach (var r in res)
                            {
                                // 由服务器接口查询文件大小
                                string rUrl = string.Format("{0}?filename={1}", ConfigContent.configURL.APIHost, r.assetName);
                                needToDownloadTotalSize += GetNetFileLength(rUrl);
                                Debug.Log("文件：" + rUrl + ",  " + needToDownloadTotalSize);
                            }
                            for (int i = 0; i < res.Count; i++)
                            {
                                var r = res[i];
                                string fileSizeUrl = string.Format("{0}?filename={1}", ConfigContent.configURL.APIHost, r.assetName);
                                string fileUrl = string.Format("{0}/{1}", ConfigContent.configURL.ResHost, r.assetName);
                                curDownloadAssetName = r.assetName;
                                curDownloadAssetSize = 0;        // 当前文件已经下载多大
                                curDownloadAssetTotalSize = GetNetFileLength(fileSizeUrl);
                                curDownloadAssetIndex = i + 1;
                                currDownloadProgress = 0.0f;    // 当前文件的进度

                                string localFilePath = Path.Combine(HotFixConfig.HotFixConfigDir, r.assetName);

                                Debug.Log(localFilePath);

                                // 设置参数
                                HttpWebRequest request = WebRequest.Create(fileUrl) as HttpWebRequest;
                                //发送请求并获取相应回应数据
                                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                                using (Stream responseStream = response.GetResponseStream())
                                {
                                    //创建本地文件写入流
                                    using (Stream stream = new FileStream(localFilePath, FileMode.OpenOrCreate))
                                    {
                                        byte[] bArr = new byte[4096];
                                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                                        while (size > 0)
                                        {
                                            curDownloadAssetSize += size;
                                            currDownloadProgress = curDownloadAssetSize * 1.0 / curDownloadAssetTotalSize;
                                            totalProgress = (downloadedSize + curDownloadAssetSize) * 1.0 / needToDownloadTotalSize;
                                            stream.Write(bArr, 0, size);
                                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                                            Debug.Log(totalProgress);
                                        }
                                    }
                                }
                                HotFixConfig.SaveDownloadedResList(r);

                                downloadedCount = i + 1;
                                downloadedSize += curDownloadAssetSize;
                            }
                            HotFixConfig.SaveLocalResList(netResList);
                        }
                    }
                }
                else
                {
                    isDone = true;
                }
            }
            else
            {
                isDone = true;
            }
        }

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
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream read = response.GetResponseStream()) {
                        using(StreamReader sr = new StreamReader(read, Encoding.UTF8))
                        {
                            string result = sr.ReadToEnd();
                            length = long.Parse(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ZFramework.Log.LogOperator.AddResErrorRecord("下载资源时出错", e.Message);
            }
            return length;
        }

        /// <summary>
        /// 获取服务器中文件的内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHttpDownloadFile(string url)
        {
            string jsonContent = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //在这里对接收到的页面内容进行处理
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        jsonContent = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception e)
            {
                ZFramework.Log.LogOperator.AddResErrorRecord("下载资源时出错", e.Message);
            }
            return jsonContent;
        }
        #endregion
    }
}