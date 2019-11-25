using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewLoginUILogicManager 
{
    private const int LANGUAGE_DELETE = 708;
    private Dictionary<int, EtyAvatar> m_avatarList = new Dictionary<int, EtyAvatar>();
    private MogoLoginCamera m_loginCamera;
    private Transform m_defaultCameraSlot;
    private Dictionary<int, Transform> m_cameraSlots = new Dictionary<int, Transform>();
    public Shader PlayerShader;
    public Shader FakeLightShader;

    protected Dictionary<int, Animator> m_Animators = new Dictionary<int, Animator>();
    protected Dictionary<int, uint> m_fxTimes = new Dictionary<int, uint>();


    private int m_selectedCharacterId = 0;
    public EtyAvatar m_lastSelectedCharacter;
    private Action<int> m_onChooseServerGridUp;
    private Action m_onChooseServerUIBackBtnUp;

    private static NewLoginUILogicManager m_instance;
    public static NewLoginUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewLoginUILogicManager();
            }
            return NewLoginUILogicManager.m_instance;
        }
    }
    public static class NewLoginUIEvent
    {
        public const string CHOOSECHARACTERGRIDUP = "NewLoginUI_ChooseCharacterGridUp";
        public const string CHOOSESERVERGRIDUP = "NewLoginUI_ChooseServerGridUp";
    }
    public void Initialize()
    {
        EventDispatcher.AddEventListener<int>(NewLoginUIEvent.CHOOSECHARACTERGRIDUP, OnChooseCharacterGridUp);
        EventDispatcher.AddEventListener<int>(NewLoginUIEvent.CHOOSESERVERGRIDUP, OnChooseServerGridUp);
        EventDispatcher.AddEventListener<GameObject>("NewLoginUIViewManager.CreateCharacterChooseModel", OnCreateCharacterSelected);

        NewLoginUIViewManager.Instance.ENTERGAMEBTNUP += OnEnterGameBtnUp;
        NewLoginUIViewManager.Instance.DELETECHARCTERBTNUP += OnDeleteCharacterBtnUp;
        NewLoginUIViewManager.Instance.ChooseCharacterUIServerBtnUp += OnChooseCharacterUIServerBtnUp;
        NewLoginUIViewManager.Instance.ChooseServerUIBackBtnUp += ChooseServerUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIEnterBtnUp += CreateCharacterDetailUIEnterBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIBackBtnUp += OnCreateCharacterDetailUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterUIBackBtnUp += OnCreateCharacterUIBackBtnUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUIJobIconUp += OnCreateCharacterDetailUIJobIconUp;
        NewLoginUIViewManager.Instance.CreateCharacterDetailUISwitch += OnCreateCharacterDetailUISwitch;

        string[] temp = SystemConfig.Instance.SelectedCharacter.Split(',');
        if (temp.Length > 1) 
        {
            int index = int.Parse(temp[1]);
            if (temp[0] == SystemConfig.Instance.Passport && index >= 0)
            {
                if (MogoWorld.theAccount.GetAvatarInfo(index) != null)
                {
                    m_selectedCharacterId = index;
                }
            }
            else
            {
                m_selectedCharacterId = 0;
            }
        }
        else
        {
            m_selectedCharacterId = 0;
        }
        NewLoginUIViewManager.Instance.SetCharacterGridDown(m_selectedCharacterId);
    }
    public void Release()
    {

    }

    #region 创建角色
    public int m_currentPos = -1;
    void OnCreateCharacterSelected(GameObject go)
    {
        OnCreateCharacterSelected(go, false);
    }
    void OnCreateCharacterSelected(GameObject go, bool needFadeIn)
    {
 
    }
    void CreateCharacterDetailUIEnterBtnUp() { }
    void OnCreateCharacterDetailUIBackBtnUp() { }
    void OnCreateCharacterUIBackBtnUp() { }
    private void OnCreateCharacterDetailUISwitch(int dir)
    { }
    void OnCreateCharacterDetailUIJobIconUp(int pos)
    { }
    void OnCreateCharacterDetailUIJobIconUp(int pos, bool needFadeIn)
    { }
    public void LoadCreateCharacter(Action callback)
    {
        AssetCacheMgr.GetResource("PlayerShader.shader", (obj) =>
        {
            AssetCacheMgr.GetResource("MogoFakeLight.shader", (obj1) =>
            {
                FakeLightShader = (Shader)obj1;
                PlayerShader = (Shader)obj;
                LoadCamera();
                MogoWorld.inCity = false;

                LoadCharacters(() =>
                {
                    foreach (var item in m_avatarList)
                    {
                        item.Value.Init();
                        item.Value.SetCreateMode();
                    }

                    var avatarList = m_avatarList.Values.ToList();
                    var counter = 0;
                    for (int i = 0; i < avatarList.Count; i++)
                    {
                        var item = avatarList[i];
                        item.gameObject.transform.position = item.CreatePosition;
                        item.Equip(item.equipList, () =>
                        {
                            counter++;
                            if (counter == m_avatarList.Count * 2)
                                if (callback != null)
                                    callback();
                        });
                        int vocation = item.vocation;
                        item.Equip(item.weapon, () =>
                        {
                            //if (vocation == (int)Vocation.Mage && m_)
                        });
                    }
                });
            });
        });
    }
    #endregion

    #region 选择角色
    public void LoadChooseCharacterSceneAfterDelete()
    { }
    public void FillChooseCharacterGridData(List<ChooseCharacterGridData> list) 
    { }
    public void UpdateSelectedAvatarModel()
    { }
    void OnChooseCharacterGridUp(int id)
    { }
    void OnChooseCharacterGridUp(int id, bool forceUpdate)
    { }
    void OnEnterGameBtnUp()
    { }
    void OnDeleteCharacterBtnUp()
    { }
    void OnChooseCharacterUIServerBtnUp()
    { }
    void LoadCamera()
    {
        var camera = Camera.mainCamera;
        m_loginCamera = camera.GetComponent<MogoLoginCamera>();
        if (m_loginCamera == null)
        {
            m_loginCamera = camera.gameObject.AddComponent<MogoLoginCamera>();
            camera.transform.parent = null;
            var go = new GameObject();
            go.transform.position = camera.transform.position;
            go.transform.rotation = camera.transform.rotation;
            m_defaultCameraSlot = go.transform;
            for (int i = 1; i <= 4; i++)
            {
                var tran = camera.transform.FindChild(i.ToString());
                tran.parent = null;
                m_cameraSlots[i] = tran;
            }
        }
        EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, 10003);
    }
    public void LoadChooseCharacter(int vocation, Action<EtyAvatar> loaded)
    { }
    public void LoadChooseCharacter()
    { }
    #endregion

    #region 选择服务器
    private bool m_isConnectSuccess;
    public void ShowChooseServerUIFormCreateChr(bool isConnectSuccess = true)
    { }
    public void ShowChooseServerUIFormLogin()
    { }
    public void ShowChooseServerUI(bool isConnectSuccess = true)
    { }
    void OnChooseServerGridUp(int id)
    { }
    void ChooseServerUIBackBtnUp()
    { }
    #endregion

    #region 模型管理
    private void ClearChooseCharacterModel()
    {
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(0, new CharacterInfoData());
        foreach (var item in m_avatarList)
        {
            item.Value.Show();
            if (item.Value.gameObject)
                item.Value.gameObject.transform.position = ci.Location;
            item.Value.Hide();
            item.Value.Unfocus();
        }
    }
    private void LoadCharacters(Action loaded)
    {
        LoadCharacter((int)Vocation.Warrior, () => 
        {
            LoadCharacter((int)Vocation.Assassin, () =>
            {
                LoadCharacter((int)Vocation.Archer, () =>
                {
                    LoadCharacter((int)Vocation.Mage, () =>
                    {
                        if (loaded != null)
                            loaded();
                    });

                });
            });
        });
    }
    private void LoadCharacter(int vocation, Action loaded)
    {
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(vocation, new CharacterInfoData());
        var ai = AvatarModelData.dataMap.GetValueOrDefault(vocation, new AvatarModelData());
        CreateCharacterModel(ai, vocation, ci, loaded);
    }
    private void CreateCharacterModel(AvatarModelData ai, int vocation, CharacterInfoData ci, Action loaded)
    {
        if (m_avatarList.ContainsKey(vocation) && loaded != null)
        {
            //if (m_avatarList[vocation].act)
        }
        AssetCacheMgr.GetInstanceAutoRelease(ai.prefabName, (prefab, id, go) =>
        {
            var ety = new EtyAvatar();
            ety.vocation  = vocation;
            ety.equipList = ci.EquipList;
            ety.weapon = ci.Weapon;
            ety.CreatePosition = ci.Location;
            var avatar = (go as GameObject);
            ety.gameObject = avatar;
            avatar.name = vocation.ToString();
            var cc = avatar.collider as CharacterController;
            cc.radius = 0.5f;
            ety.animator = avatar.GetComponent<Animator>();
            ety.animator.applyRootMotion = false;
            ety.PlayerShader = PlayerShader;
            ety.FakeLightShader = FakeLightShader;


            #region 动作
            if (m_Animators.ContainsKey(vocation))
                m_Animators[vocation] = avatar.GetComponent<Animator>();
            else
                m_Animators.Add(vocation, avatar.GetComponent<Animator>());
            #endregion

            avatar.transform.position = ci.Location;
            avatar.transform.localScale = Vector3.one;

            ety.actorParent = avatar.AddComponent<ActorParent>();
            ety.actorParent.SetNakedEquid(ai.nakedEquipList);
            ety.InitEquip();

            if (m_avatarList.ContainsKey(vocation))
            {
                //m_avatarList[vocation].
                AssetCacheMgr.ReleaseInstance(m_avatarList[vocation].gameObject);
            }
            ety.Hide();
            m_avatarList[vocation] = ety;
            AssetCacheMgr.GetResource(ci.controller, 
            (obj) => 
            {
                var controller = obj as RuntimeAnimatorController;
                if (ety.animator)
                    ety.animator.runtimeAnimatorController = controller;
                if (loaded != null)
                    loaded();
            });
        });

    }
    public void ReleaseCharacter()
    { }
    #endregion
}

