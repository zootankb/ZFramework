using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自动读取xml类型的配置文件
    /// 必须在T结构上面加上[Serializable]特性
    /// </summary>
    [Serializable]
    public class AutoReadXml<T> : BaseAutoRead<T>, IAutoReadConfigFunc<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    dirName = "Xml";
                    extension = "xml";
#if UNITY_EDITOR
                    platformPath = Application.streamingAssetsPath;
#elif UNITY_STANDALONE
                    platformPath = Application.persistentDataPath;
#endif
                    if (HasFile)
                    {
                        using (FileStream stream = new FileStream(AbsPath, FileMode.Open))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            instance = (T)xs.Deserialize(stream);
                        }
                    }
                    else
                    {
                        instance = new T();
                    }
                }
                return instance;
            }
        }

        public void SaveTIfExist()
        {
            Path.GetDirectoryName(AbsPath).CheckOrCreateDir();
            AbsPath.CheckOrCreateFile();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(AbsPath,  FileMode.OpenOrCreate))
            {
                xs.Serialize(stream, instance);
            }
        }
    }
}