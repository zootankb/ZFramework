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
        /// 获取ui物体里面的属性名字及类型，此方法可用
        /// </summary>
        /// <param name="uiGo"></param>
        /// <returns></returns>
        public static Dictionary<UILevel, Dictionary<string, string>> GetFieldNameAndType(GameObject uiGo)
        {
            Dictionary<UILevel, Dictionary<string, string>> fields = new Dictionary<UILevel, Dictionary<string, string>>();
            fields.Add(UILevel.UI, new Dictionary<string, string>());
            fields.Add(UILevel.UIElement, new Dictionary<string, string>());
            Transform[] allGos = uiGo.GetComponentsInChildren<Transform>();
            List<UIBind> binds = allGos.Where(go => go.GetComponent<UIBind>() != null).Select(g => g.GetComponent<UIBind>()).ToList();
            foreach (var bind in binds)
            {
                if(bind.level == UILevel.UI)
                {
                    if (fields[UILevel.UI].ContainsKey(bind.uiName))
                    {
                        Debug.LogWarningFormat("已经有名字为 {0} 的属性了！", bind.uiName);
                    }
                    else
                    {
                        fields[UILevel.UI].Add(bind.uiName, bind.uiType.ToString());
                    }
                }
                else if (bind.level == UILevel.UIElement)
                {
                    if (fields[UILevel.UIElement].ContainsKey(bind.uiName))
                    {
                        Debug.LogWarningFormat("已经有名字为 {0} 的属性了！", bind.uiName);
                    }
                    else
                    {
                        // ElementPanel后缀在创建脚本时添加
                        fields[UILevel.UIElement].Add(bind.uiName, bind.uiName);
                    }
                }
            }
            return fields;
        }
    }
}