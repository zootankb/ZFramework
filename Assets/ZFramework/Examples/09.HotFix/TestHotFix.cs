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

    public string url = null;

    public string path = null;

    // Start is called before the first frame update
    void Start()
    {
        long totalCount = 0;
        // print(GetNetFileLength(url));
        // HttpDownloadFile(url, path);
        /*
        Loom.RunAsync(() =>
        {
            for (long i = 0; i < 10000000000; i++)
            {
                totalCount += i;
            }
            Loom.QueueOnMainThread(() =>
            {
                print("完成度：" + totalCount);
            });
        });
        */
        
        HotFix.StartHotFix();
        txt.text = ConfigContent.configURL.ManifestHost;
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

            request.MaximumAutomaticRedirections = 4;

            request.MaximumResponseHeadersLength = 4;

            request.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            length = response.ContentLength;
            
            response.Close();
        }
        catch(Exception e)
        {
            ZFramework.Log.LogOperator.AddResErrorRecord("下载资源时出错", e.Message);
        }
        return length;
    }
}