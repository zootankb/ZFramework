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
        /// <param name="size">0为无限个，默认能容纳1000个事件id的大小</param>
        public UIMsgNodeLinked(uint size = 1000)
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
            AddNode(new UIMsgNode(new UIMsg(eventId, msg)));
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="msg"></param>
        public void AddNode(UIMsg msg)
        {
            AddNode(new UIMsgNode(msg));
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
            //  不固定容量
            if(size == 0)
            {
                if (root == null)
                {
                    lastNode = root = node;
                    ++count;
                }
                else
                {
                    UIMsgNode currNode = root;
                    while (currNode != null &&!currNode.value.HasEventId(node.value.EventId))
                    {
                        currNode = currNode.nextNode;
                    }
                    // 没有相同的id事件
                    if(currNode == null)
                    {
                        lastNode.nextNode = node;
                        lastNode = node;
                        ++count;
                    }
                    // 有相同的id事件
                    else
                    {
                        currNode.value.Register(node.value.EventId, node.value.Ets);
                    }
                }
            }
            // 固定容量
            else
            {
                if(count < size)
                {
                    if (root == null)
                    {
                        lastNode = root = node;
                        ++count;
                    }
                    else
                    {
                        UIMsgNode currNode = root;
                        while (currNode != null && !currNode.value.HasEventId(node.value.EventId))
                        {
                            currNode = currNode.nextNode;
                        }
                        // 没有相同的id事件
                        if (currNode == null)
                        {
                            lastNode.nextNode = node;
                            lastNode = node;
                            ++count;
                        }
                        // 有相同的id事件
                        else
                        {
                            currNode.value.Register(node.value.EventId, node.value.Ets);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除事件，但不移除节点
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void SubNode(int eventId, Action<int, ZMsg> msg)
        {
            SubNode(new UIMsgNode(new UIMsg(eventId, msg)));
        }

        /// <summary>
        /// 移除事件，但不移除节点
        /// </summary>
        /// <param name="msg"></param>
        public void SubNode(UIMsg msg)
        {
            SubNode(new UIMsgNode(msg));
        }

        /// <summary>
        /// 移除事件，但不移除节点
        /// </summary>
        /// <param name="node"></param>
        public void SubNode(UIMsgNode node)
        {
            if (node == null)
            {
                return;
            }
            UIMsgNode currNode = root;
            while (currNode != null && !currNode.value.HasEventId(node.value.EventId))
            {
                currNode = currNode.nextNode;
            }
            // 相同的id事件
            if (currNode != null)
            {
                currNode.value.Unregister(node.value.EventId, node.value.Ets);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        public void SendMsg(uint eventId , ZMsg msg)
        {
            UIMsgNode currNode = root;
            while(currNode != null)
            {
                currNode.value.SendMsg((int)eventId, msg);
                currNode = currNode.nextNode;
            }
        }
    }
}