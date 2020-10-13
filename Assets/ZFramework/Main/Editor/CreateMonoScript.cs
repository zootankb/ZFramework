using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace ZFramework.ZEditor
{
    /// <summary>
    /// 创建脚本C#文件
    /// </summary>
    public static class CreateMonoScript 
    {
        /// <summary>
        /// 生成资源常量脚本
        /// </summary>
        /// <param name="assetNames"></param>
        /// <param name="scriptName"></param>
        /// <param name="namespaceName"></param>
        /// <param name="savePath"></param>
        public static void CreateAssetNameScript(List<string> assetNames, string scriptName, string namespaceName, string savePath)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < assetNames.Count; i++)
            {
                string p = Path.GetFileNameWithoutExtension(assetNames[i]).ToUpper().Replace(" ", "_").Replace("(", "_").Replace(")", "_");
                sb.Append(string.Format("\t\tpublic const string {0} = \"{1}\";\r\n", p, assetNames[i]));
            }
            string con = ScriptContentModel.AssetScriptModel.Replace("{0}", namespaceName);
            con = con.Replace("{1}", scriptName);
            con = con.Replace("{2}", sb.ToString());
            savePath.WriteTextAssetContentStr(con);
        }

        /// <summary>
        /// 脚本模板类
        /// </summary>
        private static class ScriptContentModel
        {
            /// <summary>
            /// 资源常量的脚本模板
            /// </summary>
            public static string AssetScriptModel ="namespace {0}\r\n{\r\n\tpublic static class {1}\r\n\t{\r\n{2}\r\n\t}\r\n}";

            public static string UIScriptModel = "";
            public static string UIDataScriptModel = "";

        }
    }
}