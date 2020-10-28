using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using ZFramework.Res;
using ZFramework.ClassExt;

namespace ZFramework.UpdateAB
{
    /// <summary>
    /// 更新ab包的主程序
    /// </summary>
    public static class ABUpdate
    {
        #region Data
        /// <summary>
        /// 更新完成后的返回事件
        /// </summary>
        private static Action<bool> fineshedCb = null;
        /// <summary>
        /// 主文件的进度
        /// </summary>
        private static Action<float> maniProgressCb = null;
        /// <summary>
        /// 更新进度,前者为第几个，后面为总数
        /// </summary>
        private static Action<int, int> progressCb = null;
        /// <summary>
        /// 更新失败后的返回事件
        /// 在下载主Manifest文件异常时最多只会返回一次错误
        /// 下载其他manifest文件的时候可能会返回多个异常，因为多个ab包是同时下载的
        /// </summary>
        private static Action<string> errorCb = null;
        /// <summary>
        /// 总体进度
        /// </summary>
        private static Action<float> childInfoCb = null;


        /// <summary>
        ///本地主manifest文件
        /// </summary>
        private static CurrPlatformManifestInfo localManifestInfo = null;

        /// <summary>
        /// 本地manifestInfo文件里存储的子manifest文件信息
        /// </summary>
        private static Dictionary<string, ChildDetailManifestInfo> localDetailInfos = new Dictionary<string, ChildDetailManifestInfo>();

        /// <summary>
        ///网络主manifest文件
        /// </summary>
        private static CurrPlatformManifestInfo netManifestInfo = null;

        /// <summary>
        /// 网络manifestInfo文件里存储的子manifest文件信息
        /// </summary>
        private static Dictionary<string, ChildDetailManifestInfo> netDetailInfos = new Dictionary<string, ChildDetailManifestInfo>();

        /// <summary>
        /// 总共有多少个ab包
        /// </summary>
        private static int totalAbNum = 0;

        /// <summary>
        /// 当前下载好的ab包的数量
        /// </summary>
        private static int currDownloadedNum = 0;

        /// <summary>
        /// 当前完成的下载的数量
        /// </summary>
        private static int currDownloadNum = 0;
        #endregion 

        #region API

        /// <summary>
        /// 开启更新
        /// </summary>
        /// <param name="fineshed">更新完成后的返回事件</param>
        /// <param name="maniProgress">主文件的进度</param>
        /// <param name="progress">更新进度</param>
        /// <param name="error">更新失败后的返回事件</param>
        /// <param name="childInfo">总体进度</param>
        public static void StartUpdate(Action<bool> fineshed = null, Action<float> maniProgress = null, Action<int,int> progress = null,
            Action<string> error = null, Action<float> childInfo = null)
        {
            fineshedCb = fineshed;
            maniProgressCb = maniProgress;
            progressCb = progress;
            errorCb = error;
            childInfoCb = childInfo;

            StartManifest();
        }
        #endregion

        #region Pri Func

        /// <summary>
        /// 开始更新manifest文件
        /// </summary>
        private static void StartManifest()
        {
            string url = ConfigContent.configURL.ManifestHost;
            Action<string, long, byte[], object[]> cb = (eurl, code, bytes, args) =>
            {
                if (bytes != null)
                {
                    string content = Encoding.UTF8.GetString(bytes);
                    netManifestInfo = CurrPlatformManifestInfo.AllocateByContent(content);
                    netDetailInfos = new Dictionary<string, ChildDetailManifestInfo>();
                    UpdateAbs(netManifestInfo.assetBundleInfos);
                }
                else
                {
                    string errMsg = string.Format("从 {0} 下载主文件失败", eurl);
                    errorCb?.Invoke(errMsg);
                    Log.LogOperator.AddNetErrorRecord(errMsg);
                }
            };
            // 开启下载
            NetResMgr.DownloadBytes(url, cb, maniProgressCb);
        }

        /// <summary>
        /// 开始更新ab包
        /// </summary>
        /// <param name="assetBundleInfos"></param>
        private static void UpdateAbs(List<ChildManifestInfo> assetBundleInfos)
        {
            // 因为要下载的文件种类包括manifest文件和ab文件，所以要乘以2
            totalAbNum = assetBundleInfos.Count * 2;
            currDownloadedNum = 0;
            currDownloadNum = 0;
            // 下载Manifest文件
            Action<string, long, byte[], object[]> cb = (eurl, code, bytes, args) =>
            {
                currDownloadNum++;
                if (bytes != null)
                {
                    // manifest下载成功
                    currDownloadedNum++;
                    ChildManifestInfo cmi = args.First() as ChildManifestInfo;
                    string netContent = Encoding.UTF8.GetString(bytes);
                    ChildDetailManifestInfo netCdmi = new ChildDetailManifestInfo(netContent);
                    UpdateAb(netCdmi, cmi);
                }
                else
                {
                    // manifest下载失败
                    string errMsg = string.Format("下载 {0} 时出现异常！", eurl);
                    errorCb?.Invoke(errMsg);
                    Log.LogOperator.AddNetErrorRecord(errMsg);
                }
                progressCb?.Invoke(currDownloadNum, totalAbNum);

            };
            // 单个ab包的进度
            Action<float> progress = (pro) =>
            {
                float p = currDownloadNum * 1.0f / totalAbNum;
                childInfoCb?.Invoke(p);
            };
            // 一次性下载所有的manifest资源
            foreach (var manifest in assetBundleInfos)
            {
                string url = string.Format("{0}/{1}", ConfigContent.configURL.ResHost, manifest.name);
                NetResMgr.DownloadBytes(url, cb, progress, manifest);
            }
        }

        /// <summary>
        /// 更新单个ab包
        /// </summary>
        /// <param name="netCdmi">服务器里详细manifest文件</param>
        /// <param name="cmi">主manifest文件里面的ab包信息</param>
        private static void UpdateAb(ChildDetailManifestInfo netCdmi, ChildManifestInfo cmi)
        {
            bool hasUpdated = true;
            if(netCdmi != null && cmi != null)
            {
                // 和本地对应的的manifest文件做对比
                string localMFPath = string.Format("{0}/{1}.manifest", ConfigContent.GetPerABDir(), cmi.name);
                string localAbPath = string.Format("{0}/{1}", ConfigContent.GetPerABDir(), cmi.name);
                // 检查本地是否存在对应manifest文件
                if (File.Exists(localMFPath))
                {
                    string localContent = localMFPath.GetTextAssetContentStr();
                    ChildDetailManifestInfo localCdmi = new ChildDetailManifestInfo(localContent);
                    if(netCdmi.crc == localCdmi.crc && File.Exists(localAbPath))
                    {
                        hasUpdated = false;
                        currDownloadedNum++;
                        currDownloadNum ++;
                        progressCb?.Invoke(currDownloadNum, totalAbNum);
                        // crc相同，ab包也在，就不需要下载了
                        // 不管crc是否相同，只要两个文件有一个对不上，一律下载
                        // Pass
                    }
                }
            }
            if (!hasUpdated)
            {
                // 下载ab包，同时把manifest的信息做参数传递
                string url = string.Format("{0}/{1}", ConfigContent.configURL.ResHost, cmi.name);
                // Action
                // Progress
                // TODO
            }
        }
        #endregion
    }
}