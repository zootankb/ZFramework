using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestAssetDataLoad : MonoBehaviour
{

    public string path = "Assets/ZFramework/Examples/05.CreateUIScript/TestUIPanel.prefab";

    public Transform canvas = null;

    void Start()
    {
        //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //Instantiate(go, canvas);
    }
}
