using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace ZFramework.Res
{
    /// <summary>
    /// 资源的临时存储到内存中
    /// </summary>
    public static class ResTmpSave
    {
        #region 由于VideoClip没有能直接下载的组件，所以返回的时候直接返回一个已经下载好的本地的路径，由VideoPlayer直接加载使用
        /// <summary>
        /// 自定义使用的VideoClip
        /// </summary>
        public sealed class NetVideoClip : UnityEngine.Object
        {
            /// <summary>
            /// url链接，尽量不要使用此属性，只作为记录url地址使用而已
            /// </summary>
            public string url;
            /// <summary>
            /// 本地存储路径，在使用此属性时说明视频已经存储到了本地，并且只使用此属性
            /// </summary>
            public string path;

            public NetVideoClip() : base()
            {
            }

            public NetVideoClip(string url, string path):base()
            {
                this.url = url;
                this.path = path;
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(path) ? "path null" : url;
            }

            public override bool Equals(object other)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

        }
        #endregion


        #region 静态构造函数和数据

        /// <summary>
        /// ab包的信息
        /// </summary>
        private static ResInfo<AssetBundle> abInfo = new ResInfo<AssetBundle>();

        /// <summary>
        /// Texture2D的信息
        /// </summary>
        private static ResInfo<Texture2D> t2dInfo = new ResInfo<Texture2D>();

        /// <summary>
        /// sprite的信息
        /// </summary>
        private static ResInfo<Sprite> sprInfo = new ResInfo<Sprite>();

        /// <summary>
        /// TextAsset的信息
        /// </summary>
        private static ResInfo<TextAsset> txtatInfo = new ResInfo<TextAsset>();

        /// <summary>
        /// audioclip的信息
        /// </summary>
        private static ResInfo<AudioClip> audioInfo = new ResInfo<AudioClip>();

        /// <summary>
        /// videoclip的信息
        /// </summary>
        private static ResInfo<NetVideoClip> videoInfo = new ResInfo<NetVideoClip>();
        #endregion

        #region API

        /// <summary>
        /// 分发消息后即刻调用SubEvent清除消息事件和俩的key值，并根据url和filepath存储临时内存资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="code"></param>
        /// <param name="t"></param>
        /// <param name="args"></param>
        public static void Invoke<T>(string url, long code, T t, object[] args)
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                if (t == null)
                {
                    abInfo.Invoke(url, code, null, args);
                }
                else
                {
                    abInfo.Invoke(url, code, t as AssetBundle, args);
                }
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                if (t == null)
                {
                    t2dInfo.Invoke(url, code, null, args);
                }
                else
                {
                    t2dInfo.Invoke(url, code, t as Texture2D, args);
                }
            }
            else if (typeof(Sprite) == typeof(T))
            {
                if (t == null)
                {
                    sprInfo.Invoke(url, code, null, args);
                }
                else
                {
                    sprInfo.Invoke(url, code, t as Sprite, args);
                }
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                if (t == null)
                {
                    txtatInfo.Invoke(url, code, null, args);
                }
                else
                {
                    txtatInfo.Invoke(url, code, t as TextAsset, args);
                }
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                if (t == null)
                {
                    audioInfo.Invoke(url, code, null, args);
                }
                else
                {
                    audioInfo.Invoke(url, code, t as AudioClip, args);
                }
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                if (t == null)
                {
                    videoInfo.Invoke(url, code, null, args);
                }
                else
                {
                    videoInfo.Invoke(url, code, t as NetVideoClip, args);
                }
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void SubEvent<T>(string url, Action<string, long, T, object[]> callback = null) where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                abInfo.SubEvent(url, callback as Action<string, long, AssetBundle, object[]>);
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                t2dInfo.SubEvent(url, callback as Action<string, long, Texture2D, object[]>);
            }
            else if (typeof(Sprite) == typeof(T))
            {
                sprInfo.SubEvent(url, callback as Action<string, long, Sprite, object[]>);
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                txtatInfo.SubEvent(url, callback as Action<string, long, TextAsset, object[]>);
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                audioInfo.SubEvent(url, callback as Action<string, long, AudioClip, object[]>);
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                videoInfo.SubEvent(url, callback as Action<string, long, NetVideoClip, object[]>);
            }
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void AddEvent<T>(string url, Action<string, long, T, object[]> callback = null) where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                abInfo.AddEvent(url, callback as Action<string, long, AssetBundle, object[]>);
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                t2dInfo.AddEvent(url, callback as Action<string, long, Texture2D, object[]>);
            }
            else if (typeof(Sprite) == typeof(T))
            {
                sprInfo.AddEvent(url, callback as Action<string, long, Sprite, object[]>);
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                txtatInfo.AddEvent(url, callback as Action<string, long, TextAsset, object[]>);
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                audioInfo.AddEvent(url, callback as Action<string, long, AudioClip, object[]>);
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                videoInfo.AddEvent(url, callback as Action<string, long, NetVideoClip, object[]>);
            }
        }

        /// <summary>
        /// 根据url获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetContentT<T>(string url) where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                var t = abInfo.GetContentT(url);
                return t == null ? null : t as T;
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                var t = t2dInfo.GetContentT(url);
                return t == null ? null : t as T;
            }
            else if (typeof(Sprite) == typeof(T))
            {
                var t = sprInfo.GetContentT(url);
                return t == null ? null : t as T;
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                var t = txtatInfo.GetContentT(url);
                return t == null ? null : t as T;
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                var t = audioInfo.GetContentT(url);
                return t == null ? null : t as T;
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                
                var t = videoInfo.GetContentT(url);
                bool isNotNull = false;
                try
                {
                    // 暂时不知道继承UnityEngine.Object后的类怎么写不等于null的代码，暂时用一个try验证
                    string n = t.ToString();
                    isNotNull = true;
                }
                catch(Exception e)
                {
                    isNotNull = false;
                    Log.LogOperator.AddWarnningRecord("获取视频的时候触发了临时操作事件", "异常信息：" + e.Message);
                }
                return isNotNull ? null : t as T;
            }
            return null;
        }

        /// <summary>
        /// 是否已经存储资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="utl"></param>
        /// <returns></returns>
        public static bool HasContentT<T>(string url) where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                return abInfo.HasContentT(url);
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                return t2dInfo.HasContentT(url);
            }
            else if (typeof(Sprite) == typeof(T))
            {
                return sprInfo.HasContentT(url);
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                return txtatInfo.HasContentT(url);
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                return audioInfo.HasContentT(url);
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                return videoInfo.HasContentT(url);
            }
            return false;
        }

        /// <summary>
        /// 清除内存中指定类型的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        public static void ClearResInfo<T>() where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                abInfo.ClearAllT();
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                t2dInfo.ClearAllT();
            }
            else if (typeof(Sprite) == typeof(T))
            {
                sprInfo.ClearAllT();
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                txtatInfo.ClearAllT();
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                audioInfo.ClearAllT();
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                videoInfo.ClearAllT();
            }
        }

        /// <summary>
        /// 清除内存中指定的url
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        public static void ClearResInfo<T>(string url) where T : UnityEngine.Object
        {
            if (typeof(AssetBundle) == typeof(T))
            {
                abInfo.ClearT(url);
            }
            else if (typeof(Texture2D) == typeof(T))
            {
                t2dInfo.ClearT(url);
            }
            else if (typeof(Sprite) == typeof(T))
            {
                sprInfo.ClearT(url);
            }
            else if (typeof(TextAsset) == typeof(T))
            {
                txtatInfo.ClearT(url);
            }
            else if (typeof(AudioClip) == typeof(T))
            {
                audioInfo.ClearT(url);
            }
            else if (typeof(NetVideoClip) == typeof(T))
            {
                videoInfo.ClearT(url);
            }
        }



        #endregion

        #region Pri Class
        /// <summary>
        /// 内部辅助类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ResInfo<T> where T : UnityEngine.Object
        {
            /// <summary>
            /// 保存的数据字典
            /// </summary>
            private Dictionary<string, T> saveConDic = null;

            /// <summary>
            /// 保存的事件链接
            /// </summary>
            private Dictionary<string, Action<string, long, T, object[]>> eventDic = null;

            public ResInfo()
            {
                saveConDic = new Dictionary<string, T>();
                eventDic = new Dictionary<string, Action<string, long, T, object[]>>();
            }

            /// <summary>
            /// 分发消息
            /// </summary>
            /// <param name="url"></param>
            /// <param name="code"></param>
            /// <param name="t"></param>
            /// <param name="args"></param>
            public void Invoke(string url, long code, T t, object[] args)
            {
                if (eventDic.ContainsKey(url))
                {
                    eventDic[url]?.Invoke(url, code, t, args);
                    eventDic.Remove(url);
                }
                if (typeof(T) == typeof(NetVideoClip) || t != null)
                {
                    saveConDic.Add(url, t);
                }
            }

            /// <summary>
            /// 判断是否存有资源
            /// </summary>
            /// <param name="url"></param>
            public bool HasContentT(string url)
            {
                return saveConDic.ContainsKey(url);
            }

            /// <summary>
            /// 获取资源T
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public T GetContentT(string url)
            {
                return saveConDic.ContainsKey(url) ? saveConDic[url] : null;
            }

            /// <summary>
            /// 添加事件
            /// </summary>
            /// <param name="url"></param>
            /// <param name="e"></param>
            public void AddEvent(string url, Action<string, long, T, object[]> e)
            {
                if (eventDic.ContainsKey(url))
                {
                    eventDic[url] += e;
                }
                else
                {
                    eventDic.Add(url, e);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="url"></param>
            /// <param name="e"></param>
            public void SubEvent(string url, Action<string, long, T, object[]> e)
            {
                if (eventDic.ContainsKey(url))
                {
                    eventDic[url] -= e;
                }
            }

            /// <summary>
            /// 清除事件链表
            /// </summary>
            /// <param name="url"></param>
            public void ClearEvent(string url)
            {
                if (eventDic.ContainsKey(url))
                {
                    eventDic[url] = null;
                }
            }

            /// <summary>
            /// 添加并覆盖T
            /// </summary>
            /// <param name="url"></param>
            /// <param name="t"></param>
            public void AddT(string url, T t)
            {
                if (saveConDic.ContainsKey(url))
                {
                    saveConDic[url] = t;
                }
                else
                {
                    saveConDic.Add(url, t);
                }
            }

            /// <summary>
            /// 清除T
            /// </summary>
            /// <param name="url"></param>
            public void ClearT(string url)
            {
                if (saveConDic.ContainsKey(url))
                {
                    if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Sprite) || typeof(T) == typeof(NetVideoClip)
                        || typeof(T) == typeof(TextAsset) || typeof(T) == typeof(AudioClip))
                    {
                        MonoBehaviour.DestroyImmediate(saveConDic[url]);
                    }
                    if(typeof(T) == typeof(AssetBundle))
                    {
                        (saveConDic[url] as AssetBundle).Unload(false);
                    }
                    saveConDic.Remove(url);
                }
            }

            /// <summary>
            /// 清除缓存的所有资源
            /// </summary>
            public void ClearAllT()
            {
                foreach (var item in saveConDic)
                {
                    if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Sprite) || typeof(T) == typeof(NetVideoClip)
                        || typeof(T) == typeof(TextAsset) || typeof(T) == typeof(AudioClip))
                    {
                        MonoBehaviour.DestroyImmediate(item.Value);
                    }
                    if (typeof(T) == typeof(AssetBundle))
                    {
                        (item.Value as AssetBundle).Unload(false);
                    }
                }
                saveConDic.Clear();
            }
        }
        #endregion

    }
}