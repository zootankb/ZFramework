using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// 消息中心
    /// </summary>
    public class UIMsgCenter
    {
        #region Data
        /// <summary>
        /// 消息列表
        /// </summary>
        private UIMsgNodeLinked msgLinked = null;

        /// <summary>
        /// 操作者名字
        /// </summary>
        private string operatorName = null;
        #endregion

        #region Constructor

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="operatorName">操作者名字</param>
        /// <param name="eventSize">事件池子大小</param>
        private UIMsgCenter(string operatorName = null, int eventSize = 0)
        {
            this.operatorName = operatorName;
            this.msgLinked = new UIMsgNodeLinked((uint)eventSize);
        }

        #endregion

        #region Pub Func
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Register(int eventId, Action<int,ZMsg> ets)
        {
            msgLinked.AddNode(eventId, ets);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="msg"></param>
        public void Register(UIMsg msg)
        {
            msgLinked.AddNode(msg);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="node"></param>
        public void Register(UIMsgNode node)
        {
            msgLinked.AddNode(node);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void Unregister(int eventId, Action<int, ZMsg> msg)
        {
            msgLinked.SubNode(eventId, msg);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="msg"></param>
        public void Unregister(UIMsg msg)
        {
            msgLinked.SubNode(msg);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="node"></param>
        public void Unregister(UIMsgNode node)
        {
            msgLinked.SubNode(node);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void SendMsg(int eventId, ZMsg msg)
        {
            msgLinked.SendMsg(eventId, msg);
        }
        #endregion

        #region Private Static Data
        /// <summary>
        /// UI消息中心的主题部分
        /// </summary>
        private static Dictionary<string, UIMsgCenter> centers = new Dictionary<string, UIMsgCenter>();
        #endregion

        #region API
        
        /// <summary>
        /// 分配一个消息空间
        /// </summary>
        /// <param name="operatorName"></param>
        /// <param name="eventSize"></param>
        /// <returns></returns>
        public static UIMsgCenter Allocate(string operatorName, int eventSize = 0)
        {
            if (centers.ContainsKey(operatorName))
            {
                return centers[operatorName];
            }
            else
            {
                UIMsgCenter center = new UIMsgCenter(operatorName, eventSize);
                centers.Add(operatorName, center);
                return center;
            }
        }

        /// <summary>
        /// 释放消息中心
        /// </summary>
        /// <param name="operatorName"></param>
        public static void Release(string operatorName)
        {
            if (centers.ContainsKey(operatorName))
            {
                centers.Remove(operatorName);
            }
        }

        /// <summary>
        /// 是否拥有消息中心
        /// </summary>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        public static bool HasMsgCenter(string operatorName)
        {
            return centers.ContainsKey(operatorName);
        }
        #endregion
    }
}