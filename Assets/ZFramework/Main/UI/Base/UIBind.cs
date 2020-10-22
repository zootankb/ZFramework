using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            GameObject,
            Transform,
            RectTransform,
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

        /// <summary>
        /// 父级Bind脚本
        /// </summary>
        [HideInInspector]
        public Transform parentBind = null;

        /// <summary>
        /// 获取ui物体里面的属性名字及类型，此方法可用，若正常获取，返回不为空的数据，若执行异常，就返回Null
        /// </summary>
        /// <param name="uiGo"></param>
        /// <returns></returns>
        public static Dictionary<string, UIBind> GetFieldNameAndType(GameObject uiGo)
        {
            Dictionary<string, UIBind> fields = new Dictionary<string, UIBind>();
            Transform[] allGos = uiGo.GetComponentsInChildren<Transform>();
            List<UIBind> binds = allGos.Where(go => go.GetComponent<UIBind>() != null).Select(g => g.GetComponent<UIBind>()).ToList();
            foreach (var bind in binds)
            {
                if (fields.ContainsKey(bind.uiName))
                {
                    Debug.LogFormat("------已经存在Key值为 {0} 的UI，所标记的UI名字不能相同！-------", bind.name);
                    return null;
                }
                fields.Add(bind.uiName, bind);
            }
            return fields;
        }
    }
}