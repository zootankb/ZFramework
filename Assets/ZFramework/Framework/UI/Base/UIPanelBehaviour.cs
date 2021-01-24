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
        /// <summary>
        /// UI内部间全局通用消息传输中枢
        /// </summary>
        private static UIMsgCenter uiMsgCenter = UIMsgCenter.Allocate(typeof(UIPanelBehaviour).Name);

        /// <summary>
        /// 初始ui事件id
        /// </summary>
        private int mUIEventId = int.MinValue;

        /// <summary>
        /// UI的专属事件ID，为gameObject.GetHashCode(),全部为负值
        /// </summary>
        protected int uiEventId
        {
            get
            {
                if(mUIEventId == int.MinValue)
                {
                    mUIEventId = gameObject.GetHashCode();
                }
                return mUIEventId;
            }
        }


        /// <summary>
        /// ui数据存储
        /// </summary>
        protected IUIData mUiData = null;

        #region Mono
        private void Awake()
        {
            uiMsgCenter.Register(uiEventId, ProcessMsg);
        }

        private void OnEnable()
        {
            OnShow();
        }

        private void Start()
        {
            OnInit(mUiData);
        }

        private void OnDisable()
        {
            OnHide();
        }

        private void OnDestroy()
        {
            OnBeforeDestroy();
            uiMsgCenter.Unregister(uiEventId, ProcessMsg);
        }
        #endregion

        #region Virtual
        /// <summary>
        /// 打开，对应UIMgr里面的open
        /// </summary>
        /// <param name="mUiData"></param>
        public virtual void OnOpen(IUIData mUiData = null)
        {
            this.mUiData = mUiData;
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