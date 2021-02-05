using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZFramework.ClassExt;

namespace ZFramework.ClassJsonFromFile
{
    /// <summary>
    /// 底层类
    /// </summary>
    public partial class BaseClassJson<T> : IClassJson<T> where T : class, new()
    {
        /// <summary>
        /// 存放Json文件的文件夹路径
        /// </summary>
        public readonly static string ClassJsonFileDir = null;

        /// <summary>
        /// 存放json文件的文件路径
        /// </summary>
        public readonly static string ClassJsonFilePath = null;

        /// <summary>
        /// 每个继承类型中的唯一从json文件中读取的实例
        /// </summary>
        private static T instanceT = null;

        static BaseClassJson()
        {
            ClassJsonFileDir = Path.Combine(Application.persistentDataPath, "BaseClassJson");
            ClassJsonFilePath = Path.Combine(ClassJsonFileDir, string.Format("{0}.json", typeof(BaseClassJson<T>).Name));
            if (File.Exists(ClassJsonFilePath))
            {
                string jsonContent = ClassJsonFilePath.GetTextAssetContentStr();
                if (string.IsNullOrEmpty(jsonContent))
                {
                    instanceT = new T();
                }
                else
                {
                    instanceT = jsonContent.ToNewtonObjectT<T>();
                }
            }
            else
            {
                instanceT = new T();
            }
        }

        public T GetT
        {
            get
            {
                return instanceT;
            }
        }

        public string FileName
        {
            get
            {
                return typeof(BaseClassJson<T>).Name;
            }
        }

        public string FileNameWithExtension
        {
            get { return string.Format("{0}.json", FileName); }
        }

        public bool HasFile()
        {
            return File.Exists(ClassJsonFilePath);
        }

        public void SaveTIfExist()
        {
            string jsonContent = instanceT.ToNewtonJson();
            ClassJsonFilePath.WriteTextAssetContentStr(jsonContent);
        }
    }
}