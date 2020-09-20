using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework.Net
{
    /// <summary>
    /// Post数据请求
    /// 确保WWWForm、Dictionary、string、byte类型中只有一个不为空
    /// 若4种提交数据部分不为空的话，就按上述数据类型顺序，选择第一个不为空的类型提交
    /// </summary>
    public sealed class NetPostRequest
    {
        #region Field
        /// <summary>
        /// url
        /// </summary>
        public string url = string.Empty;

        /// <summary>
        /// 数据表单
        /// </summary>
        public WWWForm postDataForm = null;
        /// <summary>
        /// 数据字典
        /// </summary>
        public Dictionary<string, string> postDataDic = null;
        /// <summary>
        /// 数据字符串
        /// </summary>
        public string postDataStr = null;
        /// <summary>
        /// 数据流
        /// </summary>
        public byte[] postDataByteArr = null;
        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> headers = null;

        /// <summary>
        /// 回掉，url， 请求状态码，传过来的参数
        /// </summary>
        public Action<string, long, object[]> callback = null;
        /// <summary>
        /// 回掉，url， 请求状态码，返回的数据流，传过来的参数
        /// </summary>
        public Action<string, long, byte[], object[]> callbackByteArr = null;
        /// <summary>
        /// 回掉，url， 请求状态码，返回的数据流，传过来的参数
        /// </summary>
        public Action<string, long, string, object[]> callbackStr = null;

        /// <summary>
        /// 进度
        /// </summary>
        public Action<float> progress = null;
        /// <summary>
        /// 调用者传过来的参数，最后还要返回给调用者
        /// </summary>
        private object[] args = null;
        #endregion

        #region Constructors
        /// <summary>
        /// 内部通用构造器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, WWWForm postDataForm, Dictionary<string, string> headers = null, Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataForm = postDataForm;
            this.headers = headers;
            this.callback = callback;
            this.progress = progress;
            this.args = args;
        }
        /// <summary>
        /// Form POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, WWWForm postDataForm, Dictionary<string, string> headers = null, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataForm = postDataForm;
            this.callbackByteArr = callbackByteArr;
            this.headers = headers;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// Form POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, WWWForm postDataForm, Dictionary<string, string> headers = null, Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataForm = postDataForm;
            this.callbackStr = callbackStr;
            this.headers = headers;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据字典 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null, Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataDic = postDataDic;
            this.callback = callback;
            this.headers = headers;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据字典 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataDic = postDataDic;
            this.callbackByteArr = callbackByteArr;
            this.headers = headers;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据字典 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null, Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataDic = postDataDic;
            this.callbackStr = callbackStr;
            this.headers = headers;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 字符串 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, string postDataStr, Dictionary<string, string> headers = null, Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataStr = postDataStr;
            this.headers = headers;
            this.callback = callback;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 字符串 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        private NetPostRequest(string url, string postDataStr, Dictionary<string, string> headers = null, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataStr = postDataStr;
            this.headers = headers;
            this.callbackByteArr = callbackByteArr;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 字符串 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, string postDataStr, Dictionary<string, string> headers = null, Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataStr = postDataStr;
            this.headers = headers;
            this.callbackStr = callbackStr;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据流 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        private NetPostRequest(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null, Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataByteArr = postDataByteArr;
            this.headers = headers;
            this.callback = callback;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据流 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        private NetPostRequest(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataByteArr = postDataByteArr;
            this.headers = headers;
            this.callbackByteArr = callbackByteArr;
            this.progress = progress;
            this.args = args;
        }

        /// <summary>
        /// 数据流 POST方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        private NetPostRequest(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null, Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            this.url = url;
            this.postDataByteArr = postDataByteArr;
            this.headers = headers;
            this.callbackStr = callbackStr;
            this.progress = progress;
            this.args = args;
        }
        #endregion

        #region POST 私有接口
        /// <summary>
        /// 上传WWWForm表单的POST，会有 urlencoded 加密支持
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumPostWWWForm()
        {
            UnityWebRequest request = UnityWebRequest.Post(url, postDataForm);
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, null, args);
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                    }
                    else
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
                        callbackStr?.Invoke(url, request.responseCode, request.downloadHandler.text, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// 上传数据字典的POST
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumPostDic()
        {
            UnityWebRequest request = UnityWebRequest.Post(url, postDataDic);
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
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, null, args);
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                    }
                    else
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
                        callbackStr?.Invoke(url, request.responseCode, request.downloadHandler.text, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// 上传字符串的POST
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumPostStr()
        {
            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(UTF8Encoding.UTF8.GetBytes(postDataStr));
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, null, args);
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                    }
                    else
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, request.downloadHandler.data, args);
                        callbackStr?.Invoke(url, request.responseCode, request.downloadHandler.text, args);
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            request.Dispose();
        }

        /// <summary>
        /// 上传数据流的POST
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumPostByteArr()
        {
            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.SetRequestHeader(kv.Key, kv.Value);
                }
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(postDataByteArr);
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                progress?.Invoke(ao.progress);
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        callback?.Invoke(url, request.responseCode, args);
                        callbackByteArr?.Invoke(url, request.responseCode, null, args);
                        callbackStr?.Invoke(url, request.responseCode, null, args);
                    }
                    else
                    {
                        callback?.Invoke(url, request.responseCode, args);
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
        /// 外部统一调用接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator IEnumStart()
        {
            if (postDataForm != null)
            {
                yield return IEnumPostWWWForm();
            }
            else if (postDataDic != null)
            {
                yield return IEnumPostDic();
            }
            else if (postDataStr != null)
            {
                yield return IEnumPostStr();
            }
            else if (postDataByteArr != null)
            {
                yield return IEnumPostByteArr();
            }
        }
        #endregion

        #region 静态调用
        /// <summary>
        /// WWWForm方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataForm, headers, callback, progress, args);
        }

        /// <summary>
        /// WWWForm方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataForm, headers, callback, progress, args);
        }

        /// <summary>
        /// WWWForm方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataForm, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据字典方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataDic, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据字典方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataDic, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据字典方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate( string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataDic, headers, callback, progress, args);
        }

        /// <summary>
        ///  数据字符串方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataStr, headers, callback, progress, args);
        }

        /// <summary>
        ///  数据字符串方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataStr, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据字符串方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataStr, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据流方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataByteArr, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据流方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataByteArr, headers, callback, progress, args);
        }

        /// <summary>
        /// 数据流方式 分配一个内存空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static NetPostRequest Allocate(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            return new NetPostRequest(url, postDataByteArr, headers, callback, progress, args);
        }
        #endregion
    }
}