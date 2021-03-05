using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using ZFramework;
using ZFramework.HotFix;

public class TestHotFix : MonoBehaviour
{
    public Text txt = null;

    void Start()
    {
        txt.text = ConfigContent.configURL.ManifestHost;

        txt.text = Application.version;
        //HotFix.StartHotFix();        
        //StartCoroutine(IEumStart());
    }

    private void OnDestroy()
    {
        HotFix.StopHotFix();
    }

    private IEnumerator IEumStart()
    {
        while (!HotFix.isDone)
        {
            string content = string.Format("文件名字：{0}\t已经下载：{1}/{2}\t当前子进度：{3}\t第 {4} 个文件\r\n已经存储了：{5}/{6}\t已经下载了 {7} 个文件\t总共 {8} 个文件，总进度：{9}",
                HotFix.curDownloadAssetName, HotFix.curDownloadAssetSize, HotFix.curDownloadAssetTotalSize, HotFix.currDownloadProgress, HotFix.curDownloadAssetIndex,
                HotFix.downloadedSize, HotFix.needToDownloadTotalSize, HotFix.downloadedCount, HotFix.needToDownloadCount, HotFix.totalProgress);
            txt.text = content;
            yield return new WaitForEndOfFrame();
        }
    }
}