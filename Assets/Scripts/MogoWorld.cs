using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using Mogo.Util;
using Mogo.RPC;
using Mogo.Game;
using Mogo.GameData;
//using Mogo.FSM;
using System.Reflection;
using System.Security.Cryptography;

public class MogoWorld
{
    #region 属性

    static private int monsterCount = 0;
    static private int playerCount = 0;
    static private int dummyCount = 0;
    static private int mercenaryCount = 0;
    static private int enemyCount = 0;
    static private uint chgDummyRateTimer = 0;

    static private bool sceneChanged = false;
    static private int orgId = 0;
    static private bool islogining;
    static private float StartTime = 0;

    static private int asyncGearNum = 0;

    static private Dictionary<uint, EntityParent> entities = new Dictionary<uint, EntityParent>();

    static private Dictionary<int, EntityParent> gameObjects = new Dictionary<int, EntityParent>();
    static private Dictionary<uint, Vector3> dropPoints = new Dictionary<uint, Vector3>();

    static public LoginInfo loginInfo;
    static public string passport;
    static public string password;
    static public EntityAccount theAccount;
    static public EntityMyself thePlayer;
    static public EntityParent m_currentEntity;
    static public uint theLittleGuyID;
    static public ScenesManager scenesManager;
    static public GlobalData globalSetting;

    static public int arenaState = 0;
    static public bool isLoadingScene;
    static public bool inCity = true;
    static public bool showClientGM = false;
    static public bool showSkillFx = true;
    static public bool showHitAction = true;
    static public bool showHitEM = true;
    static public bool showHitShader = true;
    static public bool showFloatBlood = true;
    static public bool unhurtMe = false;
    static public bool unhurtDummy = false;
    static public bool pauseAI = false;
    static public bool showFloatText = true;
    static public bool isReConnect = false;
    static public bool rc = false;
    static public bool beKick = false;
    static public string reConnectKey = "";
    static public string baseLoginKey = "";
    static public string baseIP = "";
    static public int basePort = 0;
    static public string gmcontent = "";
    static public bool mainCameraCompleted = false;
    static public bool canAutoFight = false;
    static public bool CGing = false;
    static public bool hasMonster = false;
    static public bool touchLastPathPoint = false;
    static public bool connoning = false;
    static public bool showDebug = false;

    static public readonly object GameDataLocker = new object();
    static public bool IsGameDataReady;
    static public Action OnGameDataReady;
    static public bool isFirstTimeInCity = true;
    static public bool BeginLogin { get; private set; }

     static public CellAttachedInfo me = null;
     static public  List<CellAttachedInfo> others = new List<CellAttachedInfo>();
    static public bool isClientPositionSync;

    static public Dictionary<uint, EntityParent> Entities
    {
        get { return entities; }
    }
    static public Dictionary<int, EntityParent> GameObjects
    {
        get { return gameObjects; }
    }
    static public Dictionary<uint, Vector3> DropPoints
    {
        get { return dropPoints; }
    }
    static public int MonsterCount
    {
        get { return monsterCount; }
    }
    static public int DummyCount
    {
        get { return dummyCount; }
    }
    static public int MercenaryCount
    {
        get { return mercenaryCount; }
    }
    static public int PlayerCount
    {
        get { return playerCount; }
    }
    static public int  EnemyCount
    {
        get { return enemyCount; }
    }
    static public bool IsClientMission
    {
        get { return true; }
    }
    static public bool IsClientPositionSync
    {
        get { return isClientPositionSync; }
        set
        {
            isClientPositionSync = value;
        }
    }
    #endregion;

