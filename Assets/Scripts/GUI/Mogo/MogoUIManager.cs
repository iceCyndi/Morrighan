using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;

public class MogoUIManager : MonoBehaviour
{
    private static MogoUIManager m_instance;
    public static MogoUIManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MogoMainUIPanel");
                if (obj)
                {
                    m_instance = obj.GetComponentsInChildren<MogoUIManager>(true)[0];
                }
                m_instance.RegisterEvent();
            }
            return MogoUIManager.m_instance;
        }
    }
    private Transform m_myTransform;
    private Camera m_camUI;
    public string waitingWidgetName;
    public bool UICanChange = true;
    public GameObject m_MainUI;
    GameObject m_LoginUI;
    public GameObject m_NewLoginUI;


    private GameObject m_CurrentUI;
    public GameObject CurrentUI
    {
        get { return m_CurrentUI; }
        set 
        {
            m_CurrentUI = value;
            EventDispatcher.TriggerEvent<GameObject>("CurrentUIChange", m_CurrentUI);
        }
    }

    void RegisterEvent()
    {
        EventDispatcher.AddEventListener<GameObject>(Events.MogoUIManagerEvent.SetCurrentUI, SetCurrentUI);
    }
    public void SetCurrentUI(GameObject currentUI)
    {
        CurrentUI = currentUI;
    }

    void Awake()
    {
        m_myTransform = transform;
        m_camUI = m_myTransform.parent.parent.GetComponentsInChildren<Camera>(true)[0];

        m_MainUI = m_myTransform.FindChild("MainUI").gameObject;
    }
    void Update()
    {

    }
    public void ChangeSettingToControlStick()
    {
        if (MainUIViewManager.Instance)
        {
            MainUIViewManager.Instance.SetControllStickEnable(true);
        }
    }
    public void ChangeSettingToTouch()
    {
        if (MainUIViewManager.Instance)
        {
            MainUIViewManager.Instance.SetControllStickEnable(true);
        }
    }

    public void ShowCurrentUI(bool isShow)
    {
        if (!isShow)
        {
            
        }
        CurrentUI.SetActive(isShow);
    }
    bool m_bEnableControlStick = true;
    public bool IsBattleMainUILoaded = false;
    private void TruelyShowMogoBattleMainUI()
    {
        if (!UICanChange)
            return;

        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattleMainUI);
        MainUIViewManager.Instance.SetUIDirty();

        if (m_MainUI == null)
        {
            if (IsBattleMainUILoaded)
            {
                IsBattleMainUILoaded = true;
                //CallWhenUILoaded()
                AssetCacheMgr.GetUIInstance("MainUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_MainUI = go as GameObject;
                        m_MainUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_MainUI.transform.localPosition = Vector3.zero;
                        m_MainUI.transform.localScale = new Vector3(1, 1, 1);
                        m_MainUI.AddComponent<MainUIViewManager>();
                        if (CurrentUI != m_MainUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_MainUI;
                            ShowCurrentUI(true);
                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            m_MainUI.GetComponentsInChildren<MainUIViewManager>(true)[0].SetControllStickEnable(m_bEnableControlStick);
                        }
                    });
            }
        }
        else
        {
            if (CurrentUI != m_MainUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_MainUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;
                m_MainUI.GetComponentsInChildren<MainUIViewManager>(true)[0].SetControllStickEnable(m_bEnableControlStick);
            }
        }
    }
    public void ShowMogoBattleMainUI()
    {
        if (!UICanChange)
            return;

        TruelyShowMogoBattleMainUI();
    }

    bool IsLoginUILoaded = false;
    public void ShowMogoLoginUI(Action callback, Action<float> process = null)
    {
        if (!UICanChange)
            return;

        if (m_LoginUI == null)
        {
            if (!IsLoginUILoaded)
            {
                IsLoginUILoaded = true;
                AssetCacheMgr.GetUIInstance("LoginUI.prefab",
                (prefab, id, go) =>
                {
                    m_LoginUI = go as GameObject;
                    m_LoginUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_LoginUI.transform.localPosition = new Vector3(313.8f, 0, 0);
                    m_LoginUI.transform.localScale = new Vector3(1, 1, 1);
                    m_LoginUI.AddComponent<LoginUIViewManager>();

                    if (CurrentUI != m_LoginUI)
                    {
                        if (CurrentUI != null)
                        {
                            ShowCurrentUI(false);
                        }
                        CurrentUI = m_LoginUI;
                        ShowCurrentUI(true);
                        m_camUI.clearFlags = CameraClearFlags.Depth;

                        if (callback != null)
                            callback();
                        
                    }
                }, process);
            }
        }
    }
    public bool IsNewLoginUILoaded = false;
    private void TruelyShowNewLoginUI(Action cb)
    {
        if (!UICanChange)
            return;
        if (m_NewLoginUI == null)
        {
            if (!IsNewLoginUILoaded)
            {
                IsNewLoginUILoaded = true;
                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("NewLoginUI.prefab",
                (prefab, id, go) =>
                {
                    m_NewLoginUI = go as GameObject;
                    m_NewLoginUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_NewLoginUI.transform.localPosition = Vector3.zero;
                    m_NewLoginUI.transform.localScale = new Vector3(1, 1, 1);
                    m_NewLoginUI.AddComponent<NewLoginUIViewManager>();

                    if (CurrentUI != m_NewLoginUI)
                    {
                        if (CurrentUI != null)
                        {
                            ShowCurrentUI(false);
                        }
                        CurrentUI = m_NewLoginUI;
                       // ShowCurrentUI(true);
                     //   if (MogoMainCamera.instance)
                      //      MogoMainCamera.instance.SetActive(false);
                        m_camUI.clearFlags = CameraClearFlags.Depth;
                        if (cb != null)
                            cb();
                    }
                });
            }

        }
        else
        {
            if (CurrentUI != m_NewLoginUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_NewLoginUI;
                ShowCurrentUI(true);
                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);

                m_camUI.clearFlags = CameraClearFlags.Depth;
                if (cb != null)
                    cb();
            }
        }
    }
    public void ShowNewLoginUI(Action cb)
    {
        if (!UICanChange)
            return;
        TruelyShowNewLoginUI(cb);
    }
    public void CallWhenUILoad(uint deltaTime = 2000, bool defaultTime = true)
    {

    }
    #region 贴图管理
    List<UIAtlas> m_listSpcecityAtlas = new List<UIAtlas>();
    public UIAtlas CommonAtlas;
    public UIAtlas CommonExtraAtlas;
    public UIAtlas MogoNormalMainUIAtlas;
    public UIAtlas SkillIconAtlas;
    public UIAtlas TempAtlas;

    public string GetAtlasNameByJob() 
    {
        string atlasName = "MogoWarrior";
//         switch (MogoWorld.thePlayer.vocation)
//         {
//             case Mogo.Game.Vocation.Archer:
//                 atlasName = "MogoArcher";
//                 break;
//             case Mogo.Game.Vocation.Assassin:
//                 atlasName = "MogoAssasin";
//                 break;
//             case Mogo.Game.Vocation.Mage:
//                 atlasName = "MogoMage";
//                 break;
//             case Mogo.Game.Vocation.Warrior:
//                 atlasName = "MogoWarrior";
//                 break;
//         }
        return atlasName;
    }
    public void TryingSetSpriteName(string iconName, UISprite sp)
    {
        if (string.IsNullOrEmpty(iconName)) return;
        if (sp.atlas != null && sp.atlas.GetSprite(iconName) != null)
        {
            sp.spriteName = iconName;
        }
        if (CommonAtlas.GetSprite(iconName) != null)
        {
            sp.atlas = CommonAtlas;
            sp.spriteName = iconName;
            return;
        }
        else if (CommonExtraAtlas != null && MogoNormalMainUIAtlas.GetSprite(iconName) != null)
        {
            sp.atlas = MogoNormalMainUIAtlas;
            sp.spriteName = iconName;
        }
        else
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i)
            {
                if (m_listSpcecityAtlas[i].GetSprite(iconName) != null)
                {
                    sp.atlas = m_listSpcecityAtlas[i];
                    sp.spriteName = iconName;
                    if (sp.atlas.spriteMaterial.mainTexture == null)
                    {
                        AssetCacheMgr.GetUIResource(m_listSpcecityAtlas[i].name + ".png", (obj) => 
                        {
                            sp.atlas.spriteMaterial.mainTexture = (Texture)obj;
                            sp.atlas.MarkAsDirty();
                        });
                    }
                    return;
                }
            }

            if (TempAtlas != null)
            {
                if (TempAtlas.GetSprite(iconName) != null)
                {
                    sp.atlas = TempAtlas;
                    sp.spriteName = iconName;
                    return;
                }
                else
                {
                    AssetCacheMgr.ReleaseResource(TempAtlas.texture);
                    AssetCacheMgr.ReleaseInstance(TempAtlas);
                    TempAtlas = null;
                }
            }

            if (iconName.Length >= 11 && iconName.Substring(0, 11) == "icon_archer")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);
                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoArcher20EquipUI.prefab", (prefab, guid, gameObject) => 
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoArcher40EquipUI.prefab", (prefab, guid, gameObject) => 
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
          
                }
            }
            else if (iconName.Length >= 9 && iconName.Substring(0, 9) == "icon_mage")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);
                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoMage20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoMage40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
            else if (iconName.Length >= 13 && iconName.Substring(0, 13) == "icon_assassin")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);
                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoAssassin20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoAssassin40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
            else if (iconName.Length >= 12 && iconName.Substring(0, 12) == "icon_warrior")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);
                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoWarrior20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoWarrior40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;
                            TempAtlas = go.GetComponentInChildren<UIAtlas>();
                            go.hideFlags = HideFlags.HideAndDontSave;
                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
        }


    }
    public UIAtlas GetAtlasByIconName(string iconName) 
    {
        if (CommonAtlas.GetSprite(iconName) != null)
        {
            return CommonAtlas;
        }
        else if (CommonExtraAtlas != null && CommonExtraAtlas.GetSprite(iconName) != null)
        {
            return CommonExtraAtlas;
        }
        else if (MogoNormalMainUIAtlas != null && MogoNormalMainUIAtlas.GetSprite(iconName) != null)
        {
            return MogoNormalMainUIAtlas;
        }
        else if (SkillIconAtlas != null && SkillIconAtlas.GetSprite(iconName) != null)
        {
            return SkillIconAtlas;
        }
        else 
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i)
            {
                if (m_listSpcecityAtlas[i].GetSprite(iconName) != null)
                {
                    if (SystemSwitch.DestroyResource)
                    {
                        if (m_listSpcecityAtlas[i].spriteMaterial.mainTexture == null)
                        {
                            AssetCacheMgr.GetUIResource(string.Concat(m_listSpcecityAtlas[i].name, ".png"),
                                (obj) =>
                                {
                                    m_listSpcecityAtlas[i].spriteMaterial.mainTexture = (Texture)obj;
                                    m_listSpcecityAtlas[i].MarkAsDirty();
                                });
                        }
                    }
                    return m_listSpcecityAtlas[i];
                }

            }
            return CommonAtlas;
        }

    }
    public UIAtlas GetSkillIconAtlas()
    {
        return SkillIconAtlas;
    }
    public void ReleaseUIResources()
    {
        if (m_listSpcecityAtlas.Count > 0)
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i )
            {
                AssetCacheMgr.ReleaseInstance(m_listSpcecityAtlas[i].gameObject);
                AssetCacheMgr.ReleaseInstance(m_listSpcecityAtlas[i].texture);
                m_listSpcecityAtlas[i] = null;
            }
                m_listSpcecityAtlas.Clear();
        }
        if (SkillIconAtlas != null)
        {
            AssetCacheMgr.ReleaseInstance(SkillIconAtlas.gameObject);
            AssetCacheMgr.ReleaseInstance(SkillIconAtlas.texture);
            SkillIconAtlas = null;
        }

        if (TempAtlas != null)
        {
            AssetCacheMgr.ReleaseInstance(TempAtlas.gameObject);
            AssetCacheMgr.ReleaseInstance(TempAtlas.texture);
            TempAtlas = null;
        }
    }
    #endregion

    #region Camera管理
    private Camera m_mainUICamera;
    public void LockMainCamera(bool isLock)
    {
        GetMainUICamera().enabled = !isLock;
        GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].enabled = !isLock;
        /*
         */
    }
    public Camera GetMainUICamera()
    {
        if (m_mainUICamera == null)
        {
            m_mainUICamera = m_mainUICamera = GameObject.Find("MogoMainUI").transform.FindChild("Camera").GetComponent<Camera>();
        }
        return m_mainUICamera;
    }
    #endregion

    #region 加载UI
    public void FirstPreLoadUIResources() { }
    public void SecondPreLoadUIResources() { }
    protected int InstanceUIPartCount = 0;
    protected void CheckDefaultLoadInstanceUI() { }
    public void LoadUIResource() 
    {
        AssetCacheMgr.GetUIResource("MogoLakeBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDeepBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoPurposeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoOrangeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoRedMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoYellowMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoRoseRedMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoGrassGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoBlackWhiteMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonPurposeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonOrangeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonDrakGoldMat.mat", (obj) => { });


        AssetCacheMgr.GetUIResource("fx_ui_skill_yes.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("fx_ui_anliuzhuanquan.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("ComboAttackNum.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("fx_ui_icon_open.prefab", null);

        if (MogoUIManager.Instance.SkillIconAtlas == null)
        {
            string atlasName = "";
            switch (4) //MogoWorld.thePlayer.vocation)
            {
//                 case Mogo.Game.Vocation.Archer:
//                     atlasName = "MogoArcherSkillIcon.prefab";
//                     break;
//                 case Mogo.Game.Vocation.Assassin:
//                     atlasName = "MogoAssassinSkillIcon.prefab";
//                     break;
//                 case Mogo.Game.Vocation.Mage:
//                     atlasName = "MogoMageSkillIcon.prefab";
//                     break;
                case 4 : //Mogo.Game.Vocation.Warrior:
                    atlasName = "MogoWarriorSkillIcon.prefab";
                    break;
            }
            AssetCacheMgr.GetUIInstance(atlasName, (prefab, guid, gameObject) => 
            {
                GameObject go = (GameObject)gameObject;
                MogoUIManager.Instance.SkillIconAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }
        if (CommonAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;
                CommonAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }
        if (CommonExtraAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoUIExtra.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;
                CommonExtraAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }
        if (MogoNormalMainUIAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoNormalMainUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;
                MogoNormalMainUIAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }
        if (m_listSpcecityAtlas.Count == 0)
        {
            AssetCacheMgr.GetUIInstance(MogoUIManager.Instance.GetAtlasNameByJob() + "20EquipUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;
                go.name = MogoUIManager.Instance.GetAtlasNameByJob() + "20EquipUI";
                m_listSpcecityAtlas.Add(go.GetComponentInChildren<UIAtlas>());

                go.hideFlags = HideFlags.HideAndDontSave;
                if (SystemSwitch.DestroyResource)
                {
                    m_listSpcecityAtlas[m_listSpcecityAtlas.Count - 1].spriteMaterial.mainTexture = null;
                    AssetCacheMgr.ReleaseResourceImmediate(string.Concat(MogoUIManager.Instance.GetAtlasNameByJob(), "20EquipUI.png"));
                }

            });
            AssetCacheMgr.GetUIInstance(MogoUIManager.Instance.GetAtlasNameByJob() + "40EquipUI.prefab", (prefab, guid, gameObject) => 
            {
                 GameObject go = (GameObject)gameObject;
                go.name = MogoUIManager.Instance.GetAtlasNameByJob() + "40EquipUI";
                m_listSpcecityAtlas.Add(go.GetComponentInChildren<UIAtlas>());

                go.hideFlags = HideFlags.HideAndDontSave;
                if (SystemSwitch.DestroyResource)
                {
                    m_listSpcecityAtlas[m_listSpcecityAtlas.Count - 1].spriteMaterial.mainTexture = null;
                    AssetCacheMgr.ReleaseResourceImmediate(string.Concat(MogoUIManager.Instance.GetAtlasNameByJob(), "40EquipUI.png"));
                }

            });
        }
    }
    public void PreLoadResources(string[] resName, Action<UnityEngine.Object[]> callBack)
    {
        AssetCacheMgr.GetUIResources(resName, callBack);
    }
    public void PreLoadResource(string resName, Action<UnityEngine.Object> callBack) 
    {
        AssetCacheMgr.GetUIResource(resName, (obj) =>
        {
            if (callBack != null)
            {
                callBack(obj);
            }
        });
    }
    #endregion

    #region 卸载UI
    public void ReleaseMogoUI()
    {

    }
    public void DestroyNewLoginUI()
    {
        IsNewLoginUILoaded = false;
        NewLoginUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_NewLoginUI);
        m_NewLoginUI = null;
    }
    #endregion
}