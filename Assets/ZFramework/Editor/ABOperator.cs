using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static ZFramework.Editor.FrameworkEditConfig;

namespace ZFramework.Editor
{
    /// <summary>
    /// ab包的操作
    /// </summary>
    public static class ABOperator
    {
        /// <summary>
        /// 根据不同方式打ab包，以文件后缀.unity区分是否是场景文件和其他资源
        /// </summary>
        /// <param name="buildType">0：一对一打包，1：多对一打包</param>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="option"></param>
        /// <param name="target"></param>
        public static void BuildAssetBundle(int buildType, List<SelectAssetInfo> assetInfos, string dir, BuildAssetBundleOptions option = BuildAssetBundleOptions.None, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            List<SelectAssetInfo> sceneInfos = new List<SelectAssetInfo>();
            List<SelectAssetInfo> resInfos = new List<SelectAssetInfo>();
            for (int i = 0; i < assetInfos.Count; i++)
            {
                var info = assetInfos[i];
                if (info.assetNames.First().ToLower().EndsWith(".unity"))
                {
                    sceneInfos.Add(info);
                }
                else
                {
                    resInfos.Add(info);
                }
            }
            switch (buildType)
            {
                case 0:
                    BuildAssetBundleOneToOne(resInfos, dir, option, target);
                    BuildSceneOneToOne(sceneInfos, dir, target);
                    break;
                case 1:
                    BuildAssetBundleMultiToOne(resInfos, dir, option, target);
                    BuildSceneMultiiToOne(sceneInfos, dir, target);
                    break;
            }
        }

        /// <summary>
        /// 多对一打场景包
        /// </summary>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="target"></param>
        public static void BuildSceneMultiiToOne(List<SelectAssetInfo> assetInfos, string dir, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            string tarUnity3dPath = EditorUtility.SaveFilePanel("Save Resource", dir, "New Scene Unity3d", "unity3d");
            if (!string.IsNullOrEmpty(tarUnity3dPath))
            {
                var scenePath = assetInfos.Select(p => p.assetNames.First()).ToArray();
                BuildPipeline.BuildPlayer(scenePath, tarUnity3dPath, target, BuildOptions.BuildAdditionalStreamedScenes);
                Caching.ClearCache();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 一对一打场景包
        /// </summary>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="target"></param>
        public static void BuildSceneOneToOne(List<SelectAssetInfo> assetInfos, string dir, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            for (int i = 0; i < assetInfos.Count; i++)
            {
                string tarPath = string.Format("{0}/{1}.unity3d", dir, assetInfos[i].assetbundleName);
                BuildPipeline.BuildPlayer(assetInfos[i].assetNames.ToArray(), tarPath, target, BuildOptions.BuildAdditionalStreamedScenes);
                Caching.ClearCache();
            }
            //刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 一对一打包
        /// </summary>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="option"></param>
        /// <param name="target"></param>
        public static void BuildAssetBundleOneToOne(List<SelectAssetInfo> assetInfos, string dir, BuildAssetBundleOptions option = BuildAssetBundleOptions.None, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            Caching.ClearCache();
            AssetBundleBuild[] buildMap = new AssetBundleBuild[assetInfos.Count];
            for (int i = 0; i < assetInfos.Count; i++)
            {
                //打包出来的资源包名字
                buildMap[i].assetBundleName = string.Format("{0}.ab", assetInfos[i].assetbundleName);
                buildMap[i].assetNames = assetInfos[i].assetNames.ToArray();
            }
            BuildPipeline.BuildAssetBundles(dir, buildMap, option, target);
            //刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 多对一打包
        /// </summary>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="option"></param>
        /// <param name="target"></param>
        public static void BuildAssetBundleMultiToOne(List<SelectAssetInfo> assetInfos, string dir, BuildAssetBundleOptions option = BuildAssetBundleOptions.None, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            Caching.ClearCache();
            string path = EditorUtility.SaveFilePanel("多对一打包ab包", dir, "New AssetBundle", ".ab");
            if (!string.IsNullOrEmpty(path))
            {
                AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
                //打包出来的资源包名字
                buildMap[0].assetBundleName = string.Format("{0}.ab", Path.GetFileNameWithoutExtension(path));
                // 选择的对象的路径
                List<string> paths = new List<string>();
                foreach (var item in assetInfos)
                {
                    paths.AddRange(item.assetNames);
                }
                buildMap[0].assetNames = paths.ToArray();
                BuildPipeline.BuildAssetBundles(dir, buildMap, option, target);
                //刷新
                AssetDatabase.Refresh();
                Debug.Log("打包完毕");
            }
        }

        /// <summary>
        /// 自动打包带有标签的ab包
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="option"></param>
        /// <param name="target"></param>
        public static void BuildAssetbundleWithTag(string dir, BuildAssetBundleOptions option = BuildAssetBundleOptions.None, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            //自动检测有AssetBundle标签的预制体,图片,音源等,并进行资源压缩.
            BuildPipeline.BuildAssetBundles(dir, option, target);
            AssetDatabase.Refresh();
        }
    }
}