    static public void Init()
    {
        scenesManager = new ScenesManager();
    }
    static public void Start()
    {
        if (GlobalData.dataMap == null ||!GlobalData.dataMap.ContainsKey(0))
        {
           return;
        }
        globalSetting = Mogo.GameData.GlobalData.dataMap[0];
        LoadLoginScene();
    }
    static public void LoadLoginScene()
    {
        scenesManager.LoadLoginScene((isLoadedScene) =>
        {
            LoginUILogicManager.Instance.IsAutoLogin = SystemConfig.Instance.IsAutoLogin;
            LoginUILogicManager.Instance.IsSaveUserName = SystemConfig.Instance.IsSavePassport;
            LoginUILogicManager.Instance.UserName = SystemConfig.Instance.Passport;
            LoginUILogicManager.Instance.Password = SystemConfig.Instance.Password;

            DefaultUI.HideLoading();

           var serverInfo = SystemConfig.GetSelectedServerInfo();
           if (serverInfo == null)
          {
               serverInfo = SystemConfig.GetRecommentServer();
               if (serverInfo != null)
                   SystemConfig.SelectedServerIndex = SystemConfig.GetServerIndexById(serverInfo.id);
           }
           if (serverInfo != null)
           {
               SystemConfig.Instance.SelectedServer = serverInfo.id;
               SystemConfig.SaveConfig();
           }
        }, DefaultUI.Loading);
    }
    static public void LoadCharacterScene()
    {
        //theAccount.RpcCall("SetPlatAccountReq", SystemConfig.Instance.Passport);
        LoadCharacterScene(null);//theAccount.UpdateCharacterList);
    }
    static public void LoadCharacterScene(Action loaded)
    {
 //       if (theAccount != null)
        {
            bool isCreateCharacter = false;// theAccount.avatarList == null || theAccount.avatarList.Count == 0;
            if (isCreateCharacter)
                GameProcManager.SetGameProgress(ProcType.ChangeScene, globalSetting.chooseCharaterScene, true);
            scenesManager.LoadCharacterScene(() =>
            {
                if (isCreateCharacter) ;
                    //GameProcManager.SetGameProgress(ProcType.ChangeScene, globalSetting.chooseCharaterScene, false);
                if (loaded != null)
                    loaded();
                if (isFirstTimeInCity)
                {
                    isFirstTimeInCity = false;
                }
            }, null, !isCreateCharacter, true);
        }
    }
    static public void LoadHomeScene()
    {
      scenesManager.LoadHomeScene((isLoadScene) =>
         {
        });
        //scenesManager.LoadInstance(1, null);
        
        
       OnEntityAttached();
    }
    static private void OnEntityAttached()
    {
        if (MogoWorld.thePlayer == null)
        {
            thePlayer = new EntityMyself();
            thePlayer.deviator = RandomHelper.GetRandomInt(1, 3000);
            m_currentEntity = thePlayer;
            thePlayer.OnEnterWorld();
        }
    }
    static public void Login()
    {
        BeginLogin = true;
        LoggerHelper.Debug("Login");
     //   var serverInfo = SystemConfig.GetSelectedServerInfo();
     //   if (serverInfo == null)
     //   {
  //          LoggerHelper.Error("Server Info index error: " + SystemConfig.Instance.SelectedServer);
    //        return;
   //     }
   //     if (!serverInfo.CanLogin())
   //     {
   //         return;
    //    }
        Action AfterGetInfo = () =>
        {
            if (islogining) return;
            if (false) //serverInfo.ip == "127.0.0.1")
            {
                ServerProxy.SwitchToLocal();

                ServerProxy.Instance.Login(LoginInfo.GetPCStrList());
                return;
            }
            else 
            {
                ServerProxy.SwitchToRemote();
            }
            if (ServerProxy.Instance.Connected)
            {
                DisConnectServer();
            }
            bool rst = false;
            if (MogoWorld.rc)
            {
                rst = ServerProxy.Instance.Connect(baseIP, basePort);
                LoginReq(rst);
            } 
            else
            {
                Action act = () =>
                {
                    rst = ServerProxy.Instance.Connect("192.168.1.102", 8001);
                    LoginReq(rst);
                };
                act.BeginInvoke(null, null);
            }

        };

        AfterGetInfo();
    }
    static private void LoginReq(bool rst)
    {
        if (rst)
        {
            if (MogoWorld.rc)
            {
                LoggerHelper.Error("send rc key " + reConnectKey);
                ServerProxy.Instance.SendReConnectKey(reConnectKey);
                return;
            }
            ServerProxy.Instance.Login(LoginInfo.GetPCStrList());
        }
        else
        {

        }
    }
    static MogoWorld()
    {
        AddListeners();
    }
    static public void OnEnterWorld(EntityParent entity)
    {
        int oldEnemyCount = enemyCount;
        if (entities.ContainsKey(entity.ID))
        {
            LoggerHelper.Error("Space has the same id:" + entity.ID);
            return;
        }
        if (entity is EntityPlayer)
        {
            playerCount++;
        }
        entities.Add(entity.ID, entity);
        if (oldEnemyCount != enemyCount)
        {
            MogoWorld.hasMonster = true;
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        }
    }
    static private void OnLeaveWorld(uint eid)
    {
        if (!entities.ContainsKey(eid))
        {
            return;
        }
        EntityParent entity = entities[eid];
        if (entity is EntityPlayer)
        {
            playerCount--;
        }

        entities.Remove(eid);
        if (enemyCount <= 0)
        {
            MogoWorld.hasMonster = false;
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        }

    }
    public static void SetGameProgress(ushort progress)
    {
        LoggerHelper.Info("progressL " + progress);
        if (m_currentEntity != null)
            m_currentEntity.RpcCall("SetProgress", progress);
    }
    #region 框架协议处理函数
    static public void OnLoginResp(LoginResult result)
    {
        if (result != LoginResult.SUCCESS)
        {
            LoggerHelper.Debug("OnLoginResp: error");
        }
        else
        {
            LoggerHelper.Debug("OnLoginResp: success");
        }
    }
    static private void OnBaseLogin(string ip, int port, string token)
    {
        ServerProxy.Instance.Disconnect();
        LoggerHelper.Debug("base ip: " + ip + " port: " + port);
        ServerProxy.Instance.Connect(ip, port);
        ServerProxy.Instance.BaseLogin(token);
        baseIP = ip;
        basePort = port;
    }
    static private void OnEntityCellAttached(CellAttachedInfo info)
    {
        LoggerHelper.Debug("OnEntityCellAttached " + info.position);
        EntityParent entity = null;
        var eid = info.id;
        if (eid == thePlayer.ID)
        {
            if (isReConnect && !mainCameraCompleted)
            {
                me = info;
                return;
            }
            entity = thePlayer;
            thePlayer.SetEntityCellInfo(info);
        }
        else if (eid == -1)
        {
            //entity = the
        }

    }
    static private void OnEntityAttached(BaseAttachedInfo baseInfo)
    {
        if (baseInfo.entity == null)
        {
            LoggerHelper.Error("Entity Attach Error.");
            return;
        }
        switch (baseInfo.entity.Name)
        {
            case "Account":
                {
                    m_currentEntity = theAccount;
                    LoggerHelper.Debug("account attach");
                    Action action = () =>
                    {
                        Mogo.GameLogic.LocalServer.LocalServerResManager.Initialize();
                        theAccount.SetEntityInfo(baseInfo);
                        theAccount.OnEnterWorld();
                        theAccount.CheckVersionReq();
                        theAccount.entity = baseInfo.entity;
                        isReConnect = false;
                        LoadCharacterScene();
                    };
                    lock (MogoWorld.GameDataLocker)
                    {
                        if (MogoWorld.IsGameDataReady)
                        {
                            Driver.Invoke(action);
                        }
                        else
                        {
                            MogoWorld.OnGameDataReady = action;
                        }
                    }
                    break;
                }
            case "Avatar":
                {
                    LoggerHelper.Debug("self attach");
                    bool ab = false;
                    if (MogoWorld.thePlayer == null)
                    {
                        ab = true;
                        thePlayer.deviator = RandomHelper.GetRandomInt(1, 3000);
                        thePlayer.CalcuDmgBase();
                    }
                    m_currentEntity = thePlayer;
                    if (ab)
                    {
                        thePlayer.OnEnterWorld();
                    }
                    thePlayer.entity = baseInfo.entity;
                    break;
                }
            default:
                break;

        }
    }
    static private void AOINewEntity(CellAttachedInfo info)
    {
        EntityParent entity = null;
        LoggerHelper.Debug(info.entity.Name);
        if (isReConnect && !mainCameraCompleted)
        {
            others.Add(info);
            return;
        }
        if (Entities.ContainsKey(info.id) || (thePlayer != null && thePlayer.ID == info.id))
        {
            LoggerHelper.Debug("has same id entity in the world");
            return;
        }

        if ((info.entity.Name == "Monster" ||
            info.entity.Name == "Mercenary" ||
            info.entity.Name == "Drop") && inCity)
        {
            return;
        }
        switch (info.entity.Name)
        {
            case "Avatar":
                entity = new EntityPlayer();
                entity.vocation = Vocation.Warrior;
                break;
        }
        entity.ID = info.id;
        entity.entity = info.entity;
        entity.SetEntityCellInfo(info);
        entity.OnEnterWorld();
        if (!isLoadingScene)
            entity.CreateModel();
        OnEnterWorld(entity);
    }
    static public void CreateDummy(uint eid, int x, int y, int cfgId, int difficulty, int spawnPointCfgId)
    {}
   // static private void DelayCreateDummy(EntityDummy e) {}
    static public void CreateDrop(uint eid, int x, int y, int gold, int itemId, int belognAvatar)
    {}
    static private void AOIDelEntity(uint eid)
    {
        if (!entities.ContainsKey(eid))
        {
            return;
        }

        EntityParent entity = entities[eid];
        if (entity == null)
            return;
        entity.OnLeaveWorld();
        OnLeaveWorld(eid);
        EventDispatcher.TriggerEvent(Events.CampaignEvent.RemovePlayerMessage, entity);
    }
    #endregion