public class EtyAvatar
{
    public int vocation { get; set; }
    public List<int> equipList { get; set; }
    public int weapon { get; set; }
    public ActorParent actorParent { get; set; }

    public Animator animator { get; set; }
    public GameObject gameObject { get; set; }
    public Vector3 CreatePosition { get; set; }
    private bool m_isShown = true;
    public SkinnedMeshRenderer smr;
    public Shader PlayerShader;
    public Shader FakeLightShader;
    public void Init()
    {
        Show();
        Unfocus();
    }
    public void Release()
    { }
    public void ResetFade()
    { }
    public void FadeIn(bool noDelay = false)
    { }
    public void FakeOut(bool noDelay = false)
    { }
    public void Focus()
    { }
    public void Unfocus()
    { }
    public void Show()
    { }
    public void Hide()
    { }
    public void RemoveOld()
    { }
    public void RemoveAll()
    { }
    public void InitEquip()
    {
        try
        {
            if (actorParent)
                actorParent.InitNakedEquid();
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }
    public void Equip(List<int> equips, Action onDone = null)
    {
        try
        {
            if (actorParent)
                actorParent.Equip(equips, onDone);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }
    public void Equip(int equipId, Action onDone = null)
    {
        try
        {
            if (actorParent && equipId != 0 && EquipData.dataMap.ContainsKey(equipId))
                actorParent.Equip(equipId, onDone);
            else
            {
                if (onDone != null)
                    onDone();
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }
    public void SetChooseMode()
    {
        if (animator)
        {
            animator.SetInteger("Action", -1);
            actorParent.AddCallbackInFrames(() =>
            {
                if (animator)
                    animator.SetInteger("Action", 0);
            });
        }
    }
    public void SetCreateMode()
    {
        if (animator)
        {
            animator.SetInteger("Action", 34);
            actorParent.AddCallbackInFrames(() =>
            {
                if (animator)
                    animator.SetInteger("Action", 0);
            });
        }
    }
}