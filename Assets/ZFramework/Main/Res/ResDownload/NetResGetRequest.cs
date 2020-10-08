using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using ZFramework.Log;

namespace ZFramework.Res
{
    /// <summary>
    /// 网络资源下载专用
    /// </summary>
    public class NetResGetRequest
    {
        /// <summary>
        /// 超时时间，防止超大文件
        /// </summary>
        public const int TIME_OUT = 60;

        #region Field
        /// <summary>
        /// url
        /// </summary>
        public string url = string.Empty;
        /// <summary>
        /// 传进来的数据,同时要返回给调用者
        /// </summary>
        public object[] args = null;
        /// <summary>
        /// 默认的超时时间
        /// </summary>
        public int timeout = 10;

        /// <summary>
        /// 数据流回掉事件：url，请求状态码，返回的数据流，传进来的参数
        /// </summary>
        public Action<string, long, byte[], object[]> callbackByteArr = null;
        /// <summary>
        /// Texture2D回掉事件：url，请求状态码，返回的Texture2D，传进来的参数
        /// </summary>
        public Action<string, long, Texture2D, object[]> callbackTexture2D = null;
        /// <summary>
        /// AssetBundle回掉事件：url，请求状态码，返回的AssetBundle，传进来的参数
        /// </summary>
        public Action<string, long, AssetBundle, object[]> callbackAb = null;
        /// <summary>
        /// Sprite回掉事件：url，请求状态码，返回的Sprite，传进来的参数
        /// </summary>
        public Action<string, long, Sprite, object[]> callbackSprite = null;
        /// <summary>
        /// TextAsset回掉事件：url，请求状态码，返回的TextAsset，传进来的参数
        /// </summary>
        public Action<string, long, TextAsset, object[]> callbackTextasset = null;
        /// <summary>
        /// AudioClip回掉事件：url，请求状态码，返回的AudioClip，传进来的参数
        /// </summary>
        public Action<string, long, AudioClip, object[]> callbackAudio = null;

        /// <summary>
        /// 进度
        /// </summary>
        public Action<float> progress = null;
        #endregion

        #region Constructors
        /// <summary>
        /// 返回数据流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackByteArr = callbackByteArr;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// 返回ab包
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackAb = callbackAb;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// 返回texture2d
        /// </summary>
        /// <param name="resName">资源的名字，带后缀</param>
        /// <param name="url"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackTexture2D = callbackTexture2D;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// 返回sprite
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackSprite = callbackSprite;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// 返回TextAsset
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTextasset"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, TextAsset, object[]> callbackTextasset = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackTextasset = callbackTextasset;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// 返回audioclip
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAudio"></param>
        /// <param name="progress"></param>
        /// <param name="timeout"></param>
        /// <param name="args"></param>
        private NetResGetRequest(string url, Action<string, long, AudioClip, object[]> callbackAudio = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.callbackAudio = callbackAudio;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }


        #endregion