    #region 网络处理函数
    static public void ConnectServer(string ip, int port)
    { 
        bool ret = ServerProxy.Instance.Connect(ip, port);
        LoggerHelper.Debug("connect ret: " + ret);
    }
    static public void DisConnectServer()
    {
        ServerProxy.Instance.Disconnect();
    }
    static public void OnConnected(int nErrorID, string message) 
    {
        LoggerHelper.Debug("onConnected server. " + message);
    }
    static public void OnDisconnectionFromServer()
    {
 
    }
    public static void CheckDefMD5()
    {
       // server
    }
    public static void OnCheckDefMD5(DefCheckResult ret)
    { }
    static private void MainCameraCompleted()
    {

    }
    private static void AddListeners()
    {
        EventDispatcher.AddEventListener<string, int>(Events.NetworkEvent.Connect, ConnectServer);
        EventDispatcher.AddEventListener<LoginResult>(Events.FrameWorkEvent.Login, OnLoginResp);
        EventDispatcher.AddEventListener<string, int, string>(Events.FrameWorkEvent.BaseLogin, OnBaseLogin);
        EventDispatcher.AddEventListener<BaseAttachedInfo>(Events.FrameWorkEvent.EntityAttached, OnEntityAttached);
        EventDispatcher.AddEventListener<CellAttachedInfo>(Events.FrameWorkEvent.EntityCellAttached, OnEntityCellAttached);
        EventDispatcher.AddEventListener<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, AOINewEntity);
        EventDispatcher.AddEventListener<uint>(Events.FrameWorkEvent.AOIDelEvtity, AOIDelEntity);
        //EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SceneLoaded);
        EventDispatcher.AddEventListener(Events.OtherEvent.MainCameraComplete, MainCameraCompleted);
        //EventDispatcher.AddEventListener<int, float>(Events.OtherEvent.ChangeDummyRate, ChangeDummyRate);
        EventDispatcher.AddEventListener<DefCheckResult>(Events.FrameWorkEvent.CheckDef, OnCheckDefMD5);
    }
    #endregion


    public class LoginInfo
    {
        public string uid;
        public string timestamp;
        public string strSign;
        public string strPlatId;
        public string strPlatAccount;
        public string token;
        public string platName;

        public string[] GetStrList()
        {
            string[] strs = new string[6];
            strs[0] = uid;
            strs[1] = timestamp;
            strs[2] = strSign;
            strs[3] = strPlatId;
            strs[4] = strPlatAccount;
            strs[5] = token;

            for (int i = 0; i < strs.Length; i++)
            {
                LoggerHelper.Debug(strs[i]); 
            }
            return strs;
        }

        static public string[] GetPCStrList()
        {
            string[] strs = new string[6];
            strs[0] = passport;
            strs[1] = "timestamp";
            strs[2] = "strSign";
            strs[3] = "strPlatId";
            strs[4] = "0";
            strs[5] = "token";

            for (int i = 0; i < strs.Length; i++)
            {
                LoggerHelper.Debug(strs[i]);
            }
            return strs;
        }
    }
}