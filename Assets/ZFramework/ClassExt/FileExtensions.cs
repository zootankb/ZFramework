using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ZFramework.Log;

namespace ZFramework.ClassExt
{
    /// <summary>
    /// 文件的操作
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// 文件锁
        /// </summary>
        private static readonly object _locker = new object();

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
            catch(IOException e)
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
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bs = new byte[fs.Length];
                        fs.Read(bs, 0, bs.Length);
                    }
                }
                return bs;
            }
            catch (IOException e)
            {
                LogOperator.AddResErrorRecord("获取文件字符串时有误",e.Message, "文件路径：", path);
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
        /// 检查并创建文件
        /// </summary>
        /// <param name="path"></param>
        public static void CheckOrCreateFile(this string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }
    }
}