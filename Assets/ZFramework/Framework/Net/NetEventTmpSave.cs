using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Net
{
    /// <summary>
    /// Net事件临时存储区，防止短时间内调用统一重复事件
    /// </summary>
    public static class NetEventTmpSave
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// GET方式
            /// </summary>
            GET,
            /// <summary>
            /// 删除方式
            /// </summary>
            DELETE,
            /// <summary>
            /// POST方式
            /// </summary>
            POST
        }

        /// <summary>
        /// 事件分类区
        /// </summary>
        private static Dictionary<EventType, TempEvent> eventDic = new Dictionary<EventType, TempEvent>();

        static NetEventTmpSave()
        {
            // 把所有的事件分类全部加进去
            foreach (EventType t in Enum.GetValues(typeof(EventType)))
            {
                eventDic.Add(t, new TempEvent());
            }
        }

        #region API
        /// <summary>
        /// 是否拥有字符串事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool HasEventNone(EventType t, string url)
        {
            return eventDic[t].HasEventNone(url);
        }
        /// <summary>
        /// 是否拥有字符串事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool HasEventByteArr(EventType t, string url)
        {
            return eventDic[t].HasEventByteArr(url);
        }
        /// <summary>
        /// 是否拥有字符串事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool HasEventStr(EventType t, string url)
        {
            return eventDic[t].HasEventStr(url);
        }

        /// <summary>
        /// 添加请求空事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(EventType t, string url, Action<string, long, object[]> callback)
        {
            eventDic[t].AddEvent(url, callback);
        }

        /// <summary>
        /// 添加请求字节数组事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(EventType t, string url, Action<string, long,byte[], object[]> callback)
        {
            eventDic[t].AddEvent(url, callback);
        }

        /// <summary>
        /// 添加请求字符串事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent(EventType t, string url, Action<string, long,string, object[]> callback)
        {
            eventDic[t].AddEvent(url, callback);
        }

        /// <summary>
        /// 移除请求空事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(EventType t, string url, Action<string, long, object[]> callback)
        {
            eventDic[t].SubEvent(url, callback);
        }

        /// <summary>
        /// 移除请求字节数组事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(EventType t, string url, Action<string, long, byte[], object[]> callback)
        {
            eventDic[t].SubEvent(url, callback);
        }

        /// <summary>
        /// 移除请求字符串事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent(EventType t, string url, Action<string, long, string, object[]> callback)
        {
            eventDic[t].SubEvent(url, callback);
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        public static void Invoke(EventType t, string url, long code, object[] args)
        {
            eventDic[t].Invoke(url, code, args);
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="bs"></param>
        /// <param name="args"></param>
        public static void Invoke(EventType t, string url, long code, byte[] bs, object[] args)
        {
            eventDic[t].Invoke(url, code, bs, args);
        }

        /// <summary>
        /// 执行事件，后并移除事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="str"></param>
        /// <param name="args"></param>
        public static void Invoke(EventType t, string url, long code, string str, object[] args)
        {
            eventDic[t].Invoke(url, code, str, args);
        }
        #endregion

        public class TempEvent
        {
            /// <summary>
            /// 记录请求空事件
            /// </summary>
            private  Dictionary<string, Action<string, long, object[]>> eventNoneDic = new Dictionary<string, Action<string, long, object[]>>();

            /// <summary>
            /// 记录请求bytearr事件
            /// </summary>
            private  Dictionary<string, Action<string, long, byte[], object[]>> eventBsDic = new Dictionary<string, Action<string, long, byte[], object[]>>();

            /// <summary>
            /// 记录请求string事件
            /// </summary>
            private  Dictionary<string, Action<string, long, string, object[]>> eventStrDic = new Dictionary<string, Action<string, long, string, object[]>>();

            /// <summary>
            /// 是否拥有返回空事件
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public  bool HasEventNone(string url)
            {
                return eventNoneDic.ContainsKey(url);
            }

            /// <summary>
            /// 是否拥有字节数组事件
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public  bool HasEventByteArr(string url)
            {
                return eventBsDic.ContainsKey(url);
            }

            /// <summary>
            /// 是否拥有字符串事件
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public  bool HasEventStr(string url)
            {
                return eventStrDic.ContainsKey(url);
            }

            /// <summary>
            /// 添加请求空事件
            /// </summary>
            /// <param name="url"></param>
            /// <param name="callback"></param>
            public  void AddEvent(string url, Action<string, long, object[]> callback)
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
            public  void AddEvent(string url, Action<string, long, byte[], object[]> callback)
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
            public  void AddEvent(string url, Action<string, long, string, object[]> callback)
            {
                if (eventStrDic.ContainsKey(url))
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
            public  void SubEvent(string url, Action<string, long, object[]> callback)
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
            public  void SubEvent(string url, Action<string, long, byte[], object[]> callback)
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
            public  void SubEvent(string url, Action<string, long, string, object[]> callback)
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
            public  void Invoke(string url, long code, object[] args)
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
            public  void Invoke(string url, long code, byte[] bs, object[] args)
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
            public  void Invoke(string url, long code, string content, object[] args)
            {
                if (eventStrDic.ContainsKey(url))
                {
                    eventStrDic[url]?.Invoke(url, code, content, args);
                    eventStrDic.Remove(url);
                }
            }
        }
    }
}