using System.Collections.Generic;

public class InstancePool
{
    private readonly Dictionary<string, object> instancePool = new Dictionary<string, object>();

    public bool TryFind(string className, out object value)
    {
        if (instancePool.TryGetValue(className, out value))
        {
            return true;
        }
        return false;
    }

    public void Insert(string className, object value)
    {
        instancePool.Add(className, value);
    }

    public void Remove(string className)
    {
        instancePool.Remove(className);
    }
} 

public class Repository
{
    private static Repository instance = null;
    private static readonly object padlock = new object();

    private InstancePool singletonPool = new InstancePool();
    private Dictionary<string, InstancePool> scenePool = new Dictionary<string, InstancePool>();

    private Repository()
    {

    }

    public static Repository Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Repository();
                    }
                }
            }
            return instance;
        }
    }

    public InstancePool GetSingletonPool()
    {
        return singletonPool;
    }

    public InstancePool GetScenePool(string sceneName)
    {
        if (scenePool.TryGetValue(sceneName, out InstancePool instancePool))
        {
            return instancePool;
        }
        else
        {
            var newInstancePool = new InstancePool();
            scenePool.Add(sceneName, newInstancePool);
            return newInstancePool;
        }
    }

    public void RemoveScenePool(string sceneName)
    {
        scenePool.Remove(sceneName);
    }

    public bool TryFind(string className, out object value)
    {
        if (singletonPool.TryFind(className, out value))
        {
            return true;
        }
        return false;
    }

    public bool TryFind(string sceneName, string className, out object value)
    {
        if (scenePool.TryGetValue(sceneName, out InstancePool instancePool))
        {
            if (instancePool.TryFind(className, out value))
            {
                return true;
            }
        }
        value = null;
        return false;
    }
}