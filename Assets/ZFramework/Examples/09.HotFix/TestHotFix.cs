using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using ZFramework;

public class TestHotFix : MonoBehaviour
{
    public string url = null;

    public string path = null;

    // Start is called before the first frame update
    void Start()
    {
        long totalCount = 0;
        // print(GetNetFileLength(url));
        // HttpDownloadFile(url, path);
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

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string HttpDownloadFile(string url, string path)
    {
        // 设置参数
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        //发送请求并获取相应回应数据
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        //直到request.GetResponse()程序才开始向目标网页发送Post请求
        using (Stream responseStream = response.GetResponseStream())
        {
            print("文件长度：" + responseStream.Length);
            //创建本地文件写入流
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
            }
        }
        return path;
    }
}