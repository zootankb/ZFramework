using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZFramework.UpdateAB
{
    /// <summary>
    /// 每个ab包详细的内容
    /// </summary>
    public class ChildDetailManifestInfo 
    {
        /// <summary>
        /// 最原始的字符内容
        /// </summary>
        public string oriContent;
        /// <summary>
        /// 文件版本
        /// </summary>
        public string manifestFileVersion;
        /// <summary>
        /// crc校验
        /// </summary>
        public string crc;
        /// <summary>
        /// 包含的资源信息
        /// </summary>
        public List<ChildManifestInfo> assets;
        /// <summary>
        /// 以来的资源，暂时以字符串存储，后续有需要再做处理
        /// </summary>
        public string dependencies;

        public ChildDetailManifestInfo(string content)
        {
            oriContent = content;
            string[] rows = content.Split(Environment.NewLine.ToCharArray());
            rows = rows.Where(s => !string.IsNullOrEmpty(s)).Select(t => t.Trim()).ToArray();
            int rowIndex = 0;
            if (rows[rowIndex].ToLower().StartsWith("manifestfileversion:"))
            {
                manifestFileVersion = rows[rowIndex].Split(':')[1].Trim();
                ++rowIndex;
            }
            if (rows[rowIndex].ToLower().StartsWith("crc:"))
            {
                crc = rows[rowIndex].Split(':')[1].Trim();
                ++rowIndex;
            }
            while (rowIndex < rows.Length)
            {
                if (rows[rowIndex].ToLower().StartsWith("dependencies"))
                {
                    dependencies = rows[rowIndex].Split(':')[1];
                    break;
                }
                ++rowIndex;
            }
        }
    }
}