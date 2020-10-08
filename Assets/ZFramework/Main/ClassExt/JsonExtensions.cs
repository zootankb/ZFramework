using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using ZFramework.Log;

namespace ZFramework.ClassExt
{
    /// <summary>
    /// json的扩展操作
    /// </summary>
    public static class JsonExtensions
    {

        /// <summary>
        /// 转化为实例，忽略空字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToTObject<T>(this string json) where T : class,new ()
        {
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                T t = JsonConvert.DeserializeObject<T>(json, setting);
                return t;
            }
            catch (JsonSerializationException e)
            {
                LogOperator.AddFinalRecord(string.Format("转换json字符串 {0} 到类型 {1} 异常，异常原因：", json, typeof(T).Name, e.Message));
                return null;
            }
        }

        /// <summary>
        /// 转化为json字符串，忽略空字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string TObjectToJson<T>(this T t) where T : class, new()
        {
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                string json = JsonConvert.SerializeObject(t, setting);
                return json;
            }
            catch (JsonSerializationException e)
            {
                LogOperator.AddFinalRecord(string.Format("转换 {0} 类型 为json字符串异常，异常原因：", typeof(T).Name, e.Message));
                return null;
            }
        }
    }

    
}