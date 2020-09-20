using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using ZFramework.SqliteStore;
using ZFramework.Res;
using ZFramework.ClassExt;

public class TestDb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalDataTable ldt = new LocalDataTable("15", "content18885555", 5, "测试155888855", "2020-09-04 10:02:22:673");
        NetFileTable nft = new NetFileTable(4, 2, "path444444", "1.0.0.1", 0, "测试4更新", "2020-09-04 10:02:22:677");

        //for (int i = 0; i < 2; i++)
        //{
        //    if (DBMgr.DefaultDB.Insert(new List<NetFileTable> { nft, nft, nft }) && DBMgr.DefaultDB.Insert(new List<LocalDataTable> { ldt, ldt, ldt }))
        //    {
        //        print("已经插入");
        //    }
        //    else
        //    {
        //        print("mei插入");
        //    }
        //}

        //print(to.Delete(nft));
        //print(to.Delete(ldt));

        //Dictionary<string, List<object>> d = new Dictionary<string, List<object>>();
        //d.Add("id", new List<object>(){"id", typeof(Int32), 5});
        //foreach (var item in DBMgr.DefaultDB.Select<NetFileTable>(d))
        //{
        //    print("" + item.id+" , " + item.remark);
        //}

        //print(DBMgr.DefaultDB.Insert(nft));


        // 测试下载byte
        //NetResMgr.DownloadBytes("http://127.0.0.1:8000/static/AppOne/123.jpg", (url, code, bs, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(bs.Length);
        //    string path = LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(url);
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //    {
        //        using (BinaryWriter bw = new BinaryWriter(fs))
        //        {
        //            bw.Write(bs);
        //        }
        //    }

        //    DBMgr.CloseAll();
        //}, (progress) => { print(progress); }, new object[] { "测试下载" });

        //NetFileTable bb = new NetFileTable(0, 3, "wawa", null, (int)NetFileTable.FileDownloadState.Downloaded);
        //bool res = DBMgr.Instance.DefaultDB.Insert(bb);
        //print(res);
        //DBMgr.Instance.DefaultDB.Close();

        print("end");
        //StartCoroutine(IEnumGetTexture2D("http://127.0.0.1:8000/static/AppOne/123.jpg"));

        print(TestSingleton.Instance.ToString());

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Test();
        }
    }

    [Obsolete]
    void Test()
    {
        //NetResMgr.DownloadTexture2D("http://127.0.0.1:8000/static/AppOne/123.jpg", (url, code, t2d, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(objs[0]);
        //    rimg.texture = t2d;

        //}, (progress) => { print(progress); }, new object[] { "测试下载" });

        //NetResMgr.DownloadSprite("http://127.0.0.1:8000/static/AppOne/123.jpg", (url, code, sprite, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(objs[0]);
        //    img.sprite = sprite;

        //}, (progress) => { print(progress); }, new object[] { "测试下载" });

        //NetResMgr.DownloadTextAsset("http://127.0.0.1:8000/static/AppOne/123.txt", (url, code, txtast, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(objs[0]);
        //    print(txtast.text);

        //}, (progress) => { print(progress); }, new object[] { "测试下载" });

        //NetResMgr.DownloadAudioClip("http://127.0.0.1:8000/static/AppOne/123.wav", (url, code, aclip, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(objs[0]);
        //    AudioSource.PlayClipAtPoint(aclip, transform.position);
        //}, (progress) => { print(progress); }, new object[] { "测试下载" });

        NetResMgr.DownloadVideoClip("http://127.0.0.1:8000/static/AppOne/123.mp4", (url, code, vclip, objs) =>
        {
            print(url);
            print(code);
            print(objs[0]);
            vplay.url = vclip.path;
        }, (progress) => { print(progress); }, new object[] { "测试下载" });

        //NetResMgr.DownloadAB("http://127.0.0.1:8000/static/AppOne/proto70+gdx1+chctcv.ab", (url, code, ab, objs) =>
        //{
        //    print(url);
        //    print(code);
        //    print(objs[0]);
        //    if (ab != null)
        //    {
        //        GameObject goPre = ab.LoadAsset(Path.GetFileNameWithoutExtension(url)) as GameObject;
        //        GameObject go = Instantiate(goPre, Vector3.zero, Quaternion.identity);
        //    }
        //    else
        //    {
        //        print("ab包为空");
        //    }
        //}, (progress) => { print(progress); }, new object[] { "测试下载" });
    }

    public Image img = null;
    public RawImage rimg = null;
    public VideoPlayer vplay = null;
    private IEnumerator IEnumGetTexture2D(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            yield break;
        }
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, false))
        {
            request.timeout = 60;
            UnityWebRequestAsyncOperation ao = request.SendWebRequest();
            while (true)
            {
                if (request.isDone)
                {
                    if (request.isHttpError || request.isNetworkError)
                    {
                        print("网络错误");
                    }
                    else
                    {
                        Texture2D t2d = DownloadHandlerTexture.GetContent(request);
                        Sprite sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f));
                        byte[] bs = t2d.EncodeToJPG();
                        (LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(url)).WriteTextAssetContentByteArray(bs);
                        img.sprite = sprite;
                    }
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void OnDestroy()
    {
    }
}
