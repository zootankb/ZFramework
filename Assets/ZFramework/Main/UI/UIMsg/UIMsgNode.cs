using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// 消息链表
    /// </summary>
    public class UIMsgNode 
    {
        /// <summary>
        /// 下一个消息体的指针
        /// </summary>
        public UIMsgNode nextNode = null;

        /// <summary>
        /// 消息内容
        /// </summary>
        public UIMsg value = null;

        public UIMsgNode(UIMsg value = null, UIMsgNode nextNode = null)
        {
            this.value = value;
            this.nextNode = nextNode;
        }
    }
}