using UnityEngine;
namespace Generic
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance is not null)
                    return instance;
                instance = (T)FindFirstObjectByType(typeof(T));
                if (instance is null)
                {
                    SetupInstance();
                }
                return instance;
            }
        }
        
        static void SetupInstance()
        {
            instance = (T)FindAnyObjectByType(typeof(T));
            if (instance is not null)
                return;
            var gameObject = new GameObject
            {
                name = typeof(T).Name
            };
            instance = gameObject.AddComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
        
        // This method will be called automatically when the runtime is initialized
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeInitialized()
        {
            // Access the instance to ensure it's created early
            // This will trigger the Instance property getter
            _ = Instance;
        }
        
        public virtual void Awake()
        {
            RemoveDuplicates();
        }
        
        void RemoveDuplicates()
        {
            switch (instance)
            {
                case null:
                    instance = this as T;
                    DontDestroyOnLoad(gameObject);
                    break;
                default:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
