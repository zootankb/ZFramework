﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZFramework.Log;

namespace ZFramework.ClassExt
{
    /// <summary>
    /// json字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 文件锁
        /// </summary>
        private static readonly object _locker = new object();

        /// <summary>
        /// 类转换成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToNewtonJson<T>(this T t) where T : class
        {
            string jsonconfig = null;
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                jsonconfig = JsonConvert.SerializeObject(t, setting);
            }
            catch (JsonException e)
            {
                LogOperator.AddFinalRecord("ClassExtension.ToNewtonJson序列化时异常", "异常原因：" + e.Message);
            }
            return jsonconfig;
        }

        /// <summary>
        /// 把字符串反序列化成指定类型T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToNewtonObjectT<T>(this string json) where T : class, new()
        {
            T t = null;
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                t = JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonException e)
            {
                LogOperator.AddFinalRecord("StringExtension.FromObjectT反序列化时异常", "异常原因：" + e.Message);
            }
            return t;
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteAppend(this string path, string content)
        {
            path.CheckOrCreateFile();
            lock (_locker)
            {
                using (FileStream fs = new FileStream(path, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(content);
                    }
                }
            }
        }

        /// <summary>
        /// 写入数据流
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bs"></param>
        public static void WriteAppend(this string path, byte[] bs)
        {
            path.CheckOrCreateFile();
        }

        /// <summary>
        /// 把字符串写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteTextAssetContentStr(this string path, string content)
        {
            if (File.Exists(path))
                File.Delete(path);
            lock (_locker)
            {
                string dirPath = Path.GetDirectoryName(path);
                dirPath.CheckOrCreateDir();
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(content);
                    }
                }
            }
        }

        /// <summary>
        /// 把数据流写进文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bs"></param>
        public static void WriteTextAssetContentByteArray(this string path, byte[] bs)
        {
            if (File.Exists(path))
                File.Delete(path);
            lock (_locker)
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(bs);
                    }
                }
            }
        }

        /// <summary>
        /// 获取路径文件里面的字符串内容，格式为utf-8
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTextAssetContentStr(this string path)
        {
            try
            {
                string content = null;
                lock (_locker)
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                return content;
            }
            catch (IOException e)
            {
                LogOperator.AddResErrorRecord("获取文件字符串时有误", e.Message, "文件路径：", path);
                return null;
            }
        }

        /// <summary>
        /// 获取路径文件里面的数据流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetTextAssetContentByteArr(this string path)
        {
            try
            {

                byte[] bs = null;
                lock (_locker)
                {
                    FileInfo fi = new FileInfo(path);
                    bs = new byte[fi.Length];
                    using(FileStream fs = fi.OpenRead())
                    {
                        fs.Read(bs, 0, Convert.ToInt32(fs.Length));
                    }
                }
                return bs;
            }
            catch (IOException e)
            {
                LogOperator.AddResErrorRecord("获取文件字符串时有误", e.Message, "文件路径：", path);
                return null;
            }
        }

        /// <summary>
        /// 检查并创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CheckOrCreateDir(this string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 检查并强制删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CheckForDeleteDir(this string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
        }

        /// <summary>
        /// 检查并创建文件
        /// </summary>
        /// <param name="path"></param>
        public static void CheckOrCreateFile(this string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
        }

        /// <summary>
        /// 检查并删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void CheckForDeleteFile(this string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 从URL或本地path中提取文件的名字，以便能直接把url中的文件名字提取出来
        /// 防止url中出现类似 https://ss0.bdstatic.com/70cFuHSh_Q1YnxGkpoWK1HF6hhy/it/u=2636238737,1519537265&fm=26&gp=0.jpg 的例子
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetFileNameFromUrlOrPath(this string content)
        {
            string filename = Path.GetFileName(content);
            if (filename.Contains("&"))
            {
                int startIndex = filename.LastIndexOf("&") + 1;
                filename = filename.Substring(startIndex, filename.Length - startIndex );
            }
            if (filename.Contains("="))
            {
                int startIndex = filename.LastIndexOf("=") + 1;
                filename = filename.Substring(startIndex, filename.Length - startIndex );
            }
            return filename;
        }
    }
}