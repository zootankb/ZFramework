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
        /// json字符串转化为数据结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToTClass<T>(this string json) where T : class
        {
            try
            {
                return null;
            }
            catch(Exception e)
            {
                LogOperator.AddFinalRecord(string.Format("转换json字符串 {0} 到类型 {1} 异常，异常原因：", json, typeof(T).Name, e.Message));
                return null;
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