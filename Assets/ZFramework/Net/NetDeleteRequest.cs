using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework.Net
{
    /// <summary>
    /// DELETE 数据请求
    /// </summary>
    public sealed class NetDeleteRequest
    {
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
        /// 请求头
        /// </summary>
        public Dictionary<string, string> headers = null;

        /// <summary>
        /// 回掉，url， 请求状态码，用户传过来的数据
        /// </summary>
        public Action<string, long, object[]> callback = null;
        /// <summary>
        /// 回掉，url， 请求状态码，返回的数据流，用户传过来的数据
        /// </summary>
        public Action<string, long, byte[], object[]> callbackByteArr = null;
        /// <summary>
        /// 回掉，url， 请求状态码，返回的数据流，用户传过来的数据
        /// </summary>
        public Action<string, long, string, object[]> callbackStr = null;

        /// <summary>
        /// 进度
        /// </summary>
        public Action<float> progress = null;
        /// <summary>
        /// 调用者传过来的数据，完成后还原样返回给调用者
        /// </summary>
        public object[] args = null;

        #endregion

        #region Constructors
        /// <summary>
        /// 私有基础构造器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetDeleteRequest(int id, string url, Dictionary<string, string> headers = null, Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.headers = headers;
            this.callback = callback;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 返回数据流 构造器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        private NetDeleteRequest(int id, string url, Dictionary<string, string> headers = null, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.headers = headers;
            this.callbackByteArr = callbackByteArr;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 返回字符串 构造器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        private NetDeleteRequest(int id, string url, Dictionary<string, string> headers = null, Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            this.id = id;
            this.url = url;
            this.headers = headers;
            this.callbackStr = callbackStr;
            this.progress = progress;
            this.args = args;
        }

        #endregion

        #region DELETE 私有接口
        /// <summary>
        /// 私有delete协程，根据初始化内容，回掉时返回对应格式的数据
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEnumDelete()
        {
            UnityWebRequest request = UnityWebRequest.Delete(url);
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
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
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                    }
                    else
                    {
                        callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
                        callbackStr?.Invoke(url, request.responseCode, request.downloadHandler.text, args);
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
        /// 开启协程
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumStart()
        {
            yield return IEnumDelete();
        }
        #endregion

        #region 静态调用
        /// <summary>
        /// 返回状态码，分配内存空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetDeleteRequest Allocate(int id, string url, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null)
        {
            return new NetDeleteRequest(id, url, headers, callback, progress);
        }

        /// <summary>
        /// 返回数据流 分配内存空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetDeleteRequest Allocate(int id, string url, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null)
        {
            return new NetDeleteRequest(id, url, headers, callbackByteArr, progress);
        }

        /// <summary>
        /// 返回字符串 分配内存空间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static NetDeleteRequest Allocate(int id, string url, Dictionary<string, string> headers = null,
           Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null)
        {
            return new NetDeleteRequest(id, url, headers, callbackStr, progress);
        }

        #endregion

    }
}