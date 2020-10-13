using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UI的基类
    /// </summary>
    public class UIPanel : UIPanelBehaviour
    {
        

        /// <summary>
        /// 设置UI数据,ui管理器里调用
        /// </summary>
        /// <param name="uiData"></param>
        protected void InitUIData(IUIData mUiData = null)
        {
            this.mUiData = mUiData;
        }

        /// <summary>
        /// 对应UIMgr里面的Open
        /// </summary>
        /// <param name="mUiData"></param>
        protected override void OnOpen(IUIData mUiData = null)
        {
            base.OnOpen(mUiData);
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        protected override void OnShow()
        {
            base.OnShow();
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="uiData"></param>
        protected override void OnInit(IUIData uiData)
        {
            base.OnInit(this.mUiData);
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        protected override void OnHide()
        {
            base.OnHide();
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
        }
    }
}