        #region IEnum
        /// <summary>
        /// 下载字节流
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetByteArr()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                request.timeout = timeout;
                request.downloadHandler = new DownloadHandlerBuffer();
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                        if (request.isHttpError || request.isNetworkError)
                        {
                            callbackByteArr?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord("获取数据流时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 下载ab包
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetAB()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = timeout;
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                        if (request.isHttpError || request.isNetworkError)
                        {
                            callbackAb?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord("获取数据流时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            AssetBundle ab = AssetBundle.LoadFromMemory(request.downloadHandler.data);
                            // 把ab包的二进制数据放进去
                            List<object> argsList = new List<object>();
                            if (args != null && args.Length > 0) { argsList.Add(argsList); }
                            argsList.Add(request.downloadHandler.data);
                            callbackAb?.Invoke(url, request.responseCode, ab, args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 下载Texture2D
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetTexture2D()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, false))
            {
                request.timeout = timeout;
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                        if (request.isHttpError || request.isNetworkError)
                        {
                            callbackTexture2D?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord("获取Texture2D时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            Texture2D t2d = DownloadHandlerTexture.GetContent(request);
                            t2d.name = Path.GetFileName(url);
                            callbackTexture2D?.Invoke(url, request.responseCode, t2d, args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 下载精灵图
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetSprite()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                DownloadHandlerTexture dht = new DownloadHandlerTexture();
                request.timeout = timeout;
                request.downloadHandler = dht;
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                        if (request.isHttpError || request.isNetworkError)
                        {
                            callbackSprite?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord("获取Sprite时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            Sprite sprite = Sprite.Create(dht.texture, new Rect(0, 0, dht.texture.width, dht.texture.height), Vector2.one / 2);
                            sprite.name = Path.GetFileName(url);
                            callbackSprite?.Invoke(url, request.responseCode, sprite, args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 下载Texttasset
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetTextasset()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                DownloadHandlerBuffer dht = new DownloadHandlerBuffer();
                request.timeout = timeout;
                request.downloadHandler = dht;
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                        if (request.isHttpError || request.isNetworkError)
                        {
                            callbackTextasset?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord("获取Textasset时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            callbackTextasset?.Invoke(url, request.responseCode, new TextAsset(dht.text), args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        /// <summary>
        /// 下载testasset
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumGetAudioClip()
        {
            if (string.IsNullOrEmpty(this.url))
            {
                yield break;
            }
            AudioType at = AudioType.MPEG;
            string ext = Path.GetExtension(url).ToLower();
            // unity仅支持这4种音频格式
            switch (ext)
            {
                case ".mp3":
                    at = AudioType.MPEG;
                    break;
                case ".wav":
                    at = AudioType.WAV;
                    break;
                case ".aiff":
                    at = AudioType.AIFF;
                    break;
                case ".ogg":
                    at = AudioType.OGGVORBIS;
                    break;
            }
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, at))
            {
                request.timeout = timeout;
                UnityWebRequestAsyncOperation ao = request.SendWebRequest();
                while (true)
                {
                    progress?.Invoke(ao.progress);
                    if (request.isDone)
                    {
                       if (request.isHttpError || request.isNetworkError)
                        {
                            callbackAudio?.Invoke(url, request.responseCode, null, args);
                            LogOperator.AddNetErrorRecord( "获取AudioClip时网络错误", url, request.responseCode);
                        }
                        else
                        {
                            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                            // 把语音的二进制数据放进去
                            List<object> argsList = new List<object>();
                            if(args!=null && args.Length > 0) {argsList.AddRange(args); }
                            argsList.Add(request.downloadHandler.data);
                            args = argsList.ToArray();
                            callbackAudio?.Invoke(url, request.responseCode, clip, args);
                        }
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                request.Dispose();
            }
        }
        #endregion

        #region 外部调用
        /// <summary>
        /// 外部调用
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumStart()
        {
            if (callbackByteArr != null)
            {
                yield return IEnumGetByteArr();
            }
            else if (callbackAb != null)
            {
                yield return IEnumGetAB();
            }
            else if (callbackTexture2D != null)
            {
                yield return IEnumGetTexture2D();
            }
            else if (callbackSprite != null)
            {
                yield return IEnumGetSprite();
            }
            else if (callbackTextasset != null)
            {
                yield return IEnumGetTextasset();
            }
            else if (callbackAudio != null)
            {
                yield return IEnumGetAudioClip();
            }
        }
        #endregion

        #region 静态调用
        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url, Action<string, long, byte[], object[]> callbackByteArr = null,  Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackByteArr, progress, NetResGetRequest.TIME_OUT, args);
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackAb, progress, NetResGetRequest.TIME_OUT, args);
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url, Action<string, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackTexture2D, progress, NetResGetRequest.TIME_OUT, args);
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url,  Action<string, long, Sprite, object[]> callbackSprite = null,Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackSprite, progress, NetResGetRequest.TIME_OUT, args);
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTextasset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url, Action<string, long, TextAsset, object[]> callbackTextasset = null, Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackTextasset, progress, NetResGetRequest.TIME_OUT, args);
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAudioclip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetResGetRequest Allocate(string url, Action<string, long, AudioClip, object[]> callbackAudioclip = null, Action<float> progress = null, params object[] args)
        {
            return new NetResGetRequest(url, callbackAudioclip, progress, NetResGetRequest.TIME_OUT, args);
        }
        #endregion
    }
}