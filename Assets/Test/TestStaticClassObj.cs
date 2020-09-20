using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

public static class TestStaticClassObj 
{
    public static void Test()
    {
        //Action< url, Action<string, long, byte[], object[]> callbackByteArr = null, Action< float > progress = null, params object[] args>

        Action<string, long, byte[], object[]> callback = (url, code, bsarr, objs) => {
            Debug.Log("返回参数长度：" + objs.Length);
            Debug.Log("返回数据长度："+ bsarr.Length);
        };
        //Res.NetResGetRequest nrg = Res.NetResGetRequest.Allocate("http://down.sandai.net/thunderx/XunLeiWebSetup10.1.38.884dl.exe", callback, (progress) =>
        //{
        //    Debug.Log("下载进度：" + progress);
        //}, "测试链接");

    }
}
