using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InjectorImpl
{
    static InjectorImpl()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public static void ProcessInject(object obj)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute(typeof(DI)) != null)
            .ToList();

        foreach (var property in properties)
        {
            var di = (DI)property.GetCustomAttributes(true)
                .Where(x => x.GetType() == typeof(DI)).FirstOrDefault();


            Type type = property.PropertyType;
            //Debug.Log("ProcessInject type : " + type.Name);

            object instance = null;
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new Type[0], null);
            switch(di.Scope)
            {
                case DIScope.unknown:
                    {
                        instance = constructor.Invoke(null);
                    }
                    break;
                case DIScope.singleton:
                    {
                        if (!Repository.Instance.TryFind(type.Name, out instance))
                        {
                            //Debug.Log(type.Name);
                            instance = constructor.Invoke(null);
                            Repository.Instance.GetSingletonPool().Insert(type.Name, instance);
                            LazyInject(instance);
                        }
                    }
                    break;
                default:
                    if (!Repository.Instance.TryFind(di.SceneName, type.Name, out instance))
                    {
                        instance = constructor.Invoke(null);
                        Repository.Instance.GetScenePool(di.SceneName).Insert(type.Name, instance);
                        LazyInject(instance);
                    }
                    break;
            }

            var parentTypes = new List<Type>();
            GetParentTypes(obj.GetType(), ref parentTypes);
            foreach (var pt in parentTypes)
            {
                //Debug.Log($"ProcessInject parentTypes {obj.GetType().Name } / {pt.Name}");
                SetFields(pt, obj, property, instance);
            }
            SetFields(obj.GetType(), obj, property, instance);
        }
    }

    private static void SetFields(Type t, object obj, PropertyInfo property, object instance) 
    {
        var field = t
        .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(x =>
        {
            //Debug.Log($" {property.Name } / {di.Name} / {x.Name}");
            return x.Name.Contains("<" + property.Name + ">");
        })
        .FirstOrDefault();
        field?.SetValue(obj, instance);
    }

    private static void GetParentTypes(Type thisType, ref List<Type> types)
    {
        Type parentType = thisType.BaseType;
        if (parentType == null)
        {
            return;
        }
        else if (!parentType.IsClass)
        {
            if (!parentType.IsAbstract)
            {
                return;
            }
        }
        else if (parentType.Name == "object")
        {
            return;
        }
        else if (parentType.Name == "MonoBehaviour")
        {
            return;
        }
        else if (parentType.Name == "InjectorBehaviour")
        {
            return;
        }
        types.Add(parentType);
        GetParentTypes(parentType, ref types);
    }

    private static void LazyInject(object instance)
    {
        if (instance is Injector)
        {
            var method = typeof(Injector).GetMethod("OnLazyInject", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(instance, null);
        }
    }

    private static void OnSceneUnloaded(Scene current)
    {
#if UNITY_EDITOR
        if (SceneManager.sceneCount == 2
             && current.name == SceneManager.GetActiveScene().name)
        {
            //Debug.Log("Bug!");
            return;
        }
#endif
        //Debug.Log("OnSceneUnloaded: " + current.name);
        Repository.Instance.RemoveScenePool(current.name);
    }
}
