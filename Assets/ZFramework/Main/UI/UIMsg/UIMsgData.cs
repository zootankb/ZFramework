using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UIMsg的数据
    /// </summary>
    public class UIMsgData
    {
        /// <summary>
        /// 事件id
        /// </summary>
        private int eventId = -1;

        /// <summary>
        /// 消息体
        /// </summary>
        private ZMsg msg = null;

        /// <summary>
        /// 事件存储
        /// </summary>
        private Action<int, ZMsg> ets = null;

        public UIMsgData()
        {

        }

        public UIMsgData(int eventId, ZMsg msg)
        {
            this.eventId = eventId;
            this.msg = msg;
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
                this.ets?.Invoke(eventId, msg);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Register(int eventId, Action<int, ZMsg> ets)
        {
            if(this.eventId == eventId)
            {
                this.ets += ets;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Unregister(int eventId, Action<int,ZMsg> ets)
        {
            if (this.eventId == eventId)
            {
                this.ets -= ets;
            }
        }
    }
}