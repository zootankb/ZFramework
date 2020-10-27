using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZFramework.Res;

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
        /// 正在下载的ab包的进程，下载完后的ab包进程会移出去
        /// </summary>
        private static Dictionary<string, float> currAbProgress = new Dictionary<string, float>();

        /// <summary>
        /// 总共有多少个ab包
        /// </summary>
        private static int totalAbNum = 0;

        /// <summary>
        /// 当前下载ab包的数量
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
            totalAbNum = assetBundleInfos.Count;
            currDownloadNum = 0;
            // 下载Manifest文件
            Action<string, long, byte[], object[]> cb = (eurl, code, bytes, args) =>
            {
                currDownloadNum++;
                if (bytes != null)
                {
                    string content = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    string errMsg = string.Format("下载 {0} 时出现异常！", eurl);
                    errorCb?.Invoke(errMsg);
                    Log.LogOperator.AddNetErrorRecord(errMsg);
                }
                progressCb?.Invoke(currDownloadNum, totalAbNum);
            };
            // 单个ab包的进度
            Action<float> progress = (pro) =>
            {
                float p = (currAbProgress.Values.Sum() + currDownloadNum) / totalAbNum;
                childInfoCb?.Invoke(p);
            };

            // 一次性下载所有的资源
            foreach (var manifest in assetBundleInfos)
            {
                // TODO
            }
        }
        #endregion
    }
}