using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using System.IO;
using System.Linq;
using ZFramework.SqliteStore;
using ZFramework.ClassExt;

namespace ZFramework.Res
{
    /// <summary>
    /// 搭配的资源管理类
    /// 1.先查找内存里是否有对应资源
    /// 2.再查找数据库
    ///     1.若数据库里面有，就检查本地是否有文件
    ///         有就本地加载
    ///         没有就下载
    ///     2.若数据库里面没有，就从网上下载
    /// 
    /// </summary>
    public static class NetResMgr
    {

        #region 资源的检查、本地数据的更新、数据的发送、接受和处理
        /// <summary>
        /// 二进制数据流的下载，由于二进制流不知道是什么文件类型，不做本地存储
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackByteArr"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadBytes(string url, Action<string, long, byte[], object[]> callbackByteArr = null, Action<float> progress = null, params object[] args)
        {
            Res.NetResDownload.Allocate(url, callbackByteArr, progress, args);
        }

        /// <summary>
        /// ab包的下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAb"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadAB(string url, Action<string, long, AssetBundle, object[]> callbackAb = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_ASSETBUNDLE_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<AssetBundle>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackAb?.Invoke(filepath, 0, Res.ResTmpSave.GetContentT<AssetBundle>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<AssetBundle>(url))
            {
                progress?.Invoke(1.0f);
                callbackAb?.Invoke(url, 0, Res.ResTmpSave.GetContentT<AssetBundle>(url), args);
                return;
            }
            Res.LocalResPath.DIR_ASSETBUNDLE_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.Assetbundle;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_ASSETBUNDLE_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if(result!= null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    Res.ResTmpSave.AddEvent(filepath, callbackAb);
                    // 本地异步加载
                    Action<string, long, AssetBundle, object[]> callback = (ppath, code, ab, cargs) =>
                    {
                        if (ab != null)
                        {
                            Res.ResTmpSave.Invoke(ppath, code, ab, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<AssetBundle>(ppath, code, null, cargs);
                        }
                    };
                    LocalResLoad.Allocate(filepath, callback, progress, args);
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackAb);
                    // 重新下载
                    Action<string, long, byte[], object[]> callback = (uurl, code, bs, cargs) =>
                    {
                        if (bs != null)
                        {
                            string path = LocalResPath.DIR_ASSETBUNDLE_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentByteArray(bs);
                            AssetBundle ab = AssetBundle.LoadFromMemory(bs);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            Res.ResTmpSave.Invoke(uurl, code, ab, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<AssetBundle>(uurl, code, null, cargs);
                        }
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackAb);
                // 下载
                Action<string, long, byte[], object[]> callback = (uurl, code, cbs, cargs) =>
                {
                    if (cbs != null)
                    {
                        string path = LocalResPath.DIR_ASSETBUNDLE_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentByteArray(cbs);
                        AssetBundle ab = AssetBundle.LoadFromMemory(cbs);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        Res.ResTmpSave.Invoke(uurl, code, ab, cargs);
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<AssetBundle>(uurl, code, null, cargs);
                    }
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        /// <summary>
        /// texture2d下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTexture2D"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadTexture2D(string url, Action<string, long, Texture2D, object[]> callbackTexture2D = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<Texture2D>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackTexture2D?.Invoke(filepath, 0, Res.ResTmpSave.GetContentT<Texture2D>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<Texture2D>(url))
            {
                progress?.Invoke(1.0f);
                callbackTexture2D?.Invoke(url, 0, Res.ResTmpSave.GetContentT<Texture2D>(url), args);
                return;
            }
            Res.LocalResPath.DIR_TEXTURE2D_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.Texture2D;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_TEXTURE2D_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if (result != null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    Res.ResTmpSave.AddEvent(filepath, callbackTexture2D);
                    // 本地异步加载
                    Action<string, long, Texture2D, object[]> callback = (ppath, code, ct2d, cargs) =>
                    {
                        if (ct2d != null)
                        {
                            Res.ResTmpSave.Invoke(ppath, code, ct2d, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<Texture2D>(ppath, code, null, cargs);
                        }
                    };
                    LocalResLoad.Allocate(filepath, callback, progress, args);
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackTexture2D);
                    // 重新下载
                    Action<string, long, Texture2D, object[]> callback = (uurl, code, ct2d, cargs) =>
                    {
                        if (ct2d != null)
                        {
                            byte[] bs = null;
                            if (url.ToLower().EndsWith(".png"))
                            {
                                bs = ct2d.EncodeToPNG();
                            }
                            else if (url.ToLower().EndsWith(".jpg") || url.ToLower().EndsWith(".jpeg"))
                            {
                                bs = ct2d.EncodeToJPG();
                            }
                            string path = LocalResPath.DIR_TEXTASSET_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentByteArray(bs);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            Res.ResTmpSave.Invoke(uurl, code, ct2d, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<Texture2D>(uurl, code, null, cargs);
                        }
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackTexture2D);
                // 下载
                Action<string, long, Texture2D, object[]> callback = (uurl, code, ct2d, cargs) =>
                {
                    if (ct2d != null)
                    {
                        byte[] bs = null;
                        if (url.ToLower().EndsWith(".png"))
                        {
                            bs = ct2d.EncodeToPNG();
                        }
                        else if (url.ToLower().EndsWith(".jpg") || url.ToLower().EndsWith(".jpeg"))
                        {
                            bs = ct2d.EncodeToJPG();
                        }
                        string path = LocalResPath.DIR_TEXTASSET_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentByteArray(bs);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        Res.ResTmpSave.Invoke(uurl, code, ct2d, cargs);
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<Texture2D>(uurl, code, null, cargs);
                    }
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        /// <summary>
        /// sprite下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSprite"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadSprite(string url, Action<string, long, Sprite, object[]> callbackSprite = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<Sprite>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackSprite?.Invoke(filepath, 0, Res.ResTmpSave.GetContentT<Sprite>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<Sprite>(url))
            {
                progress?.Invoke(1.0f);
                callbackSprite?.Invoke(url, 0, Res.ResTmpSave.GetContentT<Sprite>(url), args);
                return;
            }
            Res.LocalResPath.DIR_TEXTURE2D_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.Texture2D;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_TEXTURE2D_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if (result != null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    Res.ResTmpSave.AddEvent(filepath, callbackSprite);
                    // 本地异步加载
                    Action<string, long, Sprite, object[]> callback = (ppath, code, sprite, cargs) =>
                    {
                        if (sprite != null)
                        {
                            Res.ResTmpSave.Invoke(ppath, code, sprite, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<Sprite>(ppath, code, null, cargs);
                        }
                    };
                    LocalResLoad.Allocate(filepath, callback, progress, args);
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackSprite);
                    // 重新下载
                    Action<string, long, Sprite, object[]> callback = (uurl, code, sprite, cargs) =>
                    {
                        if (sprite != null)
                        {
                            byte[] bs = null;
                            if (url.ToLower().EndsWith(".png"))
                            {
                                bs = sprite.texture.EncodeToPNG();
                            }
                            else if (url.ToLower().EndsWith(".jpg") || url.ToLower().EndsWith(".jpeg"))
                            {
                                bs = sprite.texture.EncodeToJPG();
                            }
                            string path = LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentByteArray(bs);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            Res.ResTmpSave.Invoke(uurl, code, sprite, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<Sprite>(uurl, code, null, cargs);
                        }
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackSprite);
                // 下载
                Action<string, long, Sprite, object[]> callback = (uurl, code, sprite, cargs) =>
                {
                    if (sprite != null)
                    {
                        byte[] bs = null;
                        if (url.ToLower().EndsWith(".png"))
                        {
                            bs = sprite.texture.EncodeToPNG();
                        }
                        else if (url.ToLower().EndsWith(".jpg") || url.ToLower().EndsWith(".jpeg"))
                        {
                            bs = sprite.texture.EncodeToJPG();
                        }
                        string path = LocalResPath.DIR_TEXTURE2D_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentByteArray(bs);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        Res.ResTmpSave.Invoke(uurl, code, sprite, cargs);
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<Texture2D>(uurl, code, null, cargs);
                    }
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        /// <summary>
        /// TextAsset下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackTextAsset"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadTextAsset(string url, Action<string, long, TextAsset, object[]> callbackTextAsset = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_TEXTASSET_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<TextAsset>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackTextAsset?.Invoke(filepath, 0, Res.ResTmpSave.GetContentT<TextAsset>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<TextAsset>(url))
            {
                progress?.Invoke(1.0f);
                callbackTextAsset?.Invoke(url, 0, Res.ResTmpSave.GetContentT<TextAsset>(url), args);
                return;
            }
            Res.LocalResPath.DIR_TEXTASSET_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.TextAsset;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_TEXTASSET_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if (result != null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    Res.ResTmpSave.AddEvent(filepath, callbackTextAsset);
                    // 本地异步加载
                    Action<string, long, TextAsset, object[]> callback = (ppath, code, txtast, cargs) =>
                    {
                        if (txtast != null)
                        {
                            Res.ResTmpSave.Invoke(ppath, code, txtast, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<TextAsset>(ppath, code, null, cargs);
                        }
                    };
                    LocalResLoad.Allocate(filepath, callback, progress, args);
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackTextAsset);
                    // 重新下载
                    Action<string, long, TextAsset, object[]> callback = (uurl, code, txtast, cargs) =>
                    {
                        if (txtast != null)
                        {
                            string path = LocalResPath.DIR_TEXTASSET_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentStr(txtast.text);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            Res.ResTmpSave.Invoke(uurl, code, txtast, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<TextAsset>(uurl, code, null, cargs);
                        }
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackTextAsset);
                // 下载
                Action<string, long, TextAsset, object[]> callback = (uurl, code, txtast, cargs) =>
                {
                    if (txtast != null)
                    {
                        string path = LocalResPath.DIR_TEXTASSET_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentStr(txtast.text);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        Res.ResTmpSave.Invoke(uurl, code, txtast, cargs);
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<TextAsset>(uurl, code, null, cargs);
                    }
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        /// <summary>
        /// 下载音频文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackAudioClip"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadAudioClip(string url, Action<string, long, AudioClip, object[]> callbackAudioClip = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_AUDIOCLIP_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<AudioClip>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackAudioClip?.Invoke(filepath, 0, Res.ResTmpSave.GetContentT<AudioClip>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<AudioClip>(url))
            {
                progress?.Invoke(1.0f);
                callbackAudioClip?.Invoke(url, 0, Res.ResTmpSave.GetContentT<AudioClip>(url), args);
                return;
            }
            Res.LocalResPath.DIR_AUDIOCLIP_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.AudioClip;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_AUDIOCLIP_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if (result != null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    Res.ResTmpSave.AddEvent(filepath, callbackAudioClip);
                    // 本地异步加载
                    Action<string, long, AudioClip, object[]> callback = (ppath, code, aclip, cargs) =>
                    {
                        if (aclip != null)
                        {
                            Res.ResTmpSave.Invoke(ppath, code, aclip, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<AudioClip>(ppath, code, null, cargs);
                        }
                    };
                    LocalResLoad.Allocate(filepath, callback, progress, args);
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackAudioClip);
                    // 重新下载
                    Action<string, long, AudioClip, object[]> callback = (uurl, code, aclip, cargs) =>
                    {
                        if (aclip != null)
                        {
                            string path = LocalResPath.DIR_AUDIOCLIP_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentByteArray(cargs[cargs.Length - 1] as byte[]);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            List<object> argsList = new List<object>();
                            argsList.AddRange(cargs);
                            argsList.RemoveAt(argsList.Count - 1);
                            Res.ResTmpSave.Invoke(uurl, code, aclip, argsList.ToArray());
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<AudioClip>(uurl, code, null, cargs);
                        }
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackAudioClip);
                // 下载
                Action<string, long, AudioClip, object[]> callback = (uurl, code, aclip, cargs) =>
                {
                    if (aclip != null)
                    {
                        string path = LocalResPath.DIR_AUDIOCLIP_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentByteArray(cargs[cargs.Length - 1] as byte[]);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        List<object> argsList = new List<object>();
                        argsList.AddRange(cargs);
                        argsList.RemoveAt(cargs.Length - 1);
                        Res.ResTmpSave.Invoke(uurl, code, aclip, argsList.ToArray());
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<AudioClip>(uurl, code, null, cargs);
                    }
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        /// <summary>
        /// 下载视频文件，等下载完后才能正确加载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackVideoClip">返回下载到本地的文件路径</param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public static void DownloadVideoClip(string url, Action<string, long, ResTmpSave.NetVideoClip, object[]> callbackVideoClip = null, Action<float> progress = null, params object[] args)
        {
            string filepath = LocalResPath.DIR_VIDEOCLIP_PATH + Path.GetFileName(url);
            if (Res.ResTmpSave.HasContentT<ResTmpSave.NetVideoClip>(filepath))
            {
                progress?.Invoke(1.0f);
                callbackVideoClip?.Invoke(filepath, 0, ResTmpSave.GetContentT<ResTmpSave.NetVideoClip>(filepath), args);
                return;
            }
            else if (Res.ResTmpSave.HasContentT<ResTmpSave.NetVideoClip>(url))
            {
                progress?.Invoke(1.0f);
                callbackVideoClip?.Invoke(url, 0, ResTmpSave.GetContentT<ResTmpSave.NetVideoClip>(url), args);
                return;
            }
            Res.LocalResPath.DIR_VIDEOCLIP_PATH.CheckOrCreateDir();
            int localFileType = (int)NetFileTable.FileType.VideoClip;
            string localFileName = Path.GetFileName(url);
            string localFilePath = string.Format("{0}{1}", Res.LocalResPath.DIR_VIDEOCLIP_PATH, localFileName);
            Dictionary<string, List<object>> conditions = new Dictionary<string, List<object>>();
            conditions.Add("filetype", new List<object> { "filetype", typeof(int), localFileType });
            conditions.Add("path", new List<object> { "path", typeof(string), localFileName });
            var result = DBMgr.DefaultDB.Select<NetFileTable>(conditions);
            if (result != null && result.Count > 0)
            {
                // 存在记录
                var record = result.First();
                // 已经下载好
                if (record.state == (int)NetFileTable.FileDownloadState.Downloaded && File.Exists(localFilePath))
                {
                    // 本地存在视频文件，直接返回路径
                    Res.ResTmpSave.AddEvent(filepath, callbackVideoClip);
                    ResTmpSave.NetVideoClip lnvc = new ResTmpSave.NetVideoClip(url, filepath);
                    Res.ResTmpSave.Invoke(filepath, 0, lnvc, args);

                    // 整个方法不能第二次调用，暂时先清除掉内存数据，以便后面调用
                    Res.ResTmpSave.ClearResInfo<ResTmpSave.NetVideoClip>();
                }
                // 没有下载好
                else
                {
                    Res.ResTmpSave.AddEvent(url, callbackVideoClip);
                    // 重新下载
                    Action<string, long, byte[], object[]> callback = (uurl, code, cbs, cargs) =>
                    {
                        if (cbs != null)
                        {
                            string path = LocalResPath.DIR_VIDEOCLIP_PATH + Path.GetFileName(uurl);
                            path.WriteTextAssetContentByteArray(cbs);
                            record.state = (int)NetFileTable.FileDownloadState.Downloaded;
                            DBMgr.DefaultDB.Update(record);
                            ResTmpSave.NetVideoClip nvc = new ResTmpSave.NetVideoClip(uurl, path);
                            Res.ResTmpSave.Invoke(uurl, code, nvc, cargs);
                        }
                        else
                        {
                            DBMgr.DefaultDB.Delete(record);
                            Res.ResTmpSave.Invoke<ResTmpSave.NetVideoClip>(uurl, code, null, cargs);
                        }
                        // 整个方法不能第二次调用，暂时先清除掉内存数据，以便后面调用
                        Res.ResTmpSave.ClearResInfo<ResTmpSave.NetVideoClip>();
                    };
                    Res.NetResDownload.Allocate(url, callback, progress, args);
                }
            }
            else
            {
                Res.ResTmpSave.AddEvent(url, callbackVideoClip);
                // 下载
                Action<string, long, byte[], object[]> callback = (uurl, code, cbs, cargs) =>
                {
                    if (cbs != null)
                    {
                        string path = LocalResPath.DIR_VIDEOCLIP_PATH + Path.GetFileName(uurl);
                        path.WriteTextAssetContentByteArray(cbs);
                        NetFileTable nft = new NetFileTable(0, localFileType, localFileName, null, (int)NetFileTable.FileDownloadState.Downloaded);
                        DBMgr.DefaultDB.Insert(nft);
                        ResTmpSave.NetVideoClip nvc = new ResTmpSave.NetVideoClip(uurl, path);
                        Res.ResTmpSave.Invoke(uurl, code, nvc, cargs);
                    }
                    else
                    {
                        Res.ResTmpSave.Invoke<ResTmpSave.NetVideoClip>(uurl, code, null, cargs);
                    }
                    // 整个方法不能第二次调用，暂时先清除掉内存数据，以便后面调用
                    Res.ResTmpSave.ClearResInfo<ResTmpSave.NetVideoClip>();
                };
                Res.NetResDownload.Allocate(url, callback, progress, args);
            }
        }

        #endregion

    }
}