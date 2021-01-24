using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework.UpdateAB
{
    /// <summary>
    /// 主manifest信息（当前平台下载的主manifest文件和下载后主manifest文件的内容解析）
    /// </summary>
    public class CurrPlatformManifestInfo
    {
        /// <summary>
        /// 最原始的字符内容
        /// </summary>
        public string oriContent = null;
        /// <summary>
        /// 文件版本
        /// </summary>
        public string manifestFileVersion = null;
        /// <summary>
        /// crc校验
        /// </summary>
        public string crc;
        /// <summary>
        /// ab包简略信息列表
        /// </summary>
        public List<ChildManifestInfo> assetBundleInfos = null;

        /// <summary>
        ///初始化并赋值
        /// </summary>
        /// <param name="content">问津内容</param>
        private CurrPlatformManifestInfo(string content)
        {
            // 解析内容
            if (!string.IsNullOrEmpty(content))
            {
                oriContent = content;
                assetBundleInfos = new List<ChildManifestInfo>();
                string[] rows = content.Split(Environment.NewLine.ToCharArray());
                rows = rows.Where(s => !string.IsNullOrEmpty(s)).Select(t=>t.Trim()).ToArray();
                int rowIndex = 0;
                if(rows[rowIndex].ToLower().StartsWith("manifestfileversion:"))
                {
                    manifestFileVersion = rows[rowIndex].Split(':')[1].Trim();
                    ++rowIndex;
                }
                if (rows[rowIndex].ToLower().StartsWith("crc:"))
                {
                    crc = rows[rowIndex].Split(':')[1].Trim();
                    ++rowIndex;
                }
                if (rows[rowIndex].ToLower().StartsWith("assetbundlemanifest:"))
                {
                    ++rowIndex;
                }
                if (rows[rowIndex].ToLower().StartsWith("assetbundleinfos:"))
                {
                    ++rowIndex;
                }
                while (true)
                {
                    if (rowIndex >= rows.Length)
                    {
                        return;
                    }
                    string info = rows[rowIndex++].Trim();
                    string name = rows[rowIndex++].Split(':')[1].Trim();
                    string dependencies = rows[rowIndex++].Split(':')[1].Trim();
                    ChildManifestInfo cmi = new ChildManifestInfo(info, name, dependencies);
                    assetBundleInfos.Add(cmi);
                }
            }
        }

        /// <summary>
        /// 通过内容直接赋值
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static CurrPlatformManifestInfo AllocateByContent(string content)
        {
            return new CurrPlatformManifestInfo(content);
        }

        /// <summary>
        /// 通过文件路径直接赋值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static CurrPlatformManifestInfo AllocateByPath(string path)
        {
            string content = null;
            if (File.Exists(path))
            {
                content = path.GetTextAssetContentStr();
            }
            return new CurrPlatformManifestInfo(content);
        }
    }
}