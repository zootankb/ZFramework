using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Net
{
    /// <summary>
    /// Net事件临时存储区，防止短时间内调用统一重复事件
    /// </summary>
    public static class NetTempEvent
    {
        /// <summary>
        /// 记录请求空事件
        /// </summary>
        private static Dictionary<string, Action<string, long, object[]>> eventNoneDic = new Dictionary<string, Action<string, long, object[]>>();

        /// <summary>
        /// 记录请求bytearr事件
        /// </summary>
        private static Dictionary<string, Action<string, long, byte[], object[]>> eventBsDic = new Dictionary<string, Action<string, long, byte[], object[]>>();

        /// <summary>
        /// 记录请求string事件
        /// </summary>
        private static Dictionary<string, Action<string, long, string, object[]>> eventStrDic = new Dictionary<string, Action<string, long, string, object[]>>();

        /// <summary>
        /// 添加请求空事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(string url, Action<string, long, object[]> callback)
        {
            if (eventNoneDic.ContainsKey(url))
            {
                eventNoneDic[url] += callback;
            }
            else
            {
                eventNoneDic.Add(url, callback);
            }
        }

        /// <summary>
        /// 添加请求bytearr事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(string url, Action<string, long, byte[], object[]> callback)
        {
            if (eventBsDic.ContainsKey(url))
            {
                eventBsDic[url] += callback;
            }
            else
            {
                eventBsDic.Add(url, callback);
            }
        }

        /// <summary>
        /// 添加请求string事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(string url, Action<string, long, string, object[]> callback)
        {
            if (eventNoneDic.ContainsKey(url))
            {
                eventStrDic[url] += callback;
            }
            else
            {
                eventStrDic.Add(url, callback);
            }
        }

        /// <summary>
        /// 移除请求空事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(string url, Action<string, long, object[]> callback)
        {
            if (eventNoneDic.ContainsKey(url))
            {
                eventNoneDic[url] -= callback;
            }
        }

        /// <summary>
        /// 移除请求bytearr事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(string url, Action<string, long, byte[], object[]> callback)
        {
            if (eventBsDic.ContainsKey(url))
            {
                eventBsDic[url] -= callback;
            }
        }

        /// <summary>
        /// 移除请求string事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(string url, Action<string, long, string, object[]> callback)
        {
            if (eventStrDic.ContainsKey(url))
            {
                eventStrDic[url] -= callback;
            }
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        public static void Invoke(string url, long code, object[] args)
        {
            if (eventNoneDic.ContainsKey(url))
            {
                eventNoneDic[url]?.Invoke(url, code, args);
                eventNoneDic.Remove(url);
            }
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="bs"></param>
        /// <param name="args"></param>
        public static void Invoke(string url, long code,byte[] bs, object[] args)
        {
            if (eventBsDic.ContainsKey(url))
            {
                eventBsDic[url]?.Invoke(url, code, bs, args);
                eventBsDic.Remove(url);
            }
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="content"></param>
        /// <param name="args"></param>
        public static void Invoke(string url, long code, string content, object[] args)
        {
            if (eventStrDic.ContainsKey(url))
            {
                eventStrDic[url]?.Invoke(url, code, content, args);
                eventStrDic.Remove(url);
            }
        }
    }
}