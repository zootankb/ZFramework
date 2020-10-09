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
        /// 消息数据转储
        /// </summary>
        private UIMsgData msgData = null;

        public UIMsg(UIMsgData msgData = null)
        {
            this.msgData = msgData ?? new UIMsgData();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void SendMsg(int eventId, ZMsg msg)
        {
            msgData.SendMsg(eventId, msg);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Register(int eventId, Action<int, ZMsg> ets)
        {
            msgData.Register(eventId, ets);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ets"></param>
        public void Unregister(int eventId, Action<int, ZMsg> ets)
        {
            msgData.Unregister(eventId, ets);
        }
    }
}