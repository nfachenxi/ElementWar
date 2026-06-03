using UnityEngine;

namespace Base
{
    /// <summary>
    /// 单例模式限制器
    /// </summary>
    /// <typeparam name="T">子类</typeparam>
    public class SingleMonoBase<T> : MonoBehaviour where T : SingleMonoBase<T>
    {
        public static T INSTANCE; // 实例
        protected virtual void Awake()
        {
            if (INSTANCE != null)
                Debug.LogError(name + "不符合单例模式");
            INSTANCE = (T)this;
        }

        protected virtual void OnDestroy()
        {
            INSTANCE = null;
        }
    
    }
}
