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
        /// 创建UI脚本
        /// </summary>
        /// <param name="uiName">UI的脚本名字</param>
        /// <param name="filedNameAndType">UI的属性名字和类型</param>
        /// <param name="namespaceName">命名空间</param>
        /// <param name="savePathDir">保存文件的文件夹</param>
        public static void CreateUIPanelScript(string uiName, Dictionary<string, string> filedNameAndType, string namespaceName, string savePathDir)
        {

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

            /// <summary>
            /// UI逻辑控制代码
            /// </summary>
            public static string UIScriptModel =
@"using ZFramework.UI;

namespace {namespaceName}
{
    /// <summary>
    /// {uiName}的数据传输结构
    /// </summary>
    public class {uiName}Data : UIPanelData
    {
        // TODO
    }

    /// <summary>
    /// 自定义{uiName}面板操作
    /// </summary>
    public partial class {uiName} : UIPanel
    {
        protected override void SendMsg(int eventId, ZMsg msg)
        {
            base.SendMsg(eventId, msg);
        }

        protected override void OnInit(IUIData uiData)
        {
            mData = uiData as {uiName}Data ?? new {uiName}Data();
            // TODO
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
        }
    }
}";

            /// <summary>
            /// UI数据代码
            /// </summary>
            public static string UIDesignerScriptModel =
@"using UnityEngine.UI;

namespace {namespaceName}
{
    /// <summary>
    /// 自定义UI面板属性与数据存储
    /// </summary>
    public partial class {uiName}
    {
        /// <summary>
        /// 此脚本外不用
        /// </summary>
        private {uiName}Data mPrivateData = null;

        /// <summary>
        /// 传进来的UI数据
        /// </summary>
        private {uiName}Data mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new {uiName}Data());
            }
            set
            {
                mPrivateData = value;
                mUiData = value;
            }
        }

{field}
    }
}";

            /// <summary>
            /// UI子对象脚本名字
            /// </summary>
            public static string UIElementScriptModel =
@"using ZFramework.UI;
using UnityEngine.UI;

namespace {namespaceName}
{
    /// <summary>
    /// {uiName}Panel的子对象UI脚本{uielementName}ElementPanel
    /// </summary>
    public partial class {uielementName}ElementPanel : UIElement
    {
        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
        }
    }
}";

            /// <summary>
            /// UI子对象脚本设计
            /// </summary>
            public static string UIElementDesignerScriptModel =
@"using UnityEngine.UI;

namespace {namespaceName}
{
    /// <summary>
    /// {uiName}的子对象UI设计脚本{uielementName}ElementPanel
    /// </summary>
    public partial class {uielementName}ElementPanel
    {
{field}

        protected override string ComponentName
        {
            get { return this.GetType().Name; }
        }
    }
}";
        }
    }
}