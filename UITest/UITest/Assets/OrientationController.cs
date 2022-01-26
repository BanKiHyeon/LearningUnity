using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using System;
using UnityEngine.SceneManagement;
using Util;

public class OrientationController : Injector
{
#if UNITY_EDITOR
    private bool isInitialize = false;
    private int initResIndex = -1;
    private Vector2 originGvSize = new Vector2();
    private Vector2 portraitGvSize = new Vector2();
    private Vector2 landscapeGvSize = new Vector2();

    private GameViewSizeGroupType gameViewSizeGroupType;
    private static object gameViewSizesInstance;
    private static MethodInfo getGroupMethod;
#endif

    protected OrientationController()
    {
    }

    private void Initialize()
    {
        
        
#if UNITY_EDITOR
        if (isInitialize) return;
        //from static constructor
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProperty = singleType.GetProperty("instance");
        getGroupMethod = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProperty.GetValue(null, null);

        var gvSize = GetGameViewSize();
        originGvSize = gvSize;
        portraitGvSize = gvSize;
        landscapeGvSize = gvSize;
        if (gvSize.x > gvSize.y)
        {
            MaxstUtil.Swap<float>(ref portraitGvSize.x, ref portraitGvSize.y);
        }
        else
        {
            MaxstUtil.Swap<float>(ref landscapeGvSize.x, ref landscapeGvSize.y);
        }
        gameViewSizeGroupType = GetCurrentGroupType();
        initResIndex = FindSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y);
        if (initResIndex == -1)
        {
            AddCustomSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y);
            initResIndex = FindSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y);
        }
        isInitialize = true;
#endif
    }
    
    public void SetPortraitOrientation()
    {
#if UNITY_EDITOR
        Initialize();
        SetGameViewResolution(portraitGvSize);
#elif UNITY_ANDROID || UNITY_IOS
        Screen.orientation = ScreenOrientation.Portrait;
#endif
    }

    public void SetLandscapeOrientation()
    {
#if UNITY_EDITOR
        Initialize();
        SetGameViewResolution(landscapeGvSize);
#elif UNITY_ANDROID || UNITY_IOS
        Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
    }

    public void ReturnToInitialResolution()
    {
#if UNITY_EDITOR
        SetGameViewResolution(originGvSize);
#endif
    }

#if UNITY_EDITOR
    private Vector2 GetGameViewSize()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);

        var GetSizeOfMainView = gvWndType.GetMethod("GetSizeOfMainGameView", BindingFlags.Static | BindingFlags.NonPublic);
        var res = GetSizeOfMainView.Invoke(gvWnd, null);
        return (Vector2)res;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();

        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");

        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);

        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;

        var widthProperty = gvsType.GetProperty("width");
        var heightProperty = gvsType.GetProperty("height");

        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProperty.GetValue(size, null);
            int sizeHeight = (int)heightProperty.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
        return -1;
    }

    private static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroupMethod.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }
    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }

    public void SetGameViewResolution(Vector2 gvSize)
    {
        var index = FindSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y);
        if (index == -1)
        {
            AddCustomSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y);
            index = Math.Max(FindSize(gameViewSizeGroupType, (int)gvSize.x, (int)gvSize.y), 0);
        }

        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] { index, null });
    }

    public static void AddCustomSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        string text = String.Format("{0}x{1}", width, height);
        var asm = typeof(Editor).Assembly;
        var sizesType = asm.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        var getGroup = sizesType.GetMethod("GetGroup");
        var instance = instanceProp.GetValue(null, null);
        var group = getGroup.Invoke(instance, new object[] { (int)sizeGroupType });
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        Type type = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        var gvsType = asm.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] { type, typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { 1, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
    }
#endif
}

