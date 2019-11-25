using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using Mogo.GameData;
using Mogo.Game;
using Mogo.Util;
using System.Security;


public class MogoInitialize : MonoBehaviour
{
	// Use this for initialization
	void Start () 
    {
       // bool bSuccess = SystemConfig.Init();

        Init();
       

//         MogoWorld.Init();
//         MogoWorld.Start();
//         OnLoginUILoginUp();
	}

    void finish()
    {
     
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    void Init() 
    {
        //StartDrive();
        StartCoroutine(StartInit(null));
    }
    void StartDrive()
    {
        InvokeRepeating("Tick", 1, 0.1f);
    }
    void Tick()
    {
        ServerProxy.Instance.Process();
        ServerProxy.Instance.Update();
    }
    IEnumerator StartInit(Action<int> process)
    {
        yield return null;
        if (gameObject.GetComponent<LoadResources>() == null)
            gameObject.AddComponent<LoadResources>();
        StartCoroutine(AfterInit(process));
    }
    IEnumerator AfterInit(Action<int> process)
    {
        yield return null;
        Action onGameDataReady = () =>
        {
            lock (MogoWorld.GameDataLocker)
            {
                MogoWorld.IsGameDataReady = true;
                if (MogoWorld.OnGameDataReady != null)
                {
                    Driver.Invoke(MogoWorld.OnGameDataReady);
                }
            }
        };
        GameDataControler.Init(null, onGameDataReady);
        yield return null;

        //ServerProxy.Instance.Init();
        //ServerProxy.Instance.BackToChooseServer = MogoWorld.BackToChooseCharacter;
        //process(50);
        //yield return null;
        MogoWorld.Init();
        SoundManager.Init();
        MogoWorld.Start();


        ///------------------------
         //OnLoginUILoginUp();
    }
    void OnLoginUILoginUp()
    {
        string passport = "iceCyndi";
        string password = "123456";

        MogoWorld.passport = passport;
        MogoWorld.password = password;

        MogoWorld.Login();
        SystemConfig.Instance.Passport = passport;
        SystemConfig.Instance.Password = password;
        SystemConfig.SaveConfig();
        LoggerHelper.Debug("LoginUp");
    }
}
