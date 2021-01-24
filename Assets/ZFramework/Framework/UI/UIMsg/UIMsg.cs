using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UI内部之间的消息传递基类
    /// </summary>
    public class UIMsg : IUIMsg
    {
        /// <summary>
        /// 事件id
        /// </summary>
        private int eventId = -1;

        /// <summary>
        /// 事件存储
        /// </summary>
        private Action<int, ZMsg> ets = null;

        /// <summary>
        /// 事件id
        /// </summary>
        public int EventId
        {
            get { return eventId; }
        }

        /// <summary>
        /// 消息体
        /// </summary>
        public Action<int, ZMsg> Ets
        {
            get { return ets; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public UIMsg(int eventId = -1, Action<int, ZMsg> ets = null)
        {
            this.eventId = eventId;
            this.ets = ets;
        }

        /// <summary>
        /// 事件id
        /// </summary>
        /// <param name="eventId"></param>
        public bool HasEventId(int eventId)
        {
            return this.eventId == eventId;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void SendMsg(int eventId, ZMsg msg)
        {
            if (this.eventId == eventId)
            {
                ets?.Invoke(eventId, msg);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Register(int eventId, Action<int, ZMsg> ets)
        {
            if (this.eventId == eventId)
            {
                this.ets += ets;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Unregister(int eventId, Action<int, ZMsg> ets)
        {
            if (this.eventId == eventId)
            {
                this.ets -= ets;
            }
        }
    }
}