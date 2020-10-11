using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestSingleton : ZFramework.Singleton.Singleton<TestSingleton>
{
    public override void Init()
    {
        Debug.Log("初始化类 " + typeof(TestSingleton).Name);
    }
}
