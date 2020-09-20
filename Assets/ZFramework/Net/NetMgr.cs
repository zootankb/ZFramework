using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Net
{
    /// <summary>
    /// 网络管理器
    /// </summary>
    public class NetMgr : ZFramework.Singleton.SingletonMonoDontDes<NetMgr>
    {
        #region GET
        /// <summary>
        /// GET string
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void GetString(string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callbackStr = null,
            Action<float> progress = null, params object[] args)
        {
            if(NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.GET, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.GET, url, callbackStr);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.GET, url, callbackStr);
                Action<string, long, string, object[]> callback = (uurl, code, content, aargs) =>
                  {
                      NetEventTmpSave.Invoke(NetEventTmpSave.EventType.GET, uurl, code, content, aargs);
                  };
                NetGetRequest request = NetGetRequest.Allocate(url, filter, headers, callback, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
        }

        /// <summary>
        /// GET Bytearr
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <param name="headers"></param>
        /// <param name="callbackBs"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void GetByteArr(string url,
            Dictionary<string, string> filter = null,
            Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callbackBs = null,
            Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.GET, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.GET, url, callbackBs);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.GET, url, callbackBs);
                Action<string, long, byte[], object[]> callback = (uurl, code, bs, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.GET, uurl, code, bs, aargs);
                };
                NetGetRequest request = NetGetRequest.Allocate(url, filter, headers, callback, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// DELETE 返回空数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        public static void DeleteNone(string url, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callbacknone = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventNone(NetEventTmpSave.EventType.DELETE, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbacknone);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbacknone);
                Action<string, long, object[]> callback = (uurl, code, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.DELETE, uurl, code, aargs);
                };
                NetDeleteRequest request = NetDeleteRequest.Allocate(url, headers, callback, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
        }

        /// <summary>
        /// DELETE 返回字节数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        public static void DeleteByteArr(string url, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.DELETE, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbackByteArr);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbackByteArr);
                Action<string, long,byte[], object[]> callback = (uurl, code,bs, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.DELETE, uurl, code, bs, aargs);
                };
                NetDeleteRequest request = NetDeleteRequest.Allocate(url, headers, callback, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// DELETE 返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callbackStr"></param>
        /// <param name="progress"></param>
        public static void DeleteString(string url, Dictionary<string, string> headers = null,
           Action<string, long, string, object[]> callbackStr = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.DELETE, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbackStr);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.DELETE, url, callbackStr);
                Action<string, long, string, object[]> callback = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.DELETE, uurl, code, content, aargs);
                };
                NetDeleteRequest request = NetDeleteRequest.Allocate(url, headers, callback, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
           
        }

        #endregion

        #region POST
        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventNone(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, object[]> callbackt = (uurl, code, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataForm, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, byte[], object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataForm, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataForm"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, WWWForm postDataForm, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, string, object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataForm, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventNone(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, object[]> callbackt = (uurl, code, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataDic, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long,byte[], object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataDic, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataDic"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, Dictionary<string, string> postDataDic, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, string, object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataDic, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventNone(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, object[]> callbackt = (uurl, code, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataStr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, byte[], object[]> callbackt = (uurl, code,content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataStr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, string postDataStr, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, string, object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataStr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
           
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventNone(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, object[]> callbackt = (uurl, code, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataByteArr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, byte[], object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventByteArr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, byte[], object[]> callbackt = (uurl, code,content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataByteArr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }

        /// <summary>
        /// 分配一个实例空间，并开启请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataByteArr"></param>
        /// <param name="headers"></param>
        /// <param name="callback"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void Post(string url, byte[] postDataByteArr, Dictionary<string, string> headers = null,
            Action<string, long, string, object[]> callback = null, Action<float> progress = null, params object[] args)
        {
            if (NetEventTmpSave.HasEventStr(NetEventTmpSave.EventType.POST, url))
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
            }
            else
            {
                NetEventTmpSave.AddEvent(NetEventTmpSave.EventType.POST, url, callback);
                Action<string, long, string, object[]> callbackt = (uurl, code, content, aargs) =>
                {
                    NetEventTmpSave.Invoke(NetEventTmpSave.EventType.POST, uurl, code, content, aargs);
                };
                NetPostRequest request = NetPostRequest.Allocate(url, postDataByteArr, headers, callbackt, progress, args);
                Instance.StartCoroutine(request.IEnumStart());
            }
            
        }
        #endregion
    }
}