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
        /// 对应UIMgr里面的Open
        /// </summary>
        /// <param name="mUiData"></param>
        public override void OnOpen(IUIData mUiData = null)
        {
            this.mUiData = mUiData;
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        protected override void OnShow()
        {
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="uiData"></param>
        protected override void OnInit(IUIData uiData)
        {
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        protected override void OnHide()
        {
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected override void OnBeforeDestroy()
        {
        }
    }
}