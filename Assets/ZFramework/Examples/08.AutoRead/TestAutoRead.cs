using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.AutoReadConfig;
using System;

public class TestAutoRead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(TestARJson.Instance.ID);
        TestARJson.Instance.SaveTIfExist();

        print(TestARXml.Instance.ID);
        TestARXml.Instance.SaveTIfExist();

        print(TestARCsv.Instance.NUMBER);
        TestARCsv.Instance.SaveTIfExist();
    }

    class TestARJson:AutoReadJson<TestARJson>
    {
        public string ID = "-1";
        public string NUMBER = "-1";
    }

    [Serializable]
    public class TestARXml : AutoReadXml<TestARXml>
    {
        public string ID = "-2";
        public string NUMBER = "-2";
    }

    [Serializable]
    public class TestARCsv : AutoReadCsv<TestARCsv>
    {
        public string ID = "-3";
        public string NUMBER = "-3";
    }
}
