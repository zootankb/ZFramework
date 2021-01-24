using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZFramework.Singleton;

namespace ZFramework.Res
{
    /// <summary>
    /// 网络资源的下载，只提供下载，防止资源多次重复下载，没有资源暂时存储功能
    /// </summary>
    public class NetResDownload : SingletonMonoDontDes<NetResDownload>
    {
        #region Data
        /// <summary>
        /// 正常成功的下载返回值
        /// </summary>
        private const long REPONE_SUCC_CODE = 200;

        /// <summary>
        /// 存储字节流回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, byte[], object[]>> callbackByteArrDic = new Dictionary<string, Action<string, long, byte[], object[]>>();
        /// <summary>
        /// 存储ab包回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, AssetBundle, object[]>> callbackAbDic = new Dictionary<string, Action<string, long, AssetBundle, object[]>>();
        /// <summary>
        /// 存储t2d回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, Texture2D, object[]>> callbackT2dDic = new Dictionary<string, Action<string, long, Texture2D, object[]>>();
        /// <summary>
        /// 存储sprite回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, Sprite, object[]>> callbackSpriteDic = new Dictionary<string, Action<string, long, Sprite, object[]>>();
        /// <summary>
        /// 存储TextAsset回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, TextAsset, object[]>> callbackTextAssetDic = new Dictionary<string, Action<string, long, TextAsset, object[]>>();
        /// <summary>
        /// 存储audioclip回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, AudioClip, object[]>> callbackAudioclipDic = new Dictionary<string, Action<string, long, AudioClip, object[]>>();
        #endregion

        #region Pri Class
        /// <summary>
        /// 分配空间下载二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateByteArr(string url, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, byte[], object[]> callback = (uurl, code, byteArr, cargs) =>
            {
                callbackByteArrDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE ? byteArr : null, cargs);
                callbackByteArrDic.Remove(url);
            };
            // 不管下没下载都执行一次下载，在会调用处理下载结果
            if (callbackByteArrDic.ContainsKey(url))
            {
                callbackByteArrDic[url] += callbackByteArr;
            }
            else
            {
                callbackByteArrDic.Add(url, callbackByteArr);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        private void AllocateByteArr(string url, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, AssetBundle, object[]> callback = (uurl, code, ab, cargs) =>
            {
                callbackAbDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE ? ab : null, cargs);
                callbackAbDic.Remove(url);
            };
            // 不管下没下载都执行一次下载，在会调用处理下载结果
            if (callbackByteArrDic.ContainsKey(url))
            {
                callbackAbDic[url] += callbackAb;
            }
            else
            {
                callbackAbDic.Add(url, callbackAb);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载Texture2D
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTexture2d"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateTexture2d(string url, Action<string, long, Texture2D, object[]> callbackTexture2d = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, Texture2D, object[]> callback = (uurl, code, t2d, cargs) => {
                callbackT2dDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE? t2d : null, cargs);
                callbackT2dDic.Remove(url);
            };
            if (callbackT2dDic.ContainsKey(url))
            {
                callbackT2dDic[url] += callbackTexture2d;
            }
            else
            {
                callbackT2dDic.Add(url, callbackTexture2d);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载Sprite
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateSprite(string url, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, Sprite, object[]> callback = (uurl, code, sprite, cargs) => {
                callbackSpriteDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE? sprite : null , cargs);
                callbackSpriteDic.Remove(url);
            };
            if (callbackSpriteDic.ContainsKey(url))
            {
                callbackSpriteDic[url] += callbackSprite;
            }
            else
            {
                callbackSpriteDic.Add(url, callbackSprite);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载TextAsset
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTextasset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateTextAsset(string url, Action<string, long, TextAsset, object[]> callbackTextasset = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, TextAsset, object[]> callback = (uurl, code, txtast, cargs) => {
                callbackTextAssetDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE ? txtast : null, cargs);
                callbackTextAssetDic.Remove(url);
            };
            if (callbackTextAssetDic.ContainsKey(url))
            {
                callbackTextAssetDic[url] += callbackTextasset;
            }
            else
            {
                callbackTextAssetDic.Add(url, callbackTextasset);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载AudioClip
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAudioclip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateAudioClip(string url, Action<string, long, AudioClip, object[]> callbackAudioclip = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, AudioClip, object[]> callback = (uurl, code, audioclip, cargs) => {
                callbackAudioclipDic[url]?.Invoke(uurl, code, code == REPONE_SUCC_CODE ? audioclip : null, cargs);
                callbackAudioclipDic.Remove(url);
            };
            if (callbackAudioclipDic.ContainsKey(url))
            {
                callbackAudioclipDic[url] += callbackAudioclip;
            }
            else
            {
                callbackAudioclipDic.Add(url, callbackAudioclip);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(url, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        #endregion

        #region Static
        /// <summary>
        /// 开始下载二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateByteArr(url, callbackByteArr, progress, args);
        }

        /// <summary>
        /// 开始下载ab包
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateByteArr(url, callbackAb, progress, args);
        }

        /// <summary>
        /// 开始下载Texture2D图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateTexture2d(url, callbackTexture2D, progress, args);
        }

        /// <summary>
        /// 开始下载Sprite图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateSprite(url, callbackSprite, progress, args);
        }

        /// <summary>
        /// 开始下载TextAsset
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTextAsset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, TextAsset, object[]> callbackTextAsset = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateTextAsset(url, callbackTextAsset, progress, args);
        }

        /// <summary>
        /// 开始下载AudioClip
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAudioclip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string url, Action<string, long, AudioClip, object[]> callbackAudioclip = null, Action<float> progress = null, params object[] args)
        {
            NetResDownload.Instance.AllocateAudioClip(url, callbackAudioclip, progress, args);
        }
        #endregion
    }
}