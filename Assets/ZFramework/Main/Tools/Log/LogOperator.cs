using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework.Log
{
    /// <summary>
    /// 日志的操作
    /// </summary>
    public static class LogOperator
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public enum EnumLogLevel
        {
            /// <summary>
            /// 正常日志
            /// </summary>
            [Description("正常日志")]
            Log = 1,
            /// <summary>
            /// 警告
            /// </summary>
            [Description("警告")]
            Warnning,
            /// <summary>
            /// 资源错误
            /// </summary>
            [Description("资源错误")]
            ResError,
            /// <summary>
            /// 网络错误
            /// </summary>
            [Description("网络错误")]
            NetError,
            /// <summary>
            /// 不可变错误
            /// </summary>
            [Description("不可变错误")]
            Final,
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        private static readonly string LogFilePath = string.Format("{0}/{1}_Log.txt", Application.persistentDataPath, Application.productName);

        /// <summary>
        /// 添加log级别日志
        /// </summary>
        /// <param name="args"></param>
        public static void AddLogRecord(params object[] args)
        {
            var conList = args.Select(arg => arg.ToString());
            string content = string.Join(", ", conList);
            AddLogRecord(EnumLogLevel.Log, content);
        }

        /// <summary>
        /// 添加Warnning级别日志
        /// </summary>
        /// <param name="args"></param>
        public static void AddWarnningRecord(params object[] args)
        {
            var conList = args.Select(arg => arg.ToString());
            string content = string.Join(", ", conList);
            AddLogRecord(EnumLogLevel.Warnning, content);
        }

        /// <summary>
        /// 添加ResError级别日志
        /// </summary>
        /// <param name="args"></param>
        public static void AddResErrorRecord(params object[] args)
        {
            var conList = args.Select(arg => arg.ToString());
            string content = string.Join(", ", conList);
            AddLogRecord(EnumLogLevel.ResError, content);
        }

        /// <summary>
        /// 添加NetError级别日志
        /// </summary>
        /// <param name="args"></param>
        public static void AddNetErrorRecord(params object[] args)
        {
            var conList = args.Select(arg => arg.ToString());
            string content = string.Join(", ", conList);
            AddLogRecord(EnumLogLevel.NetError, content);
        }

        /// <summary>
        /// 添加Final级别日志
        /// </summary>
        /// <param name="args"></param>
        public static void AddFinalRecord(params object[] args)
        {
            var conList = args.Select(arg => arg.ToString());
            string content = string.Join(", ", conList);
            AddLogRecord(EnumLogLevel.Final, content);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="logContent"></param>
        public static void AddLogRecord(EnumLogLevel level = EnumLogLevel.Log, string logContent = null)
        {
            LogFilePath.CheckOrCreateFile();
            string header = string.Empty;
            switch (level)
            {
                case EnumLogLevel.Log:
                    header = string.Format("{0} {1}:\t", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), level.ToString());
                    break;
                case EnumLogLevel.Warnning:
                    header = string.Format("{0} {1}:\t", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), level.ToString());
                    break;
                case EnumLogLevel.ResError:
                    header = string.Format("{0} {1}:\t", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), level.ToString());
                    break;
                case EnumLogLevel.NetError:
                    header = string.Format("{0} {1}:\t", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), level.ToString());
                    break;
                case EnumLogLevel.Final:
                    header = string.Format("{0} {1}:\t", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), level.ToString());
                    break;
            }
            string content = string.Format("{0}{1}\r\n\r\n", header, logContent);
            LogFilePath.WriteAppend(content);
        }
    }
}