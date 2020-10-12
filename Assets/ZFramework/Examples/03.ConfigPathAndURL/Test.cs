using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(ConfigContent.configPath.FrameworkNamespace);
        print(ConfigContent.configPath.UIPrePath);
        print(ConfigContent.configPath.UIScriptPath);
        print(ConfigContent.configPath.AssetbundlePath);

        print(ConfigContent.configURL.APIHost);
        print(ConfigContent.configURL.ManifestHost);
        print(ConfigContent.configURL.ResHost);
    }
}
