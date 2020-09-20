using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    /// <summary>
    /// 面板化操作多个数据库
    /// </summary>
    public class DBsEditor : EditorWindow
    {
        [MenuItem("多个数据库操作/打开操作面板")]
        public static void OpenEBsEditorWindow()
        {
            Debug.Log("打开了");
            DBsEditor window = EditorWindow.GetWindow<DBsEditor>();
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("this is a panel!");
        }
    }
}