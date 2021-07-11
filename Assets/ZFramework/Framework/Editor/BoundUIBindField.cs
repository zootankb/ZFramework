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
        public static void Bound(GameObject go)
        {
            // 创建好脚本之后就要给物体添加好创建的脚本，并把各种属性自动赋值到创建好的脚本属性中
            // 寻找目标脚本类型
            Type targetT = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly assem = null;
            // 此UI中需要的脚本，包括主UI脚本和子UI脚本
            Dictionary<string, Type> uiScriptTypes = new Dictionary<string, Type>();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp,"))
                {
                    assem = a;
                    Type[] ts = a.GetTypes();
                    foreach (var t in ts)
                    {
                        if(t.BaseType == typeof(UIPanel) || t.BaseType == typeof(UIElement))
                        {
                            uiScriptTypes.Add(t.Name, t);
                        }
                    }
                    break;
                }
            }
            if (uiScriptTypes.ContainsKey(go.name))
            {
                targetT = uiScriptTypes[go.name];
            }
            if (targetT != null)
            {
                //AssetDatabase.StartAssetEditing();
                GameObject newPrefab = PrefabUtility.InstantiatePrefab(go) as GameObject;
               // GameObject newPrefab = go;
                Dictionary<string, UIBind> fields = UI.UIBind.GetFieldNameAndType(newPrefab);
                try
                {
                    // 添加主UI脚本
                    Component mainUICom = newPrefab.GetComponent(targetT);
                    if (mainUICom == null)
                    {
                        mainUICom = newPrefab.AddComponent(targetT);
                    }
                    // 把所有的脚本都挂上，不包括包括主UI，因为fields里面没有主UI的信息
                    foreach (var childUI in uiScriptTypes)
                    {
                        if (fields.ContainsKey(childUI.Key))
                        {
                            Component t = fields[childUI.Key].GetComponent(childUI.Value);
                            if (t == null)
                            {
                                fields[childUI.Key].gameObject.AddComponent(childUI.Value);
                            }
                        }
                    }
                    
                    // 主UI脚本里面的属性
                    Dictionary<string, FieldInfo> fs = targetT.GetFields().ToDictionary(p => p.Name, p => p);
                    // 主UI脚本里面的属性实体
                    List<UIBind> binds = fields.Values.Where(p => p.parentBind == null && fs.ContainsKey(p.uiName)).ToList();
                    foreach (var bind in binds)
                    {
                        UnityEngine.Object tarCom = null;
                        if (bind.level == UIBind.UILevel.UI)
                        {
                            switch (bind.uiType)
                            {
                                case UIBind.UIType.Button:
                                    tarCom = bind.GetComponent<Button>();
                                    break;
                                case UIBind.UIType.Canvas:
                                    tarCom = bind.GetComponent<Canvas>();
                                    break;
                                case UIBind.UIType.Dropdown:
                                    tarCom = bind.GetComponent<Dropdown>();
                                    break;
                                case UIBind.UIType.RectTransform:
                                    tarCom = bind.GetComponent<RectTransform>();
                                    break;
                                case UIBind.UIType.Image:
                                    tarCom = bind.GetComponent<Image>();
                                    break;
                                case UIBind.UIType.InputField:
                                    tarCom = bind.GetComponent<InputField>();
                                    break;
                                case UIBind.UIType.RawImage:
                                    tarCom = bind.GetComponent<RawImage>();
                                    break;
                                case UIBind.UIType.Scrollbar:
                                    tarCom = bind.GetComponent<Scrollbar>();
                                    break;
                                case UIBind.UIType.ScrollView:
                                    tarCom = bind.GetComponent<ScrollRect>();
                                    break;
                                case UIBind.UIType.Slider:
                                    tarCom = bind.GetComponent<Slider>();
                                    break;
                                case UIBind.UIType.Text:
                                    tarCom = bind.GetComponent<Text>();
                                    break;
                                case UIBind.UIType.Toggle:
                                    tarCom = bind.GetComponent<Toggle>();
                                    break;
                                case UIBind.UIType.Transform:
                                    tarCom = bind.transform;
                                    break;
                                case UIBind.UIType.GameObject:
                                    tarCom = bind.gameObject;
                                    break;
                                default:
                                    tarCom = bind.gameObject;
                                    break;
                            }
                        }
                        else if (bind.level == UIBind.UILevel.UIElement)
                        {
                            tarCom = bind.GetComponent(uiScriptTypes[bind.uiName]);
                        }
                        fs[bind.uiName].SetValue(mainUICom, tarCom);
                    }
                    // 各种子UI的脚本属性辅助
                    List<UIBind> eleUIBinds = fields.Values.Where(p => p.level == UIBind.UILevel.UIElement).ToList();
                    foreach (var eleUIBind in eleUIBinds)
                    {
                        Type eleType = uiScriptTypes[eleUIBind.uiName];
                        Component eleUICom = eleUIBind.GetComponent(eleType);
                        Dictionary<string, FieldInfo> eleFields = eleType.GetFields().ToDictionary(p => p.Name, p => p);
                        List<UIBind> eleChildBinds = fields.Values.Where(p => p.parentBind == eleUIBind.transform).ToList();
                        foreach (var eleChildBind in eleChildBinds)
                        {
                            UnityEngine.Object tarCom = null;
                            if (eleChildBind.level == UIBind.UILevel.UI)
                            {
                                switch (eleChildBind.uiType)
                                {
                                    case UIBind.UIType.Button:
                                        tarCom = eleChildBind.GetComponent<Button>();
                                        break;
                                    case UIBind.UIType.Canvas:
                                        tarCom = eleChildBind.GetComponent<Canvas>();
                                        break;
                                    case UIBind.UIType.Dropdown:
                                        tarCom = eleChildBind.GetComponent<Dropdown>();
                                        break;
                                    case UIBind.UIType.RectTransform:
                                        tarCom = eleChildBind.GetComponent<RectTransform>();
                                        break;
                                    case UIBind.UIType.Image:
                                        tarCom = eleChildBind.GetComponent<Image>();
                                        break;
                                    case UIBind.UIType.InputField:
                                        tarCom = eleChildBind.GetComponent<InputField>();
                                        break;
                                    case UIBind.UIType.RawImage:
                                        tarCom = eleChildBind.GetComponent<RawImage>();
                                        break;
                                    case UIBind.UIType.Scrollbar:
                                        tarCom = eleChildBind.GetComponent<Scrollbar>();
                                        break;
                                    case UIBind.UIType.ScrollView:
                                        tarCom = eleChildBind.GetComponent<ScrollRect>();
                                        break;
                                    case UIBind.UIType.Slider:
                                        tarCom = eleChildBind.GetComponent<Slider>();
                                        break;
                                    case UIBind.UIType.Text:
                                        tarCom = eleChildBind.GetComponent<Text>();
                                        break;
                                    case UIBind.UIType.Toggle:
                                        tarCom = eleChildBind.GetComponent<Toggle>();
                                        break;
                                    case UIBind.UIType.Transform:
                                        tarCom = eleChildBind.transform;
                                        break;
                                    case UIBind.UIType.GameObject:
                                        tarCom = eleChildBind.gameObject;
                                        break;
                                    default:
                                        tarCom = eleChildBind.gameObject;
                                        break;
                                }
                            }
                            else if (eleChildBind.level == UIBind.UILevel.UIElement)
                            {
                                tarCom = eleChildBind.GetComponent(uiScriptTypes[eleChildBind.uiName]);
                            }
                            eleFields[eleChildBind.uiName].SetValue(eleUICom, tarCom);
                        }
                        Debug.LogFormat("--------Succeed Create {0}'s {1} script and bind ui component!!--------", go.name, eleUIBind.uiName);
                    }
                    string prefabPath = AssetDatabase.GetAssetPath(go);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, prefabPath, InteractionMode.UserAction);
                    MonoBehaviour.DestroyImmediate(newPrefab);
                    Debug.LogFormat("--------Succeed Create {0}'s all scripts and bind ui component!!--------", go.name);
                }
                catch(Exception e)
                {
                    Debug.LogWarningFormat("在自动给脚本属性赋值的时候报错，原因：{0}", e.Message);
                }
                finally
                {
                    //UnityEngine.Object.DestroyImmediate(newPrefab);
                    //AssetDatabase.StopAssetEditing();
                }
            }
        }
            
    }
}