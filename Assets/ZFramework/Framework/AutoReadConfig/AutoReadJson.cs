using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZFramework.ClassExt;

namespace ZFramework.AutoReadConfig
{
    /// <summary>
    /// 自动读取json类型的配置文件
    /// </summary>
    public class AutoReadJson<T> : BaseAutoRead<T>, IAutoReadConfigFunc<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    dirName = "Json";
                    extension = "json";
#if UNITY_EDITOR
                    platformPath = Application.streamingAssetsPath;
#elif UNITY_STANDALONE
                    platformPath = Application.persistentDataPath;
#endif
                    if (HasFile)
                    {
                        string jsonContent = AbsPath.GetTextAssetContentStr();
                        instance = jsonContent.ToNewtonObjectT<T>();
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
            string jsonContent = instance.ToNewtonJson();
            AbsPath.WriteTextAssetContentStr(jsonContent);
        }
    }
}