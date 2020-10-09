using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// 消息链表
    /// </summary>
    public class UIMsgNodeLinked
    {
        /// <summary>
        /// 节点数量
        /// </summary>
        private int count = 0;

        /// <summary>
        /// 链表的起始点
        /// </summary>
        private UIMsgNode root = null;

        /// <summary>
        /// 最后一个节点
        /// </summary>
        private UIMsgNode lastNode = null;

        /// <summary>
        /// 链表容量，初始化时使用，若为0则无线长度
        /// </summary>
        private uint size = 0;

        /// <summary>
        /// 节点数量
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// 节点容量
        /// </summary>
        public uint Size
        {
            get { return size; }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="size"></param>
        public UIMsgNodeLinked(uint size = 0)
        {
            this.size = size;
            this.lastNode = this.root;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void AddNode(int eventId, Action<int, ZMsg> msg)
        {
            AddNode(new UIMsgNode())
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(UIMsgNode node)
        {
            if(node == null)
            {
                return;
            }
            if(size == 0)
            {
                if (root == null)
                {
                    root = new UIMsgNode
                }
            }
            else
            {
                if(count < size)
                {
                    // TODO
                }
            }
        }

       

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public void SubNode(UIMsgNode node)
        {
            if (node == null)
            {
                return;
            }
            // TODO
        }
    }
}