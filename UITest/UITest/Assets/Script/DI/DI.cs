using System;


public enum DIScope
{
    unknown = -1,
    singleton = 0,
    scene,
}

[AttributeUsage(AttributeTargets.Property)]
public class DI : Attribute
{
    public DIScope Scope { get; set; }
    public string SceneName { get; set; }
    public DI(DIScope scope)
    {
        Scope = scope;
        SceneName = "";
    }

    public DI(DIScope scope, string sceneName)
    {
        Scope = scope;
        SceneName = sceneName;
    }
}