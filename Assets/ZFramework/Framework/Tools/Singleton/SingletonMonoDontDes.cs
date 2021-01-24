using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Singleton
{
    /// <summary>
    /// 单例不销毁物体接口
    /// </summary>
    public class SingletonMonoDontDes<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(go);
                    go.name = typeof(T).FullName;
                    instance = go.AddComponent<T>();
                }
                return instance;
            }
        }
    }
}