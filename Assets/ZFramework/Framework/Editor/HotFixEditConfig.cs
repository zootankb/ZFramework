using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using System.Data;
using System.Reflection;
using ZFramework.UI;
using System.Text.RegularExpressions;

namespace ZFramework.ZEditor
{
    /// <summary>
    /// 热更新使用的窗口
    /// </summary>
    public class HotFixEditConfig : EditorWindow
    {
        #region Data 框架配置操作
        /// <summary>
        /// Unity操作栏的名字
        /// </summary>
        private const string OPTION_NAME = "ZFramework";

        /// <summary>
        /// 当前的存放ab包的文件夹，和FrameworkEditConfig.ConfigContent里面是一样的
        /// </summary>
        private string CUR_AB_DIR_PATH = null;

        /// <summary>
        /// 从电脑上选择的需要对比的存放ab包文件夹的路径
        /// </summary>
        private string PRE_AB_DIR_PATH = null;

        /// <summary>
        /// 打包后的ab包的路径
        /// </summary>
        private string AB_DIR_PATH = null;

        /// <summary>
        /// 从电脑上选择的需要对比的存放ab包文件夹的路径存放名字
        /// </summary>
        private const string PRE_AB_DIR_PATH_PREF_NAME = "PRE_AB_DIR_PATH_PREF_NAME";

        /// <summary>
        /// 新的ab包信息
        /// </summary>
        private List<ABInfo> curABInfos = null;

        /// <summary>
        /// 旧的ab包信息
        /// </summary>
        private List<ABInfo> preABInfos = null;

        /// <summary>
        /// 选择的平台
        /// </summary>
        private int selectedOption = 0;

        /// <summary>
        /// 上次点击选择的平台
        /// </summary>
        private int preSelectedOption = 0;

        /// <summary>
        /// 最新ab包滑动框的位置
        /// </summary>
        private Vector2 abAssetScvPos = Vector2.zero;

        /// <summary>
        /// 上次ab包滑动框的位置
        /// </summary>
        private Vector2 abAssetScvPos0 = Vector2.zero;
        #endregion


        #region MenuItem操作
        [MenuItem(OPTION_NAME + "/打开AB包比对面板")]
        public static void Init()
        {
            HotFixEditConfig window = EditorWindow.GetWindow<HotFixEditConfig>(false, "ZFramework热更配置", false);
            window.Show();
        }
        #endregion

        #region 窗口事件
        /// <summary>
        /// 在新窗口打开时调用
        /// </summary>
        private void OnEnable()
        {
            // 1.寻找配置文件
            FrameworkEditConfig.ConfigContent config = new FrameworkEditConfig.ConfigContent();
            if (File.Exists(ZFramework.ConfigContent.CONFIG_FILE_PATH))
            {
                string json = ZFramework.ConfigContent.CONFIG_FILE_PATH.GetTextAssetContentStr();
                config = json.ToNewtonObjectT<FrameworkEditConfig.ConfigContent>();
            }
            AB_DIR_PATH = Application.dataPath.Replace("Assets", config.AssetbundlePath);
            CUR_AB_DIR_PATH = Path.Combine(AB_DIR_PATH, ABInfo.platforms[selectedOption]);
            PRE_AB_DIR_PATH = EditorPrefs.GetString(PRE_AB_DIR_PATH_PREF_NAME, null);

            // 2.获取 CUR_AB_DIR_PATH 和 PRE_AB_DIR_PATH 两个文件夹里面的ab包信息
            curABInfos = GetAbInfoByDir(CUR_AB_DIR_PATH, ABInfo.platforms[selectedOption]);
            preABInfos = GetAbInfoByDir(PRE_AB_DIR_PATH);
        }

