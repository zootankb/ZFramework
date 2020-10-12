using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UI界面中的属性绑定类
    /// </summary>
    public class UIBind : MonoBehaviour
    {
        /// <summary>
        /// 绑定的UI类型选择
        /// </summary>
        public enum UIType
        {
            /// <summary>
            /// 空物体
            /// </summary>
            None,
            Text,
            Image,
            RawImage,
            Button,
            Toggle,
            Slider,
            Scrollbar,
            Dropdown,
            InputField,
            Canvas,
            ScrollView
        }

        /// <summary>
        /// UI级别
        /// </summary>
        public enum UILevel
        {
            /// <summary>
            /// 常规UI
            /// </summary>
            UI,
            /// <summary>
            /// 子级UI
            /// </summary>
            UIElement,
        }

        /// <summary>
        /// UI组件的名字
        /// </summary>
        [HideInInspector]
        public string uiName = null;

        /// <summary>
        /// UI级别
        /// </summary>
        [HideInInspector]
        public UILevel level = UILevel.UI;

        /// <summary>
        /// UI类型
        /// </summary>
        [HideInInspector]
        public UIType uiType = UIType.Text;

        /// <summary>
        /// 组件属性解释
        /// </summary>
        [HideInInspector]
        public string explain = string.Empty;

    }
}