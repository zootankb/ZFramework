using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;

public class BuildResource : Editor
{
    #region 显示在Unity上的各种名字
    /// <summary>
    /// 显示栏的名字
    /// </summary>
    private const string MenuItemName = "打包工具/";

    /// <summary>
    /// ab包的后缀
    /// </summary>
    private const string AbFileExt = ".ab";

    /// <summary>
    /// 场景包的后缀
    /// </summary>
    private const string SceneFileExt = ".unity";
    #endregion

    #region 检查文件夹
    /// <summary>
    /// pc端文件
    /// </summary>
    public const string pcDirName = "PC";
    /// <summary>
    /// 安卓端文件
    /// </summary>
    public const string AndroidDirName = "Andorid";
    /// <summary>
    /// ios端文件夹
    /// </summary>
    public const string IOSDirName = "IOS";

    /// <summary>
    /// 存储场景包的文件夹路径，在StreamAssets文件夹下
    /// </summary>
    public const string sceneDirName = "Unity3dFile";

    /// <summary>
    /// 存储资源ab包的文件夹路径，在StreamAssets文件夹下
    /// </summary>
    public const string abDirName = "AssetbundleFile";

    /// <summary>
    /// 存储带有标签资源ab包的文件夹路径，在StreamAssets文件夹下
    /// </summary>
    public const string abTagDirName = "TagAssetbundleFile";

