﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UIPanel的子UI组件，此组件里面没有自动生成的属性，需要手动创建
    /// </summary>
    public class UIElement : MonoBehaviour
    {
        private void Start()
        {
            OnInit();
        }

        private void OnDestroy()
        {
            OnBeforeDestroy();
        }

        protected virtual void OnInit()
        {
            // Pass
        }

        protected virtual void OnBeforeDestroy()
        {
            // Pass
        }

        /// <summary>
        /// 获取组件名字
        /// </summary>
        protected virtual string ComponentName
        {
            get { return this.GetType().Name; } 
        }
    }
}