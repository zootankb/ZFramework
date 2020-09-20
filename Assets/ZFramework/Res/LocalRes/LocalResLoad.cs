using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZFramework.Singleton;

namespace ZFramework.Res
{
    /// <summary>
    /// 利用NetResDownload进行本地资源的加载，不作为资源的内存存储
    /// Assetbundle、texture2d、sprite、textasset、audioclip、videoclip
    /// </summary>
    public class LocalResLoad : SingletonMonoDontDes<LocalResLoad>
    {
        #region Data
        /// <summary>
        /// 存储字节流回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
        /// </summary>
        private Dictionary<string, Action<string, long, byte[], object[]>> callbackByteArrDic = new Dictionary<string, Action<string, long, byte[], object[]>>();
        /// <summary>
        /// 存储assetbundle回掉集合，用完即废，随用随加，防止同一个资源同时多个请求下载
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
        /// <param name="path"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateByteArr(string path, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, byte[], object[]> callback = (ppath, code, byteArr, cargs) =>
            {
                callbackByteArrDic[ppath]?.Invoke(ppath, code, byteArr, args);
                callbackByteArrDic.Remove(ppath);
            };
            // 不管下没下载都执行一次下载，在会调用处理下载结果
            if (callbackByteArrDic.ContainsKey(path))
            {
                callbackByteArrDic[path] += callbackByteArr;
            }
            else
            {
                callbackByteArrDic.Add(path, callbackByteArr);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间加载ab包数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateAssetbundle(string path, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, AssetBundle, object[]> callback = (ppath, code, ab, cargs) =>
            {
                callbackAbDic[ppath]?.Invoke(ppath, code, ab, args);
                callbackAbDic.Remove(ppath);
            };
            // 不管下没下载都执行一次下载，在会调用处理下载结果
            if (callbackByteArrDic.ContainsKey(path))
            {
                callbackAbDic[path] += callbackAb;
            }
            else
            {
                callbackAbDic.Add(path, callbackAb);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载Texture2D
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callbackTexture2d"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateTexture2d(string path, Action<string, long, Texture2D, object[]> callbackTexture2d = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, Texture2D, object[]> callback = (ppath, code, t2d, cargs) => {
                callbackT2dDic[ppath]?.Invoke(ppath, code, t2d, args);
                callbackT2dDic.Remove(ppath);
            };
            if (callbackT2dDic.ContainsKey(path))
            {
                callbackT2dDic[path] += callbackTexture2d;
            }
            else
            {
                callbackT2dDic.Add(path, callbackTexture2d);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载Sprite
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateSprite(string path, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, Sprite, object[]> callback = (ppath, code, sprite, cargs) => {
                callbackSpriteDic[ppath]?.Invoke(ppath, code, sprite, args);
                callbackSpriteDic.Remove(ppath);
            };
            if (callbackSpriteDic.ContainsKey(path))
            {
                callbackSpriteDic[path] += callbackSprite;
            }
            else
            {
                callbackSpriteDic.Add(path, callbackSprite);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载TextAsset
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callbackTextasset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateTextAsset(string path, Action<string, long, TextAsset, object[]> callbackTextasset = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, TextAsset, object[]> callback = (ppath, code, audioclip, cargs) => {
                callbackTextAssetDic[ppath]?.Invoke(ppath, code, audioclip, args);
                callbackTextAssetDic.Remove(ppath);
            };
            if (callbackTextAssetDic.ContainsKey(path))
            {
                callbackTextAssetDic[path] += callbackTextasset;
            }
            else
            {
                callbackTextAssetDic.Add(path, callbackTextasset);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        /// <summary>
        /// 分配空间下载AudioClip
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callbackAudioclip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private void AllocateAudioClip(string path, Action<string, long, AudioClip, object[]> callbackAudioclip = null, Action<float> progress = null, params object[] args)
        {
            Action<string, long, AudioClip, object[]> callback = (ppath, code, audioclip, cargs) => {
                callbackAudioclipDic[ppath]?.Invoke(ppath, code, audioclip, args);
                callbackAudioclipDic.Remove(ppath);
            };
            if (callbackAudioclipDic.ContainsKey(path))
            {
                callbackAudioclipDic[path] += callbackAudioclip;
            }
            else
            {
                callbackAudioclipDic.Add(path, callbackAudioclip);
            }
            NetResGetRequest nrgr = NetResGetRequest.Allocate(path, callback, progress, args);
            StartCoroutine(nrgr.IEnumStart());
        }

        #endregion

        #region Static
        /// <summary>
        /// 开始下载二进制数据
        /// </summary>
        /// <param name="filepath">因为是二进制数据，不知道是那种类型的文件，所以必须是文件的本地绝对路径</param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateByteArr(filepath, callbackByteArr, progress, args);
        }

        /// <summary>
        /// 加载二进制数据
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateAssetbundle(filepath, callbackAb, progress, args);
        }

        /// <summary>
        /// 开始下载Texture2D图片
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateTexture2d(filepath, callbackTexture2D, progress, args);
        }

        /// <summary>
        /// 开始下载Sprite图片
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateSprite(filepath, callbackSprite, progress, args);
        }

        /// <summary>
        /// 开始下载TextAsset
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="callbackTextAsset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, TextAsset, object[]> callbackTextAsset = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateTextAsset(filepath, callbackTextAsset, progress, args);
        }

        /// <summary>
        /// 开始下载AudioClip
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="callbackAudioclip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Allocate(string filepath, Action<string, long, AudioClip, object[]> callbackAudioclip = null, Action<float> progress = null, params object[] args)
        {
            Instance.AllocateAudioClip(filepath, callbackAudioclip, progress, args);
        }
        #endregion
    }
}