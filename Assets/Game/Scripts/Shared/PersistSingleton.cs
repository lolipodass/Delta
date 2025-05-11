using UnityEngine;

public class PersistSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
