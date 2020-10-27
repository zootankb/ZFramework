using System;
using System.Collections;
using System.Collections.Generic;
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
        /// 更新ba包时候的返回信息事件
        /// </summary>
        private static Action<string, float> childInfoCb = null;


        /// <summary>
        ///本地主manifest文件
        /// </summary>
        private static CurrPlatformManifestInfo localManifestInfo = null;

        /// <summary>
        /// 本地manifestInfo文件里存储的子manifest文件信息
        /// </summary>
        private static Dictionary<string, ChildDetailManifestInfo> localDetailInfos = null;

        /// <summary>
        ///网络主manifest文件
        /// </summary>
        private static CurrPlatformManifestInfo netManifestInfo = null;

        /// <summary>
        /// 网络manifestInfo文件里存储的子manifest文件信息
        /// </summary>
        private static Dictionary<string, ChildDetailManifestInfo> netDetailInfos = null;

        #endregion 


        #region API

        /// <summary>
        /// 开启更新
        /// </summary>
        /// <param name="fineshed">更新完成后的返回事件</param>
        /// <param name="maniProgress">主文件的进度</param>
        /// <param name="progress">更新进度</param>
        /// <param name="error">更新失败后的返回事件</param>
        /// <param name="childInfo">更新ba包时候的返回信息事件</param>
        public static void StartUpdate(Action<bool> fineshed = null, Action<float> maniProgress = null, Action<int,int> progress = null,
            Action<string> error = null, Action<string, float> childInfo = null)
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

        }
        #endregion
    }
}