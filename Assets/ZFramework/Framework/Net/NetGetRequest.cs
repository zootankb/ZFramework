using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ZFramework.Log;

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
        /// 字符串回掉事件：url，请求状态码，返回的字符串，传过来的参数
        /// </summary>
        public Action<string, long, string, object[]> callbackStr = null;
        /// <summary>
        /// 数据流回掉事件：url，请求状态码，返回的数据流，传过来的参数
        /// </summary>
        public Action<string, long, byte[], object[]> callbackByteArr = null;

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
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        private NetGetRequest( string url ,  Dictionary<string, string> filter = null,  Dictionary<string, string> headers = null, 
            Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
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
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        private NetGetRequest( string url, Dictionary<string, string> filter = null, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, int timeout = TIME_OUT, params object[] args)
        {
            this.url = url;
            this.filter = filter;
            this.headers = headers;
            this.callbackByteArr = callbackByteArr;
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
        /// <param name="callback">Action<EventID, long, byte[]>:url， 请求状态码，返回的字符串</param>
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
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                        LogOperator.AddNetErrorRecord("GET请求失败", request.responseCode, request.error, url, request.isHttpError.ToString(), request.isNetworkError.ToString());
                    }
                    else
                    {
                        callbackStr?.Invoke(url, request.responseCode, request.downloadHandler.text, args);
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
        /// <param name="callback">Action<EventID, long, byte[]>:url， 请求状态码，返回的数据流</param>
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
                        callbackByteArr?.Invoke(url, request.responseCode, null, args);
                        LogOperator.AddNetErrorRecord("GET请求失败", request.responseCode, request.error, url, request.isHttpError.ToString(), request.isNetworkError.ToString());
                    }
                    else
                    {
                        callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
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
        public static NetGetRequest Allocate( string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callbackStr = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(url, filter, headers, callbackStr, progress, TIME_OUT, args);
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
        public static NetGetRequest Allocate( string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callbackByteArr = null,
            Action<float> progress = null, params object[] args)
        {
            return new NetGetRequest(url, filter, headers, callbackByteArr, progress, TIME_OUT, args);
        }
        #endregion
    }
}