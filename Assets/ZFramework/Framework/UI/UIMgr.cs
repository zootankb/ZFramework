using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZFramework.ClassExt;
using ZFramework.Log;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZFramework.UI
{
    /// <summary>
    /// UI层级
    /// </summary>
    public enum UILevel
    {
        /// <summary>
        /// 背景层
        /// </summary>
        BG = 0,
        /// <summary>
        /// 背景动画层
        /// </summary>
        AnimationUnder,
        /// <summary>
        /// 常规UI层
        /// </summary>
        Common,
        /// <summary>
        /// 桌面动画层
        /// </summary>
        AnimationOn,
        /// <summary>
        /// 置顶层
        /// </summary>
        Pop,
        /// <summary>
        /// 固定层
        /// </summary>
        Const,
        /// <summary>
        /// 提示层
        /// </summary>
        Toast,
        /// <summary>
        /// UI最上层
        /// </summary>
        Forward,
    }

    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIMgr : Singleton.SingletonMonoDontDes<UIMgr>
    {
        #region Data
        /// <summary>
        /// ui的主物体
        /// </summary>
        private GameObject uiRoot = null;

        /// <summary>
        /// ui的事件监听系统
        /// </summary>
        private GameObject eventSystem = null;

        /// <summary>
        /// UI摄像机
        /// </summary>
        private Camera uicamera = null;

        /// <summary>
        /// UI的各个层
        /// </summary>
        private Dictionary<UILevel, GameObject> uiLevels = new Dictionary<UILevel, GameObject>();

        /// <summary>
        /// 存储的UI预制体
        /// </summary>
        private Dictionary<string, GameObject> uiPrefabs = new Dictionary<string, GameObject>();

        /// <summary>
        /// 存储所有打开的UI
        /// </summary>
        private Dictionary<string, GameObject> uiGos = new Dictionary<string, GameObject>();
        #endregion

        #region Static Data
        /// <summary>
        /// ui根节点
        /// </summary>
        private const string UI_ROOT_NAME = "UIRoot";

        /// <summary>
        /// ui事件系统
        /// </summary>
        private const string EVENT_SYSTEM = "EventSystem";

        /// <summary>
        /// UI摄像机
        /// </summary>
        private const string UI_CAMERA = "UICamera";
        #endregion

        #region Private Func
        /// <summary>
        /// 实例化并创建UI父节点
        /// </summary>
        private void InitData()
        {
            uiRoot = new GameObject(UI_ROOT_NAME,
                        typeof(RectTransform),
                        typeof(Canvas),
                        typeof(CanvasScaler),
                        typeof(GraphicRaycaster));
            Canvas canvas = uiRoot.GetComponent<Canvas>();
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            CanvasScaler scaler = uiRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1;
            scaler.referencePixelsPerUnit = 100;
            GraphicRaycaster reycaster = uiRoot.GetComponent<GraphicRaycaster>();
            reycaster.ignoreReversedGraphics = true;
            reycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            eventSystem = new GameObject(EVENT_SYSTEM, typeof(EventSystem), typeof(StandaloneInputModule));
            EventSystem es = eventSystem.GetComponent<EventSystem>();
            es.sendNavigationEvents = true;
            es.pixelDragThreshold = 10;

            uiRoot.transform.SetParent(transform);
            eventSystem.transform.SetParent(transform);

            Array levels = Enum.GetValues(typeof(UILevel));
            foreach (var level in levels)
            {
                GameObject go = CreateUILevelGo((UILevel)level);
                go.transform.SetParent(uiRoot.transform);
                uiLevels.Add((UILevel)level, go);
            }

            GameObject camGo = new GameObject(UI_CAMERA, typeof(Camera));
            camGo.transform.SetParent(transform);
            uicamera = camGo.GetComponent<Camera>();
            uicamera.clearFlags = CameraClearFlags.Skybox;
            uicamera.backgroundColor = new Color(67f / 255, 67f / 255, 159f / 255);
            uicamera.cullingMask = 1 << 5;  //只渲染UI层    
            uicamera.orthographic = true;
            uicamera.orthographicSize = 5;
            uicamera.nearClipPlane = 0;
            uicamera.pixelRect = new Rect(0, 0, Screen.width, Screen.height);
            uicamera.targetTexture = null;
            uicamera.farClipPlane = 1;
            uicamera.depth = 0;
            uicamera.useOcclusionCulling = true;
            uicamera.allowHDR = true;
            uicamera.allowMSAA = true;
            uicamera.allowDynamicResolution = false;
        }

        /// <summary>
        /// 创建UI的层级物体
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private GameObject CreateUILevelGo(UILevel level)
        {
            GameObject go = new GameObject(level.ToString(), typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            // left bottom
            rt.offsetMin = new Vector2(0, 0);
            // right top
            rt.offsetMax = new Vector2(Screen.width, Screen.height);
            return go;

        }

        /// <summary>
        /// 获取UI物体并返回UI逻辑代码，一个UI物体只能打开一次，再打开时就返回已经存在的UI物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIData"></param>
        /// <param name="level"></param>
        /// <param name="assetName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        private T OpenUI<T>(IUIData uIData = null, UILevel level = UILevel.Common, string assetName = null, string abName = null) where T : UIPanel
        {
            string prefabName = typeof(T).Name;
            T t = null;
            if (uiGos.ContainsKey(prefabName))
            {
                LogOperator.AddWarnningRecord(string.Format("UI名字为 {0} 的物体已经存在，请不要重复创建！", prefabName));
                return null;
            }
            GameObject prefab = null;
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = prefabName;
            }
            if (string.IsNullOrEmpty(abName))
            {
                abName = prefabName;
            }
            if (!uiPrefabs.ContainsKey(prefabName))
            {
                string uiPrefabPath = null;
                string persPath = null;
#if UNITY_EDITOR
                uiPrefabPath = string.Format("{0}/{1}/{2}/{3}.ab", Application.streamingAssetsPath, ConfigContent.configPath.AssetbundlePath, ConfigContent.CurrPlatform, abName);
                persPath = string.Format("{0}/{1}/{2}/{3}.ab", Application.persistentDataPath, ConfigContent.configPath.AssetbundlePath, ConfigContent.CurrPlatform, abName);
                // 直接加载Prefab
                bool useFromPrefab = EditorPrefs.GetBool(ConfigNameForPlayerPref.EDITOR_UI_USE_FROM_PREFAB, false);
                if (useFromPrefab)
                {
                    // 直接加载Prefab
                    uiPrefabPath = string.Format("Assets/{0}/{1}.prefab", ConfigContent.configPath.UIPrePath, abName);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(uiPrefabPath);
                }
                else
                {
                    if (File.Exists(persPath))
                    {
                        // 保证使用最新UI资源
                        uiPrefabPath = persPath;
                    }
                    Debug.Log(uiPrefabPath);
                    AssetBundle ab = AssetBundle.LoadFromFile(uiPrefabPath);
                    prefab = ab.LoadAsset<GameObject>(assetName);
                    ab.Unload(false);
                }
#elif UNITY_ANDROID
                // 先查看 Application.persistentDataPath 下面有没有ab包，有的话就先使用 Application.persistentDataPath 下面的，
                // 这样就保证使用最新更改过的原UI资源和打包进ab包里面的原最新UI脚本，如果是新添加的资源就不行了，就需要更新主程序
                uiPrefabPath = string.Format("jar:file://{0}!/assets/{1}/{2}.ab", Application.dataPath, ConfigContent.CurrPlatform, abName);
                persPath = string.Format("jar:file://{0}/{1}/{2}.ab", Application.persistentDataPath, ConfigContent.CurrPlatform, abName);
                if (File.Exists(persPath))
                {
                    // 保证使用最新UI资源
                    uiPrefabPath = persPath;
                }
                AssetBundle ab = AssetBundle.LoadFromFile(uiPrefabPath);
                prefab = ab.LoadAsset<GameObject>(assetName);
                ab.Unload(false);
#elif UNITY_IPHONE
                uiPrefabPath = string.Format("file://{0}/Raw/{1}/{2}.ab", Application.dataPath, ConfigContent.CurrPlatform, abName);
                persPath = string.Format("file://{0}/{1}/{2}.ab", Application.persistentDataPath, ConfigContent.CurrPlatform, abName);
                if (File.Exists(persPath))
                {
                    // 保证使用最新UI资源
                    uiPrefabPath = persPath;
                }
                AssetBundle ab = AssetBundle.LoadFromFile(uiPrefabPath);
                prefab = ab.LoadAsset<GameObject>(assetName);
                ab.Unload(false);
#endif
                uiPrefabs.Add(prefabName, prefab);

                Debug.Log(">>>Open UI: " + uiPrefabPath);
            }
            GameObject go = Instantiate(prefab, GetLevelTransform(level));
            uiGos.Add(prefabName, go);
            go.name = abName;
            t = go.GetComponent<T>();
            t.OnOpen(uIData);
            return t;
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void CloseUI<T>() where T : UIPanel
        {
            string uiName = typeof(T).Name;
            if (uiGos.ContainsKey(uiName))
            {
                GameObject go = uiGos[uiName];
                uiGos.Remove(uiName);
                Destroy(go);
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private T ＧetUI<T>() where T : UIPanel
        {
            T t = null;
            string uiName = typeof(T).Name;
            if (uiGos.ContainsKey(uiName))
            {
                t = uiGos[uiName].GetComponent<T>();
            }
            return t;
        }

        /// <summary>
        /// 关闭所有的UI
        /// </summary>
        private void CloseAllUI()
        {
            foreach (var kvp in uiGos)
            {
                Destroy(kvp.Value);
            }
            uiGos.Clear();
        }

        /// <summary>
        /// 获取对应级别物体的transform
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private Transform GetLevelTransform(UILevel level = UILevel.Common)
        {
            return uiLevels[level].transform;
        }

        /// <summary>
        /// 释放指定UI的镜像
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void ReleaseUIPrefab<T>() where T : UIPanel
        {
            string uiPrefabName = typeof(T).Name;
            if (uiPrefabs.ContainsKey(uiPrefabName))
            {
                GameObject prefab = uiPrefabs[uiPrefabName];
                uiPrefabs.Remove(uiPrefabName);
                DestroyImmediate(prefab);
            }
        }

        /// <summary>
        /// 释放所有的UI资源
        /// </summary>
        private void ReleaseAllUIPrefab()
        {
            foreach (var kvp in uiPrefabs)
            {
                DestroyImmediate(kvp.Value);
            }
            uiPrefabs.Clear();
        }

        /// <summary>
        /// 设置屏幕的分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void SetCameraResolution(int width, int height)
        {
            if(uiRoot != null)
            {
                CanvasScaler cans = uiRoot.GetComponent<CanvasScaler>();
                cans.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cans.referenceResolution = new Vector2(width, height);
            }
        }
        #endregion
        
        #region IEnum
        
        #endregion
        
        #region API
        /// <summary>
        /// 实例化并创建UI父节点
        /// </summary>
        public static void Init()
        {
            Instance.InitData();
            // 开启多线工具Loom的运行
            Log.LogOperator.AddLogRecord("框架工具初始化以及开启多线工具Loom");
        }

        /// <summary>
        /// 打开UI界面，并返回UI脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIData"></param>
        /// <param name="level"></param>
        /// <param name="assetName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static T Open<T>(IUIData uIData = null, UILevel level = UILevel.Common, string assetName = null, string abName = null) where T : UIPanel
        {
            return Instance.OpenUI<T>(uIData, level, assetName, abName);
        }

        /// <summary>
        /// 关闭UI界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Close<T>() where T : UIPanel
        {
            Instance.CloseUI<T>();
        }

        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : UIPanel
        {
            return Instance.ＧetUI<T>();
        }

        /// <summary>
        /// 关闭UI界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CloseAll()
        {
            Instance.CloseAllUI();
        }

        /// <summary>
        /// 释放指定UI的镜像
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ReleaseUI<T>() where T : UIPanel
        {
            Instance.ReleaseUIPrefab<T>();
        }

        /// <summary>
        /// 释放所有的UI资源
        /// </summary>
        public static void ReleaseAllUI()
        {
            Instance.ReleaseAllUIPrefab();
        }

        /// <summary>
        /// 设置屏幕的分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetResolution(int width,int height)
        {
            Instance.SetCameraResolution(width, height);
        }
        #endregion

        #region Private Class
        /// <summary>
        /// ui加载状态
        /// </summary>
        private enum UIState
        {
            /// <summary>
            /// 未被加载
            /// </summary>
            Unload,
            /// <summary>
            /// 正在加载
            /// </summary>
            Loading,
            /// <summary>
            /// 已经加载好
            /// </summary>
            Loaded,
        }

        /// <summary>
        /// UI资源信息
        /// </summary>
        private class UIAsset
        {
            /// <summary>
            /// 加载状态
            /// </summary>
            public UIState state = UIState.Unload;

            /// <summary>
            /// ui的名字
            /// </summary>
            public string uiName = null;

            /// <summary>
            /// ui路径，不同模式下uiPath会有不同的值
            /// </summary>
            public string uiPath = null;

            /// <summary>
            /// 加载好的ui物体
            /// </summary>
            public GameObject uiGo = null;
        }
#endregion
    }
}