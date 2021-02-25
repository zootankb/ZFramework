using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.UI;
using ZFramework.ClassExt;
using System.IO;
using ZFramework.App;

public class TestUIMgrOpen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIMgr.Init();
        UIMgr.Open<TestOneUIPanel>();
    }
}