    /// <summary>
    /// 检查StreamAssets文件夹下是否存在保存场景包的文件夹
    /// </summary>
    public static void CheckStreamingSceneDir()
    {
        string dirPcScenePath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, pcDirName, sceneDirName);
        string dirAndroidScenePath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AndroidDirName, sceneDirName);
        string dirIOSScenePath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, IOSDirName, sceneDirName);
        if (!Directory.Exists(dirPcScenePath))
        {
            Directory.CreateDirectory(dirPcScenePath);
        }
        if (!Directory.Exists(dirAndroidScenePath))
        {
            Directory.CreateDirectory(dirAndroidScenePath);
        }
        if (!Directory.Exists(dirIOSScenePath))
        {
            Directory.CreateDirectory(dirIOSScenePath);
        }
    }

    /// <summary>
    /// 检查StreamAssets文件夹下是否存在保存资源ab包的文件夹
    /// </summary>
    public static void CheckStreamingAssetbundleDir()
    {
        string dirPcAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, pcDirName, abDirName);
        string dirAndroidAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AndroidDirName, abDirName);
        string dirIOSAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, IOSDirName, abDirName);
        if (!Directory.Exists(dirPcAbPath))
        {
            Directory.CreateDirectory(dirPcAbPath);
        }
        if (!Directory.Exists(dirAndroidAbPath))
        {
            Directory.CreateDirectory(dirAndroidAbPath);
        }
        if (!Directory.Exists(dirIOSAbPath))
        {
            Directory.CreateDirectory(dirIOSAbPath);
        }
    }

    /// <summary>
    /// 检查StreamAssets文件夹下是否存在保存带有标签ab包的文件夹
    /// </summary>
    public static void CheckStreamingTagAssetbundleDir()
    {
        string dirPcTagAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, pcDirName, abTagDirName);
        string dirAndroidTagAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AndroidDirName, abTagDirName);
        string dirIOSTagAbPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, IOSDirName, abTagDirName);
        if (!Directory.Exists(dirPcTagAbPath))
        {
            Directory.CreateDirectory(dirPcTagAbPath);
        }
        if (!Directory.Exists(dirAndroidTagAbPath))
        {
            Directory.CreateDirectory(dirAndroidTagAbPath);
        }
        if (!Directory.Exists(dirIOSTagAbPath))
        {
            Directory.CreateDirectory(dirIOSTagAbPath);
        }
    }

    /// <summary>
    /// 检查StreamAssets文件夹下是否存在保存资源ab包和场景包的文件夹
    /// </summary>
    public static void CheckStreamingDir()
    {
        CheckStreamingSceneDir();
        CheckStreamingAssetbundleDir();
        CheckStreamingTagAssetbundleDir();
    }
    #endregion

    #region PC端

    [MenuItem(MenuItemName + pcDirName + "/一对一打ab包")]
    public static void PC_BuildAssetbundle_OneToOne()
    {
        BuildAssetBundleOneToOne(string.Format("{0}/{1}", pcDirName, abDirName), BuildTarget.StandaloneWindows64);
    }

    [MenuItem(MenuItemName + pcDirName + "/多对一打ab包")]
    public static void PC_BuildAssetbundle_MultiToOne()
    {
        BuildAssetBundleMultiToOne(string.Format("{0}/{1}", pcDirName, abDirName), BuildTarget.StandaloneWindows64);
    }

    [MenuItem(MenuItemName + pcDirName + "/清除AB包")]
    public static void PC_BuildAssetbundle_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, pcDirName, abDirName);
        ClearAB(dirPath);
    }

    [MenuItem(MenuItemName + pcDirName + "/自动打包带有tag值的ab包")]
    public static void PC_BuildTagAssetbundle_OneToOne()
    {
        BuildAssetbundleWithTag(BuildTarget.StandaloneWindows64);
    }

    [MenuItem(MenuItemName + pcDirName + "/清除TagAB包")]
    public static void PC_BuildAssetbundle_ClearTagAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, pcDirName, abTagDirName);
        ClearAB(dirPath);
    }


    [MenuItem(MenuItemName + pcDirName + "/一对一打包场景")]
    public static void PC_BuildScene_OneToOne()
    {
        BuildSceneOneToOne(string.Format("{0}/{1}", pcDirName, sceneDirName), BuildTarget.StandaloneWindows64);
    }

    [MenuItem(MenuItemName + pcDirName + "/多对一打包场景")]
    public static void PC_BuildScene_MultiToOne()
    {
        BuildSceneMultiToOne(BuildTarget.StandaloneWindows64);
    }

    [MenuItem(MenuItemName + pcDirName + "/清除场景包")]
    public static void PC_BuildScene_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, pcDirName, sceneDirName);
        ClearAB(dirPath);
    }
    #endregion

    #region 安卓端

    [MenuItem(MenuItemName + AndroidDirName + "/一对一打ab包")]
    public static void Android_BuildAssetbundle_OneToOne()
    {
        BuildAssetBundleOneToOne(string.Format("{0}/{1}", AndroidDirName, abDirName), BuildTarget.Android);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/多对一打ab包")]
    public static void Android_BuildAssetbundle_MultiToOne()
    {
        BuildAssetBundleMultiToOne(string.Format("{0}/{1}", AndroidDirName, abDirName), BuildTarget.Android);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/清除AB包")]
    public static void Android_BuildAssetbundle_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, AndroidDirName, abDirName);
        ClearAB(dirPath);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/带有tag值的ab包")]
    public static void Android_BuildTagAssetbundle_OneToOne()
    {
        BuildAssetbundleWithTag(BuildTarget.Android);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/清除TagAB包")]
    public static void Android_BuildAssetbundle_ClearTagAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, AndroidDirName, abTagDirName);
        ClearAB(dirPath);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/一对一打包场景")]
    public static void Android_BuildScene_OneToOne()
    {
        BuildSceneOneToOne(string.Format("{0}/{1}", AndroidDirName, sceneDirName), BuildTarget.Android);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/多对一打包场景")]
    public static void Android_BuildScene_MultiToOne()
    {
        BuildSceneMultiToOne(BuildTarget.Android);
    }

    [MenuItem(MenuItemName + AndroidDirName + "/清除场景包")]
    public static void Android_BuildScene_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, AndroidDirName, sceneDirName);
        ClearAB(dirPath);
    }

    #endregion

    #region IOS端

    [MenuItem(MenuItemName + IOSDirName + "/一对一打ab包")]
    public static void IOS_BuildAssetbundle_OneToOne()
    {
        BuildAssetBundleOneToOne(string.Format("{0}/{1}", IOSDirName, abDirName), BuildTarget.iOS);
    }

    [MenuItem(MenuItemName + IOSDirName + "/多对一打ab包")]
    public static void IOS_BuildAssetbundle_MultiToOne()
    {
        BuildAssetBundleMultiToOne(string.Format("{0}/{1}", IOSDirName, abDirName ), BuildTarget.iOS);
    }

    [MenuItem(MenuItemName + IOSDirName + "/清除AB包")]
    public static void IOS_BuildAssetbundle_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, IOSDirName, abDirName);
        ClearAB(dirPath);
    }

    [MenuItem(MenuItemName + IOSDirName + "/带有tag值的ab包")]
    public static void IOS_BuildTagAssetbundle_MultiToOne()
    {
        BuildAssetbundleWithTag(BuildTarget.iOS);
    }

    [MenuItem(MenuItemName + IOSDirName + "/清除TagAB包")]
    public static void IOS_BuildAssetbundle_ClearTagAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, IOSDirName, abTagDirName);
        ClearAB(dirPath);
    }

    [MenuItem(MenuItemName + IOSDirName + "/一对一打包场景")]
    public static void IOSd_BuildScene_OneToOne()
    {
        BuildSceneOneToOne(string.Format("{0}/{1}", IOSDirName, sceneDirName), BuildTarget.iOS);
    }

    [MenuItem(MenuItemName + IOSDirName + "/多对一打包场景")]
    public static void IOS_BuildScene_MultiToOne()
    {
        BuildSceneMultiToOne(BuildTarget.iOS);
    }

    [MenuItem(MenuItemName + IOSDirName + "/清除场景包")]
    public static void IOS_BuildScene_ClearAB()
    {
        string dirPath = Path.Combine(Application.streamingAssetsPath, IOSDirName, sceneDirName);
        ClearAB(dirPath);
    }
    #endregion

    [MenuItem(MenuItemName + "/清除所有AB包")]
    public static void ClearAllAB()
    {
        ClearAB(Application.streamingAssetsPath);
        CheckStreamingDir();
    }

    #region Tag值

    [MenuItem(MenuItemName + "/打印所有tag值")]
    public static void PrintTag()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        if(tags.Length > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tags.Length; i++)
            {
                sb.Append(string.Format("{0}{1}", (sb.Length == 0 ? null : ","), tags[i]));
            }
            Debug.Log("所有的tag为 " + sb.ToString());
        }
        else
        {
            Debug.LogWarning("没有tag值");
        }
    }

    [MenuItem(MenuItemName + "/清除tag值（慎用）")]
    public static void ClearTag()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        if (tags.Length > 0)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                UnityEditorInternal.InternalEditorUtility.RemoveTag(tags[i]);
            }
            Debug.Log("所有的tag已经清除");
        }
        
    }

    #endregion

    #region 打包工具集合

    #region 打带标签的ab包

    /// <summary>
    /// 打包带有标签的ab包
    /// </summary>
    /// <param name="target">目标平台</param>
    public static void BuildAssetbundleWithTag(BuildTarget target)
    {
        CheckStreamingDir();
        string path = string.Empty;
        switch (target)
        {
            case BuildTarget.StandaloneWindows64:
                path = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, pcDirName, abTagDirName);
                break;
            case BuildTarget.Android:
                path = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AndroidDirName, abTagDirName);
                break;
            case BuildTarget.iOS:
                path = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, IOSDirName, abTagDirName);
                break;
        }
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("没有对应的设置针对此打包平台");
            return;
        }
        //自动检测有AssetBundle标签的预制体,图片,音源等,并进行资源压缩.
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, target);
        AssetDatabase.Refresh();
    }

    #endregion

    #region 打不带标签的ab包
    /// <summary>
    /// 一对一打ab包
    /// </summary>
    /// <param name="dir">相对目标文件夹</param>
    /// <param name="target">平台</param>
    public static void BuildAssetBundleOneToOne(string dir, BuildTarget target)
    {
        Caching.ClearCache();
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (selection.Length > 0)
        {
            CheckStreamingDir();
            AssetBundleBuild[] buildMap = new AssetBundleBuild[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                //打包出来的资源包名字
                buildMap[i].assetBundleName = string.Format("{0}{1}", selection[i].name, AbFileExt);
                buildMap[i].assetNames = new string[1] { AssetDatabase.GetAssetPath(selection[i]) };
            } 
            string output = string.Format("{0}/{1}", Application.streamingAssetsPath, dir);
            BuildPipeline.BuildAssetBundles(output, buildMap, BuildAssetBundleOptions.None, target);
            //刷新
            AssetDatabase.Refresh();
            Debug.Log("打包完成");
        }
        else
        {
            Debug.LogWarning("没有选择资源文件");
        }
    }

    /// <summary>
    /// 多对一打ab包
    /// </summary>
    /// <param name="dir">向对目标文件夹</param>
    /// <param name="target">平台</param>
    public static void BuildAssetBundleMultiToOne(string dir, BuildTarget target)
    {
        Caching.ClearCache();
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if(selection.Length > 0)
        {
            string path = EditorUtility.SaveFilePanel("多对一打包ab包", dir, "New AssetBundle", AbFileExt);
            if (!string.IsNullOrEmpty(path))
            {
                CheckStreamingDir();
                AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
                //打包出来的资源包名字
                buildMap[0].assetBundleName = string.Format("{0}{1}", Path.GetFileNameWithoutExtension(path), AbFileExt);
                // 选择的对象的路径
                buildMap[0].assetNames = selection.Select(p => AssetDatabase.GetAssetPath(p)).ToArray();
                string output = string.Format("{0}/{1}", Application.streamingAssetsPath, dir);
                BuildPipeline.BuildAssetBundles(output, buildMap, BuildAssetBundleOptions.None, target);
                //刷新
                AssetDatabase.Refresh();
                Debug.Log("打包完毕");
            }
        }
        else
        {
            Debug.LogWarning("没有选择资源文件");
        }
    }
    #endregion

    #region 打场景包
    /// <summary>
    /// 一打一打包场景包
    /// </summary>
    /// <param name="dir">工程内的向对路径</param>
    /// <param name="target">目标平台</param>
    public static void BuildSceneOneToOne(string dir, BuildTarget target)
    {
        Caching.ClearCache();
        Object[] selections = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (selections.Length > 0)
        {
            CheckStreamingDir();
            for (int i = 0; i < selections.Length; i++)
            {
                string[] sceneLocalPath = new string[] { string.Format("Assets/Scenes/{0}{1}", selections[i].name, SceneFileExt) };
                string tarPath = string.Format("{0}/{1}/{2}.unity3d",Application.streamingAssetsPath, dir, selections[i].name);
                BuildPipeline.BuildPlayer(sceneLocalPath, tarPath, target, BuildOptions.BuildAdditionalStreamedScenes);
                Caching.ClearCache();
                Debug.LogFormat("原场景包 {0} 目标场景包 {1}", sceneLocalPath[0], tarPath);
            }
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning("没有选中任务资源物体");
        }
    }

    /// <summary>
    /// 多打一打包场景包，自定义存储路径
    /// </summary>
    /// <param name="target">目标平台</param>
    public static void BuildSceneMultiToOne(BuildTarget target)
    {
        Caching.ClearCache();
        Object[] selections = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (selections.Length > 0)
        {
            string dirPath = string.Empty;
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    dirPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, pcDirName, sceneDirName);
                    break;
                case BuildTarget.Android:
                    dirPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AndroidDirName, sceneDirName);
                    break;
                case BuildTarget.iOS:
                    dirPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, IOSDirName, sceneDirName);
                    break;
            }
            string tarUnity3dPath = EditorUtility.SaveFilePanel("Save Resource", dirPath, "New Resource", "unity3d");
            if (!string.IsNullOrEmpty(tarUnity3dPath))
            {
                CheckStreamingDir();
                List<string> scenePath = new List<string>();
                for (int i = 0; i < selections.Length; i++)
                {
                    string tmpPath = string.Format("Assets/Scenes/{0}.unity", selections[i].name);
                    if (File.Exists(tmpPath))
                    {
                        scenePath.Add(tmpPath);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("场景文件不存在 {0}", tmpPath));
                    }
                }
                BuildPipeline.BuildPlayer(scenePath.ToArray(), tarUnity3dPath, target, BuildOptions.BuildAdditionalStreamedScenes);
                Caching.ClearCache();
                Debug.LogFormat("原场景包 {0} 目标场景包 {1}", string.Join(",", scenePath.ToArray()), tarUnity3dPath);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("没有选择存储路径");
            }
        }
        else
        {
            Debug.LogWarning("没有选中任务资源物体");
        }
    }
    #endregion

    /// <summary>
    /// 清除文件夹下所有的文件
    /// </summary>
    /// <param name="dirPath"></param>
    private static void ClearAB(string dirPath)
    {
        if (Directory.Exists(dirPath))
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo[] fs = di.GetFiles();
            foreach (var item in fs)
            {
                item.Delete();
            }
            DirectoryInfo[] dis = di.GetDirectories();
            foreach (var item in dis)
            {
                item.Delete();
            }
            //刷新
            AssetDatabase.Refresh();
            Debug.Log("Successed!");
        }
    }
    #endregion
}
