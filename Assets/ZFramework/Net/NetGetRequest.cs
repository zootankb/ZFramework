using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework.Net
{
    /// <summary>
    /// Get请求数据
    /// </summary>
    public sealed class NetGetRequest
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public const int TIME_OUT = 5;

        #region Field
        /// <summary>
        /// 消息id
        /// </summary>
        public int id;
        /// <summary>
        /// url
        /// </summary>
        public string url = string.Empty;
        /// <summary>
        /// 可选参
        /// </summary>
        public Dictionary<string, string> filter = null;
        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> headers = null;
        /// <summary>
        /// 默认的超时时间
        /// </summary>
        public int timeout = 10;

        /// <summary>
        /// 字符串回掉事件：事件ID，请求状态码，返回的字符串，传过来的参数
        /// </summary>
        public Action<int, long, string, object[]> callbackStr = null;
        /// <summary>
        /// 数据流回掉事件：事件ID，请求状态码，返回的数据流，传过来的参数
        /// </summary>
        public Action<int, long, byte[], object[]> callbackByteArr = null;
        /// <summary>
        /// Texture2D回掉事件：事件ID，请求状态码，返回的Texture2D，传过来的参数
        /// </summary>
        public Action<int, long, Texture2D, object[]> callbackTexture2D = null;
        /// <summary>
        /// Sprite回掉事件：事件ID，请求状态码，返回的Sprite，传过来的参数
        /// </summary>
        public Action<int, long, Sprite, object[]> callbackSprite = null;

        /// <summary>
        /// 进度
        /// </summary>
        public Action<float> progress = null;
        /// <summary>
        /// 调用者传过来的参数，还要返回给调用者
        /// </summary>
        public object[] args = null;
        #endregion

        #region Constructors
        /// <summary>
        /// 字符串 GET请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        private NetGetRequest(int id, string url ,  Dictionary<string, string> filter = null,  Dictionary<string, string> headers = null, 
            Action<int, long, string, object[]> callbackStr = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.filter = filter;
            this.headers = headers;
            this.callbackStr = callbackStr;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// byte[] GET请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        private NetGetRequest(int id, string url, Dictionary<string, string> filter = null, Dictionary<string, string> headers = null,
            Action<int, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.filter = filter;
            this.headers = headers;
            this.callbackByteArr = callbackByteArr;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// Texture2D GET请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        private NetGetRequest(int id, string url, Dictionary<string, string> filter = null, Dictionary<string, string> headers = null, 
            Action<int, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.filter = filter;
            this.headers = headers;
            this.callbackTexture2D = callbackTexture2D;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }

        /// <summary>
        /// Sprite GET请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        private NetGetRequest(int id, string url, Dictionary<string, string> filter = null,Dictionary<string, string> headers = null, 
            Action<int, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.filter = filter;
            this.headers = headers;
            this.callbackSprite = callbackSprite;
            this.progress = progress;
            this.timeout = timeout;
            this.args = args;
        }
        #endregion

        #region GET 私有接口
        /// <summary>
        /// GET接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callback">Action<EventID, long, byte[]>:事件id， 请求状态码，返回的字符串</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator IEnumGetStr()
        {
            if (filter != null)
            {
                url += "?";
                bool first = false;
                foreach (var kv in filter)
                {
                    url += string.Format("{0}{1}={2}", first ? string.Empty : "&", kv.Key, kv.Value);
                    first = true;
                }
            }
            UnityWebRequest request = new UnityWebRequest(url);
            request.timeout = timeout;
            request.downloadHandler = new DownloadHandlerBuffer();
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callbackStr?.Invoke(id, request.responseCode, null, args);
                    }
                    else
                    {
                        callbackStr?.Invoke(id, request.responseCode, request.downloadHandler.text, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// GET接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callback">Action<EventID, long, byte[]>:事件id， 请求状态码，返回的数据流</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator IEnumGetByteArr()
        {
            if (filter != null)
            {
                url += "?";
                bool first = false;
                foreach (var kv in filter)
                {
                    url += string.Format("{0}{1}={2}", first ? string.Empty : "&", kv.Key, kv.Value);
                    first = true;
                }
            }
            UnityWebRequest request = new UnityWebRequest(url);
            request.timeout = timeout;
            request.downloadHandler = new DownloadHandlerBuffer();
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callbackByteArr?.Invoke(id, request.responseCode, null, args);
                    }
                    else
                    {
                        callbackByteArr?.Invoke(id, request.responseCode, request.downloadHandler.data, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// 下载Texture
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callback">Action<EventID, long, byte[]>:事件id， 请求状态码，返回的Texture2D</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator IEnumGetTexture2D()
        {
            if (filter != null)
            {
                url += "?";
                bool first = false;
                foreach (var kv in filter)
                {
                    url += string.Format("{0}{1}={2}", first ? string.Empty : "&", kv.Key, kv.Value);
                    first = true;
                }
            }
            UnityWebRequest request = new UnityWebRequest(url);
            DownloadHandlerTexture dht = new DownloadHandlerTexture();
            request.timeout = timeout;
            request.downloadHandler = dht;
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callbackTexture2D?.Invoke(id, request.responseCode, null, args);
                    }
                    else
                    {
                        callbackTexture2D?.Invoke(id, request.responseCode, dht.texture, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// 下载Sprite
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callback">Action<EventID, long, byte[]>:事件id， 请求状态码，返回的Sprite</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator IEnumGetSprite()
        {
            if (filter != null)
            {
                url += "?";
                bool first = false;
                foreach (var kv in filter)
                {
                    url += string.Format("{0}{1}={2}", first ? string.Empty : "&", kv.Key, kv.Value);
                    first = true;
                }
            }
            UnityWebRequest request = new UnityWebRequest(url);
            DownloadHandlerTexture dht = new DownloadHandlerTexture();
            request.timeout = timeout;
            request.downloadHandler = dht;
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callbackSprite?.Invoke(id, request.responseCode, null, args);
                    }
                    else
                    {
                        Sprite sprite = Sprite.Create(dht.texture, new Rect(0, 0, dht.texture.width, dht.texture.height), Vector2.one / 2);
                        callbackSprite?.Invoke(id, request.responseCode, sprite, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }
        #endregion

        #region 外部调用
        /// <summary>
        /// 外部调用
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumStart()
        {
            if (callbackStr != null)
            {
                yield return IEnumGetStr();
            }
            else if(callbackByteArr != null)
            {
                yield return IEnumGetByteArr();
            }
            else if (callbackTexture2D != null)
            {
                yield return IEnumGetTexture2D();
            }
            else if (callbackSprite != null)
            {
                yield return IEnumGetSprite();
            }
        }
        #endregion

        #region 静态调用
        /// <summary>
        /// 字符串方式 分配一个空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetGetRequest Allocate(int id, string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<int, long, string, object[]> callbackStr = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(id, url, filter, headers, callbackStr, progress, TIME_OUT, args);
        }

        /// <summary>
        /// 数据流方式 分配一个空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetGetRequest Allocate(int id, string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<int, long, byte[], object[]> callbackByteArr = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(id, url, filter, headers, callbackByteArr, progress, TIME_OUT, args);
        }

        /// <summary>
        /// Texture2D方式 分配一个空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetGetRequest Allocate(int id, string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<int, long, Texture2D, object[]> callbackTexture2D = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(id, url, filter, headers, callbackTexture2D, progress, TIME_OUT, args);
        }

        /// <summary>
        /// Sprite方式 分配一个空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetGetRequest Allocate(int id, string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<int, long, Sprite, object[]> callbackSprite = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(id, url, filter, headers, callbackSprite, progress, TIME_OUT, args);
        }
        #endregion
    }
}