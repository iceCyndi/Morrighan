#if !UNITY_IPHONE
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Driver
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;

using Mogo.Util;
using System;
using System.Collections;
using System.Net;
using System.IO;

public class Driver : MonoBehaviour
{
    private static bool bLoodLib = false;
    public static String FileName
    {
        get
        {
            return "MogoRes";
        }
    }
    //C#的DES只支持64bits的Key
    //http://msdn.microsoft.com/en-us/library/system.security.cryptography.des.key(VS.80).aspx
    public static byte[] Number
    {
        get
        {
            return Utils.GetResNumber(); //"231,20,185,13,20,127,81,79";
        }
    }
    public static bool IsRunOnAndroid = false;
    public Action LevelWasLoaded;
    public static Driver Instance;

    void Awake()
    {
        
        LoggerHelper.Info("--------------------------------------Game Start!-----------------------------------------");
        SystemSwitch.InitSystemSwitch();
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
        DontDestroyOnLoad(transform.gameObject);
        Instance = this;
        gameObject.AddComponent<DriverLib>();
        Application.targetFrameRate = 30;
        DefaultUI.InitLanguageInfo();
        GameObject.Find("MogoForwardLoadingUIPanel").AddComponent<MogoForwardLoadingUIManager>();
        DoInit();
        InvokeRepeating("Tick", 1, 0.02f);
        gameObject.AddComponent<AudioListener>();
    }
    void DoInit()
    {
        InitLoaderLib();
        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);
        var loadCfgSuccess = SystemConfig.Init();
        gameObject.AddComponent("MogoInitialize");
    }
    void InitLoaderLib()
    {
        PluginCallback.Instance.ShowGlobleLoadingUI = MogoForwardLoadingUIManager.Instance.ShowGlobleLoadingUI;
        PluginCallback.Instance.SetLoadingStatus = MogoForwardLoadingUIManager.Instance.SetLoadingStatus;
        PluginCallback.Instance.SetLoadingStatusTip = MogoForwardLoadingUIManager.Instance.SetLoadingStatusTip;

    }
    void Tick()
    {
        TimerHeap.Tick();
        FrameTimerHeap.Tick();
    }

    void OnLevelWasLoaded()
    {
        //在场景初始化时触发场景加载完成事件，以解决场景模型Draw call优化问题。
        if (Driver.Instance.LevelWasLoaded != null)
            Driver.Instance.LevelWasLoaded();
    }

    public static void Invoke(Action action)
    {
        TimerHeap.AddTimer(0, 0, action);
    }
}
#endif