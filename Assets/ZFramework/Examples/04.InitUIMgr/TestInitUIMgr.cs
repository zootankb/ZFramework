using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestInitUIMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ZFramework.UI.UIMgr.Init();
    }
}
