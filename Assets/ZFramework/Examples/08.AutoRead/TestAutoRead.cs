using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.AutoReadConfig;

public class TestAutoRead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(TestAR.Instance.ID);
        TestAR.Instance.SaveTIfExist();
    }

    class TestAR:AutoReadJson<TestAR>
    {
        public string ID = "-1";
        public string NUMBER = "-1";
    }
}
