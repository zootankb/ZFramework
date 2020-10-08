
namespace ZFramework.Singleton
{
    /// <summary>
    /// 单例接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        public Singleton()
        {
            Init();
        }

        public virtual void Init()
        {
            // pass
        }
    }
}