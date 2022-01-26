using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InjectorBehaviour : MonoBehaviour
{
    public InjectorBehaviour()
    {
        InjectorImpl.ProcessInject(this);
    }

    public static bool Quitting { get; private set; }    
    private void OnApplicationQuit()
    {
        Quitting = true;
    }
}
