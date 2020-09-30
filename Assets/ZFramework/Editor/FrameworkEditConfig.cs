using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using NUnit.Framework.Constraints;
using ZFramework.Log;
using UnityEditor.Graphs;

namespace ZFramework.Editor
{
    /// <summary>
    /// 框架配置操作窗口
    /// </summary>
    public class FrameworkEditConfig : EditorWindow
    {
        #region Data 框架配置操作
        /// <summary>
        /// Unity操作栏的名字
        /// </summary>
        private const string OPTION_NAME = "框架配置操作";

        /// <summary>
        /// 配置文件的内容
        /// </summary>
        private ConfigContent config = null;

        /// <summary>
        /// 操作页
        /// </summary>
        private string[] optionPanel = new string[] { "框架配置", "数据库操作", "AB包管理", "测试" };

        /// <summary>
        /// 当前选择的操作页
        /// </summary>
        private int selectedOption = 0;

        /// <summary>
        /// uiscrollview的滑动值
        /// </summary>
        private Vector2 uiScrollviewPos = Vector2.zero;

        /// <summary>
        /// ui预制体物体的文件信息
        /// </summary>
        private List<AssetFileInfo> uiPrefabInfos = new List<AssetFileInfo>();

        /// <summary>
        /// 关于服务器API、assetbundle.manifest文件url、资源更新url的配置
        /// </summary>
        private NetUrlConfig netUrlConfig = null;
        #endregion

        #region Data 数据库操作
        /// <summary>
        /// 直接在application.persientdata数据库文件的路径
        /// </summary>
        private List<AssetFileInfo> dbPaths = new List<AssetFileInfo>();

        /// <summary>
        /// 当前选择的数据库的索引
        /// </summary>
        private int currSelectDb = 0;

        /// <summary>
        /// 上次选择的数据库的索引
        /// </summary>
        private int oriSelectDb = -1;

        /// <summary>
        /// 选择的数据库的所有表
        /// </summary>
        private List<string> tablesInSelectedDb = new List<string>();

        /// <summary>
        /// 当前选择的表的索引
        /// </summary>
        private int currSelectTableIndex = 0;

        /// <summary>
        /// 上次选择的表的索引
        /// </summary>
        private int oriSelectTableIndex = -1;

        /// <summary>
        /// 当前选择的表的记录
        /// </summary>
        private DataTable currSelectTable = null;

        /// <summary>
        /// 对应上面的currSelectTable数据，把选择的操作放到这里面
        /// </summary>
        private List<bool> selectedRecord = null;

        /// <summary>
        /// 表列索引和显示的宽度
        /// </summary>
        private Dictionary<int, float> columnIndexWidth = new Dictionary<int, float>();

        /// <summary>
        /// 数字列的宽度
        /// </summary>
        private float numberColumnWidth = 75f;

        /// <summary>
        /// 字符列的宽度
        /// </summary>
        private float textColumnWidth = 150f;

        /// <summary>
        /// 添加记录标记
        /// </summary>
        private bool stateAddRecord = false;

        /// <summary>
        /// 更新记录标记
        /// </summary>
        private bool stateUpdateRecord = false;

        /// <summary>
        /// 添加记录时用的数据存储
        /// </summary>
        private List<object> addValueList = new List<object>();

        /// <summary>
        /// 更新记录时用的数据存储
        /// </summary>
        private List<object> updateValueList = new List<object>();
        #endregion

        #region MenuItem的操作
        [MenuItem(OPTION_NAME +"/重置配置")]
        public static void ResetFrameworkConfig()
        {
            Debug.Log("重置操作");
        }

        [MenuItem(OPTION_NAME + "/打开操作面板")]
        public static void Init()
        {
            FrameworkEditConfig window = EditorWindow.GetWindow<FrameworkEditConfig>(false, "ZFramework框架配置", true);
            window.Show();
        }
        #endregion

