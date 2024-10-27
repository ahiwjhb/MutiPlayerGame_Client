using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{ 
    private static T instance;

    private static bool isApplicationQuitting = false;

    public static T Instance {
        get {
            if (instance == null && !isApplicationQuitting) {
                instance = FindAnyObjectByType<T>();
                if (instance != null) {
                    Instance.Init();
                }
                else {
                    instance = new GameObject("New " + typeof(T).Name).AddComponent<T>();
                    SetParentObject(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private static void SetParentObject(GameObject gameObject) {
        GameObject parentObject = GameObject.Find("Singletons");
        if (parentObject == null) {
            parentObject = new GameObject("Singletons");
        }
        gameObject.transform.parent = parentObject.transform;
    }

    protected virtual void OnDestroy() {
        isApplicationQuitting = true;
    }

    protected virtual void Init() {
        instance = this as T;
    }

    private void Awake()
    {
        if (instance == null) {
            Init();
        }
        else if (instance != this) {
#if UNITY_EDITOR
            Debug.LogFormat("当前场景有重复的单例 {0} ", this.name);
#endif
            Destroy(gameObject);
        }
    }
}
