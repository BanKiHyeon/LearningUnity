using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
/// 
//public class InjectorSingleton<T> : InjectorBehaviour where T : InjectorBehaviour
//{
//	private static T _instance;

//	private static object _lock = new object();

//	public static T Instance
//	{
//		get
//		{
//			if (applicationIsQuitting)
//			{
//				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
//					"' already destroyed on application quit." +
//					" Won't create again - returning null.");
//				return null;
//			}

//			lock (_lock)
//			{
//				if (_instance == null)
//				{
//					_instance = (T)FindObjectOfType(typeof(T));

//					if (FindObjectsOfType(typeof(T)).Length > 1)
//					{
//						Debug.LogError("[Singleton] Something went really wrong " +
//							" - there should never be more than 1 singleton!" +
//							" Reopening the scene might fix it.");
//						return _instance;
//					}

//					if (_instance == null)
//					{
//						GameObject singleton = new GameObject();
//						_instance = singleton.AddComponent<T>();
//						singleton.name = "(singleton) " + typeof(T).ToString();
//                        DontDestroyOnLoad(singleton);

//						Debug.Log("[Singleton] An instance of " + typeof(T) +
//							" is needed in the scene, so '" + singleton +
//							"' was created with DontDestroyOnLoad.");
//					}
//					else
//					{
//						Debug.Log("[Singleton] Using instance already created: " +
//							_instance.gameObject.name);
//                    }
//				}

//				return _instance;
//			}
//		}
//	}

//	private static bool applicationIsQuitting = false;
//	/// <summary>
//	/// When Unity quits, it destroys objects in a random order.
//	/// In principle, a Singleton is only destroyed when application quits.
//	/// If any script calls Instance after it have been destroyed, 
//	///   it will create a buggy ghost object that will stay on the Editor scene
//	///   even after stopping playing the Application. Really bad!
//	/// So, this was made to be sure we're not creating that buggy ghost object.
//	/// </summary>
//	public void OnDestroy()
//	{
//		applicationIsQuitting = true;
//	}
//}
public abstract class InjectorSingleton<T> : InjectorBehaviour where T : InjectorBehaviour
{
    #region  Fields
    //[CanBeNull]
    private static T _instance;

    //[NotNull]
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object Lock = new object();

    [SerializeField]
    private bool _persistent = true;
    
    #endregion

    #region  Properties
    //[NotNull]
    public static T Instance
    {
        get
        {
            if (Quitting)
            {
                Debug.LogWarning($"[{nameof(InjectorBehaviour)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }
            lock (Lock)
            {
                if (_instance != null)
                    return _instance;
                var instances = FindObjectsOfType<T>();
                var count = instances.Length;
                if (count > 0)
                {
                    if (count == 1)
                        return _instance = instances[0];
                    Debug.LogWarning($"[{nameof(InjectorBehaviour)}<{typeof(T)}>] There should never be more than one {nameof(InjectorBehaviour)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                    for (var i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);
                    return _instance = instances[0];
                }

                Debug.Log($"[{nameof(InjectorBehaviour)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                return _instance = new GameObject($"({nameof(InjectorBehaviour)}){typeof(T)}")
                           .AddComponent<T>();                
            }
        }
    }
    #endregion

    #region  Methods
    private void Awake()
    {
        if (_persistent)
            DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake() { }
    #endregion
}
