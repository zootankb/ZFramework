using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}