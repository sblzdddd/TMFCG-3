using UnityEngine;

namespace Utils
{
    public class InstancedManagerBase<T> : MonoBehaviour where T : InstancedManagerBase<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        if (!Application.isPlaying)
                        {
                            Debug.LogWarning($"No {typeof(T).Name} found. Access in Edit Mode will not auto-create.");
                            return null;
                        }
                        GameObject go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        private void LogInstanceNotFoundWarning() {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            string callerInfo = "Unknown";
            if (stackTrace.FrameCount > 1)
            {
                var frame = stackTrace.GetFrame(1);
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                    {
                        callerInfo = $"{method.DeclaringType?.Name}.{method.Name}";
                    }
                }
            }
            Debug.LogWarning($"No {typeof(T).Name} found in the scene! Accessing from: {callerInfo}");
        }

        public static bool HasInstance {
            get
            {
                if (instance == null) instance = FindFirstObjectByType<T>();
                return instance != null;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);   
                return;
            }
            instance = this as T;
        }

        protected virtual void OnDestroy() {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