        /// <summary>
        /// 在此实现窗口的界面 Editor GUI
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("选择平台：");
            selectedOption = GUILayout.Toolbar(selectedOption, ABInfo.platforms, EditorStyles.toolbarButton);
            if(preSelectedOption != selectedOption)
            {
                preSelectedOption = selectedOption;
                // 根据所选平台重新获取curABInfos中的信息
                CUR_AB_DIR_PATH = Path.Combine(AB_DIR_PATH, ABInfo.platforms[selectedOption]);
                curABInfos = GetAbInfoByDir(CUR_AB_DIR_PATH, ABInfo.platforms[selectedOption]);
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("选择", GUILayout.MaxWidth(30));
            EditorGUILayout.LabelField("包名", GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("crc", GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("绝对路径");
            EditorGUILayout.EndHorizontal();
            float scvHeight = curABInfos.Count > 10 ? 200 : (curABInfos.Count * 20);
            abAssetScvPos = EditorGUILayout.BeginScrollView(abAssetScvPos, GUILayout.MaxHeight(scvHeight));
            for (int i = 0; i < curABInfos.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                curABInfos[i].selected = GUILayout.Toggle(curABInfos[i].selected, "", GUILayout.MaxWidth(30));
                GUILayout.Label(curABInfos[i].filename, EditorStyles.label, GUILayout.MaxWidth(150));
                GUILayout.Label(curABInfos[i].crc, EditorStyles.label, GUILayout.MaxWidth(150));
                GUILayout.Label(curABInfos[i].fileAbsPath, EditorStyles.label);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("全部选中", EditorStyles.miniButtonLeft))
            {
                for (int i = 0; i < curABInfos.Count; i++)
                {
                    curABInfos[i].selected = true;
                }
            }
            GUILayout.Space(2);
            if (GUILayout.Button("全部不选中", EditorStyles.miniButtonMid))
            {
                for (int i = 0; i < curABInfos.Count; i++)
                {
                    curABInfos[i].selected = false;
                }
            }
            GUILayout.Space(2);
            if (GUILayout.Button("反向选择", EditorStyles.miniButtonRight))
            {
                for (int i = 0; i < curABInfos.Count; i++)
                {
                    curABInfos[i].selected = !curABInfos[i].selected;
                }
            }
            GUILayout.Space(50);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.IsNullOrEmpty(PRE_AB_DIR_PATH)?"没有选择任何文件夹": "原先的AB包文件夹："+ PRE_AB_DIR_PATH);
            if (GUILayout.Button("选择文件夹 【主 Manifest 文件要与文件夹名字相同】", EditorStyles.miniButtonRight))
            {
                PRE_AB_DIR_PATH = EditorUtility.OpenFolderPanel("打开文件夹", null, PRE_AB_DIR_PATH);
                EditorPrefs.SetString(PRE_AB_DIR_PATH_PREF_NAME, PRE_AB_DIR_PATH);
                preABInfos = GetAbInfoByDir(PRE_AB_DIR_PATH);
                Debug.Log(PRE_AB_DIR_PATH);
            }
            EditorGUILayout.EndHorizontal();
            if (!Directory.Exists(PRE_AB_DIR_PATH))
            {
                EditorGUILayout.LabelField(string.Format("选择的文件夹 {0} 不存在", PRE_AB_DIR_PATH));
            }
            else if(preABInfos.Count == 0)
            {
                EditorGUILayout.LabelField(string.Format("文件夹 {0} 没有ab包信息", PRE_AB_DIR_PATH));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("选择", GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField("包名", GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField("crc", GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField("绝对路径");
                EditorGUILayout.EndHorizontal();
                float scvHeight0 = curABInfos.Count > 10 ? 200 : (curABInfos.Count * 20);
                abAssetScvPos0 = EditorGUILayout.BeginScrollView(abAssetScvPos0, GUILayout.MaxHeight(scvHeight0));
                for (int i = 0; i < preABInfos.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    preABInfos[i].selected = GUILayout.Toggle(preABInfos[i].selected, "", GUILayout.MaxWidth(30));
                    GUILayout.Label(preABInfos[i].filename, EditorStyles.label, GUILayout.MaxWidth(150));
                    GUILayout.Label(preABInfos[i].crc, EditorStyles.label, GUILayout.MaxWidth(150));
                    GUILayout.Label(preABInfos[i].fileAbsPath, EditorStyles.label);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("左查询差异【结果如下表】", EditorStyles.miniButtonLeft))
            {

            }
            GUILayout.Space(2);
            if (GUILayout.Button("右查询差异", EditorStyles.miniButtonMid))
            {

            }
            GUILayout.Space(2);
            if (GUILayout.Button("查询结果导出为json【结果如下表】", EditorStyles.miniButtonRight))
            {

            }
            GUILayout.Space(50);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Inspector面板改变时用的函数
        /// </summary>
        private void OnInspectorUpdate()
        {
            this.Repaint();
        }

        /// <summary>
        /// 关闭此窗口时调用的函数
        /// </summary>
        private void OnDestroy()
        {

        }

        #endregion

        #region 私有函数
        /// <summary>
        /// 通过文件夹路径获取manifest文件及对应的文件信息 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private List<ABInfo> GetAbInfoByDir(string dir)
        {
            List<ABInfo> infos = new List<ABInfo>();
            if (Directory.Exists(dir))
            {
                DirectoryInfo diri = new DirectoryInfo(dir);
                string dirName = diri.Name;
                string abExte = ".ab";
                string manifestExte = ".manifest";
                
                FileInfo[] files = diri.GetFiles();
                var mainFileRes = files.Where(p => p.Name.Equals(dirName));
                var mainCrcRes = files.Where(p => p.Name.Equals(dirName + manifestExte));
                if (mainFileRes.Count() == 1 && mainCrcRes.Count() == 1)
                {
                    string mainCrc = GetCrcFromManifest(Path.Combine(dir, dirName + manifestExte));
                    infos.Add(new ABInfo()
                    {
                        selected = true,
                        filename = dirName,
                        crc = mainCrc,
                        fileAbsPath = Path.Combine(dir, dirName),
                        fileRelativePath = Path.Combine(dir, dirName)
                    });
                    infos.Add(new ABInfo()
                    {
                        selected = true,
                        filename = dirName + manifestExte,
                        crc = mainCrc,
                        fileAbsPath = Path.Combine(dir, dirName + manifestExte),
                        fileRelativePath = Path.Combine(dir, dirName + manifestExte)
                    });
                    var abinfos = files.Where(p => p.Extension.ToLower().Equals(abExte) || p.Extension.ToLower().Equals(manifestExte)).ToList();
                    Dictionary<string, FileInfo> abinfoDic = abinfos.ToDictionary(p => p.Name, p => p);
                    foreach (var kvp in abinfoDic)
                    {
                        // 存在对应的ab包和对应的manifest
                        if (kvp.Key.ToLower().EndsWith(abExte) && abinfoDic.ContainsKey(kvp.Key + manifestExte))
                        {
                            string crc = GetCrcFromManifest(Path.Combine(dir, kvp.Key + manifestExte));
                            infos.Add(new ABInfo()
                            {
                                selected = true,
                                filename = kvp.Key,
                                crc = crc,
                                fileAbsPath = Path.Combine(dir, kvp.Key),
                                fileRelativePath = Path.Combine(dir, kvp.Key)
                            });
                            infos.Add(new ABInfo()
                            {
                                selected = true,
                                filename = kvp.Key + manifestExte,
                                crc = crc,
                                fileAbsPath = Path.Combine(dir, kvp.Key + manifestExte),
                                fileRelativePath = Path.Combine(dir, kvp.Key + manifestExte)
                            });
                        }
                    }
                }
            }
            return infos;
        }

        /// <summary>
        /// 通过文件夹路径获和平台取manifest文件及对应的文件信息 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        private List<ABInfo> GetAbInfoByDir(string dir, string platform)
        {
            List<ABInfo> infos = new List<ABInfo>();
            if (Directory.Exists(dir))
            {
                string abExte = ".ab";
                string manifestExte = ".manifest";
                DirectoryInfo diri = new DirectoryInfo(dir);
                FileInfo[] files = diri.GetFiles();
                var mainFileRes = files.Where(p => p.Name.Equals(platform));
                var mainCrcRes = files.Where(p => p.Name.Equals(platform + manifestExte));
                if(mainFileRes.Count()==1 && mainCrcRes.Count() == 1)
                {
                    string relativePath = dir.Replace(Application.dataPath.Replace("Assets", string.Empty), string.Empty);
                    string mainCrc = GetCrcFromManifest(Path.Combine(dir, platform + manifestExte));
                    infos.Add(new ABInfo()
                    {
                        selected = true,
                        filename = platform,
                        crc = mainCrc,
                        fileAbsPath = Path.Combine(dir, platform),
                        fileRelativePath = Path.Combine(relativePath, platform)
                    });
                    infos.Add(new ABInfo()
                    {
                        selected = true,
                        filename = platform + manifestExte,
                        crc = mainCrc,
                        fileAbsPath = Path.Combine(dir, platform + manifestExte),
                        fileRelativePath = Path.Combine(relativePath, platform + manifestExte)
                    });
                    var abinfos = files.Where(p => p.Extension.ToLower().Equals(abExte) || p.Extension.ToLower().Equals(manifestExte)).ToList();
                    Dictionary<string, FileInfo> abinfoDic = abinfos.ToDictionary(p => p.Name, p => p);
                    foreach (var kvp in abinfoDic)
                    {
                        // 存在对应的ab包和对应的manifest
                        if (kvp.Key.ToLower().EndsWith(abExte) && abinfoDic.ContainsKey(kvp.Key + manifestExte))
                        {
                            string crc = GetCrcFromManifest(Path.Combine(dir, kvp.Key + manifestExte));
                            infos.Add(new ABInfo()
                            {
                                selected = true,
                                filename = kvp.Key,
                                crc = crc,
                                fileAbsPath = Path.Combine(dir, kvp.Key),
                                fileRelativePath = Path.Combine(relativePath, kvp.Key)
                            });
                            infos.Add(new ABInfo()
                            {
                                selected = true,
                                filename = kvp.Key + manifestExte,
                                crc = crc,
                                fileAbsPath = Path.Combine(dir, kvp.Key + manifestExte),
                                fileRelativePath = Path.Combine(relativePath, kvp.Key + manifestExte)
                            });
                        }
                    }
                }                
            }
            return infos;
        }

        /// <summary>
        /// 获取Manifest主文件的crc
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetCrcFromManifest(string filePath)
        {
            string content = filePath.GetTextAssetContentStr();
            string[] rows = Regex.Split(content,"\r\n");
            return rows[1].Replace("CRC: ", string.Empty);
        }
#endregion

#region 私有结构
        /// <summary>
        /// AB包使用的结构
        /// </summary>
        public class ABInfo
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
            /// ab包的crc
            /// </summary>
            public string crc = null;
            /// <summary>
            /// 文件绝对路径
            /// </summary>
            public string fileAbsPath = null;
            /// <summary>
            /// 文件相对路径，相对于工程文件夹
            /// </summary>
            public string fileRelativePath = null;

            /// <summary>
            /// 框架支持的全部平台
            /// </summary>
            public readonly static string[] platforms = new string[] { "Windows", "Andriod", "IOS" };

            public override string ToString()
            {
                return string.Format("{0},\t{1},\t{2},\t{3},\t{4}", selected, filename, crc, fileAbsPath, fileRelativePath);
            }
        }
#endregion
    }
}