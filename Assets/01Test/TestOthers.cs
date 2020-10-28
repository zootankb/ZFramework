using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZFramework;
using ZFramework.UpdateAB;

public class TestOthers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.streamingAssetsPath+"/"+ ConfigContent.GetStreamingABDir() + "/" + Path.GetFileName(ConfigContent.configURL.ManifestHost);
        var t = CurrPlatformManifestInfo.AllocateByPath(path);
        foreach (var item in t.assetBundleInfos)
        {
            print(item.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
