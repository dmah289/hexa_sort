using UnityEngine;

namespace manhnd_sdk.Scripts.SystemDesign
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance => instance;
        
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(instance);
            }
            else Destroy(gameObject);
        }
    }
}