        #region 窗体事件函数
        /// <summary>
        /// 在新窗口打开时调用
        /// </summary>
        private void OnEnable()
        {
            #region 初始化 框架配置操作
            // 1.寻找配置文件
            if (File.Exists(ConfigContent.CONFIG_FILE_PATH))
            {
                string json = ConfigContent.CONFIG_FILE_PATH.GetTextAssetContentStr();
                config = json.ToNewtonObjectT<ConfigContent>();
            }
            else
            {
                config = new ConfigContent();
            }
            // 2.根据配置文件寻找ui预制体
            InitUIInfos();
            // 3.根据配置文件找服务器接口和资源更新路径
            if (File.Exists(NetUrlConfig.NetUrlFileName))
            {
                string json = NetUrlConfig.NetUrlFileName.GetTextAssetContentStr();
                netUrlConfig = json.ToNewtonObjectT<NetUrlConfig>();
            }
            else
            {
                netUrlConfig = new NetUrlConfig();
            }
            #endregion

            #region 初始化 数据库操作
            string[] dbfiles = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
            foreach (var dbfile in dbfiles)
            {
                dbPaths.Add(new AssetFileInfo() { filename = Path.GetFileName(dbfile), fileAbsPath = dbfile, selected = false, fileRelaPath = dbfile.Replace(Application.persistentDataPath, "") });
            }
            #endregion
        }

