﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZFramework.UI;

namespace ZFramework.ZEditor
{
    [UnityEditor.CustomEditor(typeof(UIBind))]
    public class UIBindEditor : UnityEditor.Editor
    {
        //序列化
        private UnityEditor.SerializedObject obj;

        //定义变量
        /// <summary>
        /// 属性名字
        /// </summary>
        private SerializedProperty uiname;
        /// <summary>
        /// ui级别
        /// </summary>
        private SerializedProperty level;
        /// <summary>
        /// ui类型
        /// </summary>
        private SerializedProperty uiType;
        /// <summary>
        /// 属性解释
        /// </summary>
        private SerializedProperty explain;

        private void OnEnable()
        {
            obj = new SerializedObject(target);
            uiname = obj.FindProperty("uiName");
            level = obj.FindProperty("level");
            uiType = obj.FindProperty("uiType");
            explain = obj.FindProperty("explain");
            uiname.stringValue = target.name;
            uiType.enumValueIndex = GetUITypeIndex(obj.targetObject as UIBind);
            obj.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            obj.Update();
            EditorGUILayout.PropertyField(uiname, new GUIContent("UI物体属性名字"));
            EditorGUILayout.PropertyField(level, new GUIContent("UI级别"));
            if(level.enumValueIndex == (int)UI.UIBind.UILevel.UI)
            {
                EditorGUILayout.PropertyField(uiType, new GUIContent("UI类型"));
            }
            else
            {
                EditorGUILayout.LabelField("子UI脚本名字", string.Format("{0}ElementPanel", uiname.stringValue));
            }
            EditorGUILayout.PropertyField(explain, new GUIContent("UI属性解释"), GUILayout.MaxHeight(50));
            obj.ApplyModifiedProperties();
        }

        /// <summary>
        /// 根据物体上的UI类型，返回对应的UI类型枚举
        /// </summary>
        /// <param name="bind"></param>
        /// <returns></returns>
        private int GetUITypeIndex(UIBind bind)
        {
            int index = 0;
            if(bind.GetComponent<Text>() != null)
            {
                index = (int)UIBind.UIType.Text;
            }
            else if(bind.GetComponent<Button>() != null)
            {
                index = (int)UIBind.UIType.Button;
            }
            else if (bind.GetComponent<Toggle>() != null)
            {
                index = (int)UIBind.UIType.Toggle;
            }
            else if (bind.GetComponent<Slider>() != null)
            {
                index = (int)UIBind.UIType.Slider;
            }
            else if (bind.GetComponent<Scrollbar>() != null)
            {
                index = (int)UIBind.UIType.Scrollbar;
            }
            else if (bind.GetComponent<Dropdown>() != null)
            {
                index = (int)UIBind.UIType.Dropdown;
            }
            else if (bind.GetComponent<InputField>() != null)
            {
                index = (int)UIBind.UIType.InputField;
            }
            else if (bind.GetComponent<ScrollRect>() != null)
            {
                index = (int)UIBind.UIType.ScrollView;
            }
            else if (bind.GetComponent<Image>() != null)
            {
                index = (int)UIBind.UIType.Image;
            }
            else if (bind.GetComponent<RawImage>() != null)
            {
                index = (int)UIBind.UIType.RawImage;
            }
            else if (bind.GetComponent<Canvas>() != null)
            {
                index = (int)UIBind.UIType.Canvas;
            }
            else
            {
                index = (int)UIBind.UIType.GameObject;
            }
            return index;
        }
    }
}