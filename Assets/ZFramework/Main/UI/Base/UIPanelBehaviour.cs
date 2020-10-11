using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UIPanel与monobahaviour的中间层
    /// </summary>
    public class UIPanelBehaviour : MonoBehaviour
    {
        #region Mono
        private void Awake()
        {
            OnOpen();
        }

        private void OnEnable()
        {
            OnShow();
        }

        private void Start()
        {
            OnInit(null);
        }

        private void OnDisable()
        {
            OnHide();
        }

        private void OnDestroy()
        {
            OnBeforeDestroy();
        }
        #endregion

        #region Virtual
        /// <summary>
        /// 打开，对应Awake
        /// </summary>
        protected virtual void OnOpen()
        {
            // Pass
        }

        /// <summary>
        /// 展示，对应OnEnable
        /// </summary>
        protected virtual void OnShow()
        {
            // Pass
        }

        /// <summary>
        /// 初始化，对应Start
        /// </summary>
        /// <param name="uiData"></param>
        protected virtual void OnInit(IUIData uiData)
        {
            // Pass
        }

        /// <summary>
        /// 隐藏，对应OnDisable
        /// </summary>
        protected virtual void OnHide()
        {
            // Pass
        }

        /// <summary>
        /// 销毁，对应OnDestroy
        /// </summary>
        protected virtual void OnBeforeDestroy()
        {
            // Pass
        }

        /// <summary>
        /// 消息接收器
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        protected virtual void ProcessMsg(int eventId, ZMsg msg)
        {
            // Pass
        }

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        protected virtual void SendMsg(int eventId, ZMsg msg)
        {
            // Pass
        }

        #endregion
    }
}