        /// <summary>
        /// 在此处实现您自己的 Editor GUI
        /// </summary>
        private void OnGUI()
        {
            selectedOption = GUILayout.Toolbar(selectedOption, optionPanel, EditorStyles.toolbarButton);

            // 框架配置
            if (selectedOption == 0)
            {
                #region 路径及文件配置
                EditorGUILayout.HelpBox("这是一个适用于轻量级Unity应用和游戏的框架，内部还有很多需要改进之处，如有Bug，还请多多包涵，反馈开发人员进行修改！！！", MessageType.Info, true);
                EditorGUILayout.LabelField("框架文件夹配置：", EditorStyles.boldLabel);
                config.FrameworkNamespace = EditorGUILayout.TextField("代码的默认命名空间：", config.FrameworkNamespace);

                EditorGUILayout.BeginHorizontal();
                config.UIPrePath = EditorGUILayout.TextField("UI预制体存放路径：", config.UIPrePath);
                if (GUILayout.Button("创建", GUILayout.MaxWidth(50)))
                {
                    string.Format("{0}/{1}", Application.dataPath, config.UIPrePath).CheckOrCreateDir();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("删除", GUILayout.MaxWidth(50)))
                {
                    if (EditorUtility.DisplayDialog("警告", "此操作会强制删除文件夹及文件夹下的所有内容", "确定", "取消"))
                    {
                        string.Format("{0}/{1}", Application.dataPath, config.UIPrePath).CheckForDeleteDir();
                        File.Delete(string.Format("{0}/{1}.meta", Application.dataPath, config.UIPrePath));
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                config.UIScriptPath = EditorGUILayout.TextField("UI代码存放路径：", config.UIScriptPath);
                if (GUILayout.Button("创建", GUILayout.MaxWidth(50)))
                {
                    string.Format("{0}/{1}", Application.dataPath, config.UIScriptPath).CheckOrCreateDir();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("删除", GUILayout.MaxWidth(50)))
                {
                    if (EditorUtility.DisplayDialog("警告", "此操作会强制删除文件夹及文件夹下的所有内容", "确定", "取消"))
                    {
                        string.Format("{0}/{1}", Application.dataPath, config.UIScriptPath).CheckForDeleteDir();
                        File.Delete(string.Format("{0}/{1}.meta", Application.dataPath, config.UIScriptPath));
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                config.AssetbundlePath = EditorGUILayout.TextField("AB包存放路径：", config.AssetbundlePath);
                if (GUILayout.Button("创建", GUILayout.MaxWidth(50)))
                {
                    string.Format("{0}/{1}", Application.dataPath, config.AssetbundlePath).CheckOrCreateDir();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("删除", GUILayout.MaxWidth(50)) && EditorUtility.DisplayDialog("警告", "此操作会强制删除文件夹及文件夹下的所有内容", "确定", "取消"))
                {
                    if (EditorUtility.DisplayDialog("警告", "此操作会强制删除文件夹及文件夹下的所有内容", "确定", "取消"))
                    {
                        string.Format("{0}/{1}", Application.dataPath, config.AssetbundlePath).CheckForDeleteDir();
                        File.Delete(string.Format("{0}/{1}.meta", Application.dataPath, config.AssetbundlePath));
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("创建配置文件需要的文件夹"))
                {
                    string.Format("{0}/{1}", Application.dataPath, config.UIPrePath).CheckOrCreateDir();
                    string.Format("{0}/{1}", Application.dataPath, config.UIScriptPath).CheckOrCreateDir();
                    string.Format("{0}/{1}", Application.dataPath, config.AssetbundlePath).CheckOrCreateDir();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("写入配置文件"))
                {
                    ConfigContent.CONFIG_FILE_PATH.WriteTextAssetContentStr(config.ToNewtonJson());
                    AssetDatabase.Refresh();
                    if (EditorUtility.DisplayDialog("提示：", "若UI预制体文件夹路径改变，是否刷新UI预制体文件夹下的文件信息", "确定刷新", "取消"))
                    {
                        InitUIInfos();
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("服务器地址和资源URL的配置：", EditorStyles.boldLabel);
                netUrlConfig.APIHost = EditorGUILayout.TextField("服务器地址：", netUrlConfig.APIHost);
                netUrlConfig.ManifestHost = EditorGUILayout.TextField("Assetbundle.ManifestHost地址：", netUrlConfig.ManifestHost);
                netUrlConfig.ResHost = EditorGUILayout.TextField("服务器资源池的URL地址：", netUrlConfig.ResHost);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("创建StreamingAssets文件夹 / 写入网络配置文件"))
                {
                    NetUrlConfig.NetUrlFileName.WriteTextAssetContentStr(netUrlConfig.ToNewtonJson());
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
                #endregion

                #region UI预制体操作
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("路径下的UI物体：");
                float uiScvHeight = uiPrefabInfos.Count < 25 ? uiPrefabInfos.Count * 20 : 500;
                uiScrollviewPos = EditorGUILayout.BeginScrollView(uiScrollviewPos, GUILayout.MaxHeight(uiScvHeight));
                for (int i = 0; i < uiPrefabInfos.Count; i++)
                {
                    uiPrefabInfos[i].selected = EditorGUILayout.ToggleLeft(uiPrefabInfos[i].filename, uiPrefabInfos[i].selected, EditorStyles.boldLabel);
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("刷新预制体信息"))
                {
                    InitUIInfos();
                }
                if (GUILayout.Button("全部选中"))
                {
                    for (int i = 0; i < uiPrefabInfos.Count; i++)
                    {
                        uiPrefabInfos[i].selected = true;
                    }
                }
                if (GUILayout.Button("全部不选"))
                {
                    for (int i = 0; i < uiPrefabInfos.Count; i++)
                    {
                        uiPrefabInfos[i].selected = false;
                    }
                }
                if (GUILayout.Button("删除选中预制体"))
                {
                    List<AssetFileInfo> tmp = new List<AssetFileInfo>();
                    for (int i = 0; i < uiPrefabInfos.Count; i++)
                    {
                       if(uiPrefabInfos[i].selected)
                        {
                            File.Delete(uiPrefabInfos[i].fileAbsPath);
                            File.Delete(uiPrefabInfos[i].fileAbsPath + ".meta");
                        }
                        else
                        {
                            tmp.Add(uiPrefabInfos[i]);
                        }
                    }
                    uiPrefabInfos = tmp;
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
                #endregion

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if(GUILayout.Button("打开 Application.persistentDataPath 的文件夹"))
                {
                    if (Directory.Exists(Application.persistentDataPath))
                    {
                        string path = Application.persistentDataPath + "/Unity";
                        if (!Directory.Exists(path))
                        {
                            path = Application.persistentDataPath;
                        }
                        EditorUtility.RevealInFinder(path);
                    }
                }
                EditorGUILayout.SelectableLabel(Application.persistentDataPath);

            }
            // 数据库操作
            else if (selectedOption == 1)
            {
                EditorGUILayout.HelpBox("注意点：\r\n库中所有表的空字段不能为 NULL，必须是一个 string.empty 的内容或者数字为 0 的内容，切记切记！！", MessageType.Info, true);
                EditorGUILayout.LabelField(string.Format("共有数据库 {0} 个：", dbPaths.Count), EditorStyles.boldLabel);
                if (dbPaths.Count > 0)
                {
                    currSelectDb = GUILayout.Toolbar(currSelectDb, dbPaths.Select(d => d.filename).ToArray(), EditorStyles.toolbarButton);

                    if (currSelectDb != oriSelectDb)
                    {
                        oriSelectDb = currSelectDb;
                        oriSelectTableIndex = -1;
                        tablesInSelectedDb = DBOperator.GetAllDataTables(dbPaths[currSelectDb].fileAbsPath);
                    }
                    if (tablesInSelectedDb.Count > 0)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(string.Format("{0} 中共有表 {1} 个：", dbPaths[currSelectDb].filename, tablesInSelectedDb.Count), EditorStyles.boldLabel);
                        currSelectTableIndex = GUILayout.Toolbar(currSelectTableIndex, tablesInSelectedDb.ToArray(), EditorStyles.toolbarButton);
                        // 重新计算数据，防止每帧调用计算
                        if (currSelectTableIndex != oriSelectTableIndex)
                        {
                            oriSelectTableIndex = currSelectTableIndex;
                            currSelectTable = DBOperator.GetDataTabel(tablesInSelectedDb[currSelectTableIndex], dbPaths[currSelectDb].fileAbsPath);
                            selectedRecord = new List<bool>();
                            for (int i = 0; i < currSelectTable.Rows.Count; i++)
                            {
                                selectedRecord.Add(false);
                            }
                            columnIndexWidth.Clear();
                            stateAddRecord = false;
                            stateUpdateRecord = false;
                            addValueList.Clear();
                            updateValueList.Clear();
                            for (int j = 0; j < currSelectTable.Columns.Count; j++)
                            {
                                if (currSelectTable.Columns[j].DataType == typeof(int)
                                     || currSelectTable.Columns[j].DataType == typeof(float)
                                      || currSelectTable.Columns[j].DataType == typeof(double)
                                       || currSelectTable.Columns[j].DataType == typeof(decimal)
                                        || currSelectTable.Columns[j].DataType == typeof(long))
                                {
                                    columnIndexWidth.Add(j, numberColumnWidth);
                                }
                                else
                                {
                                    columnIndexWidth.Add(j, textColumnWidth);
                                }
                                if (currSelectTable.Columns[j].DataType.IsValueType)
                                {
                                    addValueList.Add(0);
                                    updateValueList.Add(0);
                                }
                                else
                                {
                                    addValueList.Add("");
                                    updateValueList.Add("");
                                }
                            }
                           
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(string.Format("表 {0} 中共有记录 {1} 条：", tablesInSelectedDb[currSelectTableIndex], currSelectTable.Rows.Count), EditorStyles.boldLabel);

                        float uiScvHeight = currSelectTable.Rows.Count < 25 ? currSelectTable.Rows.Count * 20 : 500;
                        EditorGUILayout.BeginHorizontal();
                        // 列的名字
                        GUILayout.Label("选择", EditorStyles.boldLabel, GUILayout.MaxWidth(numberColumnWidth));
                        GUILayout.Label("索引", EditorStyles.boldLabel, GUILayout.MaxWidth(numberColumnWidth));
                        for (int i = 0; i < currSelectTable.Columns.Count; i++)
                        {
                            GUILayout.Label(currSelectTable.Columns[i].ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(columnIndexWidth[i]));
                        }
                        EditorGUILayout.EndHorizontal();
                        // 表的详细内容
                        uiScrollviewPos = EditorGUILayout.BeginScrollView(uiScrollviewPos, GUILayout.MaxHeight(uiScvHeight));
                        for (int i = 0; i < currSelectTable.Rows.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            selectedRecord[i] = GUILayout.Toggle(selectedRecord[i], "", GUILayout.MaxWidth(numberColumnWidth));
                            GUILayout.Label(i.ToString(), GUILayout.MaxWidth(numberColumnWidth));
                            for (int j = 0; j < currSelectTable.Columns.Count; j++)
                            {
                                GUILayout.Label(currSelectTable.Rows[i][j].ToString(), EditorStyles.label, GUILayout.MaxWidth(columnIndexWidth[j]));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.BeginHorizontal();
                        if (currSelectTable.Rows.Count > 0)
                        {
                            if (GUILayout.Button("全部选中", EditorStyles.miniButtonLeft))
                            {
                                for (int i = 0; i < selectedRecord.Count; i++)
                                {
                                    selectedRecord[i] = true;
                                }
                            }
                            if (GUILayout.Button("全部不选中", EditorStyles.miniButtonRight))
                            {
                                for (int i = 0; i < selectedRecord.Count; i++)
                                {
                                    selectedRecord[i] = false;
                                }
                            }
                            GUILayout.Space(20);
                        }
                        
                        if (GUILayout.Button("增", EditorStyles.miniButtonLeft))
                        {
                            stateAddRecord = true;
                        }
                        if (GUILayout.Button("删", EditorStyles.miniButtonMid))
                        {
                            List<int> index = new List<int>();
                            for (int i = 0; i < selectedRecord.Count; i++)
                            {
                                if (selectedRecord[i])
                                {
                                    index.Add(i);
                                }
                            }
                            if(index.Count > 0 && EditorUtility.DisplayDialog("提醒：", "选择选择删除的记录数量为：" + index.Count, "确定删除", "取消"))
                            {
                                int res = DBOperator.DeleteRecords(currSelectTable, index, tablesInSelectedDb[currSelectTableIndex], dbPaths[currSelectDb].fileAbsPath);
                                EditorUtility.DisplayDialog("删除结果：", string.Format("选择的数量：{0}\r\n删除的数量：{1}\r\n结果对比：{2}", index.Count, res, index.Count == res?"删除成功":"删除失败"), "确定");
                                // 刷新表记录
                                oriSelectTableIndex = -1;
                            }
                            
                        }
                        if (GUILayout.Button("改", EditorStyles.miniButtonRight))
                        {
                            stateUpdateRecord = true;
                        }
                        EditorGUILayout.EndHorizontal();
                        // 添加记录
                        if (stateAddRecord)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("添加表记录：");
                            for (int i = 0; i < currSelectTable.Columns.Count; i++)
                            {
                                if (currSelectTable.Columns[i].DataType == typeof(int))
                                {
                                    addValueList[i] = EditorGUILayout.IntField(currSelectTable.Columns[i].ToString(), (int)addValueList[i]);
                                }
                                else if (currSelectTable.Columns[i].DataType == typeof(float))
                                {
                                    addValueList[i] = EditorGUILayout.IntField(currSelectTable.Columns[i].ToString(), (int)addValueList[i]);
                                }
                                else if(currSelectTable.Columns[i].DataType == typeof(double))
                                {
                                    addValueList[i] = EditorGUILayout.IntField(currSelectTable.Columns[i].ToString(), (int)addValueList[i]);
                                }
                                else if(currSelectTable.Columns[i].DataType == typeof(long))
                                {
                                    addValueList[i] = EditorGUILayout.IntField(currSelectTable.Columns[i].ToString(), (int)addValueList[i]);
                                }
                                else if (currSelectTable.Columns[i].DataType == typeof(string))
                                {
                                    addValueList[i] = EditorGUILayout.TextField(currSelectTable.Columns[i].ToString(), addValueList[i].ToString());
                                }
                                else if (currSelectTable.Columns[i].DataType == typeof(bool))
                                {
                                    addValueList[i] = EditorGUILayout.Toggle(currSelectTable.Columns[i].ToString(),  (bool)addValueList[i]);
                                }
                                else
                                {
                                    addValueList[i] = EditorGUILayout.TextField(currSelectTable.Columns[i].ToString(), addValueList[i].ToString());
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("添加", GUILayout.MaxWidth(numberColumnWidth)))
                            {
                                
                            }
                            if (GUILayout.Button("取消", GUILayout.MaxWidth(numberColumnWidth)))
                            {
                                stateAddRecord = false;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        // 更新记录
                        if (stateUpdateRecord)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("更新表记录：");
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("更新", GUILayout.MaxWidth(numberColumnWidth)))
                            {

                            }
                            if (GUILayout.Button("取消", GUILayout.MaxWidth(numberColumnWidth)))
                            {
                                stateUpdateRecord = false;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.Label("所选库中没有任何表！");
                    }
                }
            }
            else if (selectedOption == 2)
            {
                GUILayout.Label("在Project面板中选择的物体：", EditorStyles.boldLabel);
            }
            // 测试
            else if (selectedOption == 3)
            {
                //      复选框
                EditorGUILayout.BeginHorizontal(EditorStyles.centeredGreyMiniLabel);
                //testOne = EditorGUILayout.Toggle(testOne, EditorStyles.miniButtonLeft);
                //testTwo = EditorGUILayout.Toggle(testTwo, EditorStyles.miniButtonMid);
                //testThr = EditorGUILayout.Toggle(testThr, EditorStyles.miniButtonRight);
                EditorGUILayout.BeginHorizontal();
            }
        }

        private void OnInspectorUpdate()
        {
            this.Repaint();
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 获取UI预制体的文件信息
        /// </summary>
        private void InitUIInfos()
        {
            uiPrefabInfos.Clear();
            string uiPrePath = string.Format("{0}/{1}", Application.dataPath, config.UIPrePath);
            if (Directory.Exists(uiPrePath))
            {
                DirectoryInfo uiPreDir = new DirectoryInfo(uiPrePath);
                FileInfo[] uiInfos = uiPreDir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
                if (uiInfos.Length > 0)
                {
                    foreach (var fi in uiInfos)
                    {
                        uiPrefabInfos.Add(new AssetFileInfo()
                        {
                            filename = fi.Name,
                            fileRelaPath = string.Format("{0}/{1}", config.UIPrePath, fi.Name),
                            fileAbsPath = string.Format("{0}/{1}/{2}", Application.dataPath, config.UIPrePath, fi.Name)
                        });
                    }

                }
            }
        }
        #endregion

        #region Editor使用的结构

        /// <summary>
        /// 关于网络资源上的配置，生成的json文件放到Application.StreamingAssets文件夹内，由程序读取内容
        /// </summary>
        public class NetUrlConfig
        {
            /// <summary>
            /// 固定json文件的名字
            /// </summary>
            public readonly static string NetUrlFileName = string.Format("{0}/NET_URL_CONFIG.json", Application.streamingAssetsPath);
            /// <summary>
            /// API服务器地址
            /// </summary>
            public string APIHost = "http://127.0.0.1:8000";
            /// <summary>
            /// Assetbundle.manifext的URL
            /// </summary>
            public string ManifestHost = "http://127.0.0.1:8000/static/Assetbundle.manifext";
            /// <summary>
            /// 资源服务器地址
            /// </summary>
            public string ResHost = "http://127.0.0.1:8000/static";
        }

        /// <summary>
        /// 资源文件信息
        /// </summary>
        public class AssetFileInfo
        {
            /// <summary>
            /// 文件是否被选中
            /// </summary>
            public bool selected = false;
            /// <summary>
            /// 物体名字，带后缀
            /// </summary>
            public string filename = null;
            /// <summary>
            /// 文件绝对路径
            /// </summary>
            public string fileAbsPath = null;
            /// <summary>
            /// 文件相对路径，相对于工程文件Asset文件夹
            /// </summary>
            public string fileRelaPath = null;
        }

        /// <summary>
        /// 框架配置内容结构
        /// </summary>
        public class ConfigContent
        {
            /// <summary>
            /// 配置文件所在文件夹
            /// </summary>
            public static readonly string CONFIG_FILE_PATH = string.Format("{0}/FRAME_DIR_CONFIG.json", Application.streamingAssetsPath);

            /// <summary>
            /// 代码的默认命名空间
            /// </summary>
            public string FrameworkNamespace = "ZFramework.App";

            /// <summary>
            /// UI预制体路径
            /// </summary>
            public string UIPrePath = "Art/UI";

            /// <summary>
            /// UI预制对应的脚本路径
            /// </summary>
            public string UIScriptPath = "Scripts/UI";

            /// <summary>
            /// 打的ab包所存放的路径
            /// </summary>
            public string AssetbundlePath = "StreamingAssets/Assetbundles";
        }

        #endregion
    }
}