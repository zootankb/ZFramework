using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZFramework.UI;

namespace ZFramework.ZEditor
{
    /// <summary>
    /// 把UIPanel的指定脚本和生成脚本里面的属性绑定到物体上
    /// </summary>
    public static class BoundUIBindField
    {
        /// <summary>
        /// 把UIPanel的指定脚本和生成脚本里面的属性绑定到物体上
        /// </summary>
        /// <param name="go"></param>
        /// <param name="fields">属性信息</param>
        public static void Bound(GameObject go, Dictionary<string, UIBind> fields)
        {
            // 创建好脚本之后就要给物体添加好创建的脚本，并把各种属性自动赋值到创建好的脚本属性中
            // 寻找目标脚本类型
            Type targetT = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp,"))
                {
                    Type[] ts = a.GetTypes();
                    foreach (var t in ts)
                    {
                        if (t.Name.Equals(go.name))
                        {
                            targetT = t;
                            break;
                        }
                    }
                    break;
                }
            }
            if (targetT != null)
            {
                GameObject newPrefab = PrefabUtility.InstantiatePrefab(go) as GameObject;
                try
                {
                    Dictionary<string, FieldInfo> fs = targetT.GetFields().ToDictionary(p => p.Name, p => p);
                    Component com = newPrefab.GetComponent(targetT);
                    if (com == null)
                    {
                        com = newPrefab.AddComponent(targetT);
                    }
                    Dictionary<string, UIBind> childs = newPrefab.GetComponentsInChildren<UIBind>().ToDictionary(p => p.uiName, p => p);
                    foreach (var kvp in fields[UI.UIBind.UILevel.UI])
                    {
                        if (childs.ContainsKey(kvp.Key) && fs.ContainsKey(kvp.Key))
                        {
                            UnityEngine.Object tarCom = null;
                            switch (childs[kvp.Key].uiType)
                            {
                                case UIBind.UIType.Button:
                                    tarCom = childs[kvp.Key].GetComponent<Button>();
                                    break;
                                case UIBind.UIType.Canvas:
                                    tarCom = childs[kvp.Key].GetComponent<Canvas>();
                                    break;
                                case UIBind.UIType.Dropdown:
                                    tarCom = childs[kvp.Key].GetComponent<Dropdown>();
                                    break;
                                case UIBind.UIType.RectTransform:
                                    tarCom = childs[kvp.Key].gameObject;
                                    break;
                                case UIBind.UIType.Image:
                                    tarCom = childs[kvp.Key].GetComponent<Image>();
                                    break;
                                case UIBind.UIType.InputField:
                                    tarCom = childs[kvp.Key].GetComponent<InputField>();
                                    break;
                                case UIBind.UIType.RawImage:
                                    tarCom = childs[kvp.Key].GetComponent<RawImage>();
                                    break;
                                case UIBind.UIType.Scrollbar:
                                    tarCom = childs[kvp.Key].GetComponent<Scrollbar>();
                                    break;
                                case UIBind.UIType.ScrollView:
                                    tarCom = childs[kvp.Key].GetComponent<ScrollRect>();
                                    break;
                                case UIBind.UIType.Slider:
                                    tarCom = childs[kvp.Key].GetComponent<Slider>();
                                    break;
                                case UIBind.UIType.Text:
                                    tarCom = childs[kvp.Key].GetComponent<Text>();
                                    break;
                                case UIBind.UIType.Toggle:
                                    tarCom = childs[kvp.Key].GetComponent<Toggle>();
                                    break;
                            }
                            fs[kvp.Key].SetValue(com, tarCom);
                        }
                    }
                    foreach (var kvp in fields[UI.UIBind.UILevel.UIElement])
                    {
                        if (childs.ContainsKey(kvp.Key))
                        {
                            // 转换自定义组件
                            UnityEngine.Object tarCom = childs[kvp.Key].GetComponent(fs[kvp.Key].FieldType);
                            fs[kvp.Key].SetValue(com, tarCom);
                        }
                    }
                    string prefabPath = AssetDatabase.GetAssetPath(go);
                    bool isSuccess = false;
                    PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
                }
                catch(Exception e)
                {
                    Debug.LogWarningFormat("在自动给脚本属性赋值的时候报错，原因：{0}", e.Message);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(newPrefab);
                }
            }
        }
            
    }
}