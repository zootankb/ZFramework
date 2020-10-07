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
        /// 根据不同方式打ab包
        /// </summary>
        /// <param name="buildType">0：一对一打包，1：多对一打包</param>
        /// <param name="assetInfos"></param>
        /// <param name="dir"></param>
        /// <param name="option"></param>
        /// <param name="target"></param>
        public static void BuildAssetBundle(int buildType, List<SelectAssetInfo> assetInfos, string dir, BuildAssetBundleOptions option = BuildAssetBundleOptions.None, BuildTarget target = BuildTarget.StandaloneWindows64)
        {
            switch (buildType)
            {
                case 0:
                    BuildAssetBundleOneToOne(assetInfos, dir, option, target);
                    break;
                case 1:
                    BuildAssetBundleMultiToOne(assetInfos, dir, option, target);
                    break;
            }
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
    }
}