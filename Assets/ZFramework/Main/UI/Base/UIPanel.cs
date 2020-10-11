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
        /// ui数据存储
        /// </summary>
        protected IUIData uiData = null;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
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
            base.OnInit(uiData);
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