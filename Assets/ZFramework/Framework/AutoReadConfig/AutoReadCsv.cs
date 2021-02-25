using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;
using ZFramework.ClassExt;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自动读取csv类型的配置文件
    /// 必须在T结构上面加上[Serializable]特性
    /// 实际上是二进制文件
    /// </summary>
    [Serializable]
    public class AutoReadCsv<T> : BaseAutoRead<T>, IAutoReadConfigFunc<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    dirName = "Csv";
                    extension = "csv";
#if UNITY_EDITOR
                    platformPath = Application.streamingAssetsPath;
#elif UNITY_STANDALONE
                    platformPath = Application.persistentDataPath;
#endif
                    if (HasFile)
                    {
                        using (FileStream stream = new FileStream(AbsPath, FileMode.Open))
                        {
                            BinaryFormatter bs = new BinaryFormatter();
                            instance = (T)bs.Deserialize(stream);
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
            using (FileStream stream = new FileStream(AbsPath, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, instance);
            }
        }
    }
}