using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Net
{
    /// <summary>
    /// 接受数据的基类
    /// </summary>
    [System.Serializable]
    internal class BaseResponse<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int code = 0;

        /// <summary>
        /// 返回描述信息
        /// </summary>
        public string msg = string.Empty;

        /// <summary>
        /// 状态码
        /// </summary>
        public int status = 0;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error = string.Empty;

        /// <summary>
        /// 详细信息
        /// </summary>
        public string message = string.Empty;

        /// <summary>
        /// 路径指引
        /// </summary>
        public string path = string.Empty;

        /// <summary>
        /// 数据信息
        /// </summary>
        public T data;

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

        public BaseResponse(T data, int code = 0, string msg = null, int status = 0, string error =null, string message = null, string path = null)
        {
            this.data = data;
            this.code = code;
            this.msg = msg;
            this.status = status;
            this.error = error;
            this.message = message;
            this.path = path;
        }

        public override string ToString()
        {
            return string.Format("code={0}, msg={1}, status={2}, error={3}, message={4}, path={5}, data={6}, timestamp={7}", code, msg, status, error, message, path, data, timestamp);
        }
    }
}