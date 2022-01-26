using System;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;

public abstract class Injector
{
    public Injector()
    {
        
    }

    public Injector(bool isDirectInject)
    {
        if (isDirectInject) InjectorImpl.ProcessInject(this);
    }

    private void OnLazyInject()
    {
        InjectorImpl.ProcessInject(this);
    }
}