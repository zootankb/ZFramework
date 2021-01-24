using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.Log;

namespace ZFramework.ZEditor
{
    /// <summary>
    /// 类的扩展
    /// </summary>
    public static class ClassExtension 
    {
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
    }
}