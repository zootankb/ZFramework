using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestAssetbundleManifest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetBundleManifest abm = null;
        print(ZFramework.ConfigContent.configURL.ManifestHost);
        print(Path.GetDirectoryName(ZFramework.ConfigContent.configURL.ManifestHost));
        string url = Path.GetDirectoryName(ZFramework.ConfigContent.configURL.ManifestHost) + "/Assetbundles/" + ZFramework.ConfigContent.CurrPlatform;
        Application.OpenURL(url);
    }
}
