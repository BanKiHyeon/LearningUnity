using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ApplicationQuitListener : InjectorBehaviour
{
    [DI(DIScope.singleton)] OrientationController OrientationController { get; }
    
    // private UnityEvent InitPlayerInfo = new UnityEvent();
    private UnityEvent InitResolution = new UnityEvent();
    
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        // InitPlayerInfo.AddListener(ClearPlayerInfo);
                                                           
        OrientationController.SetPortraitOrientation();
        InitResolution.AddListener(OrientationController.ReturnToInitialResolution);
    }
    
    /*
    private void ClearPlayerInfo()
    {
        PlayerInfo.Clear();
        PlayerInfo.Playmode = PlayerInfo.ModeEnum.UNKNOWN;
        PlayerInfo.Nickname = null;
        Debug.Log("PlayerInfo Clear");
    }
    */

    private void OnApplicationQuit()
    {
        //InitPlayerInfo.Invoke();
        InitResolution?.Invoke();
    }
}
