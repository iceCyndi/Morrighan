using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;

public class ChooseCharacterGridData
{
    public string name;
    public string level;
    public string defaultText;
    public string headImg;
}

public class JobAttrGridData
{
    public string attrName;
    public int level;
}

public class ChooseServerGridData
{
    public string serverName;
    public ServerType serverStatus;
}

public class NewLoginUIViewManager : MogoUIBehaviour
{
    private static NewLoginUIViewManager m_instance;
    public static NewLoginUIViewManager Instance
    {
        get
        {
            return NewLoginUIViewManager.m_instance;
        }
    }
    #region 属性

    private UILabel m_lblCreateCharacterText;
    private UILabel m_lblJobName;
    private UILabel m_lblJobInfo;
    private UIInput m_lblCharacterNameInput;
    private UILabel m_lblRecommendServerName;
    private GameObject m_goCreateCharacterDetailUIEnterBtn;
    private UILabel m_lblEnter;
    private UISprite m_spCreateCharacterDetailUIEnterBtnBGUp;


    private GameObject m_goRandomBtn;


    private GameObject m_goChooseCharacterGridList;
    private GameObject m_goJobAttrList;
    private GameObject m_goChooseServerGridList;

    private GameObject m_goCurrentUI;
    private GameObject m_goChooseServerUI;
    private GameObject m_goChooseCharacterUI;
    private GameObject m_goCreateCharacterDetailUI;
    private GameObject m_goCreateCharacterUI;
    private GameObject m_goRecommendServerUI;
    private GameObject m_goCreateCharacterUIBackBtn;
    private GameObject m_goChooseServerGridPageList;

    private MogoTwoStatusButton m_MTBChooseCharacterUIServer;
    private MogoTwoStatusButton m_MTBCreateCharacterUIServer;
    private MogoTwoStatusButton m_MTBLatelyLog0;
    private MogoTwoStatusButton m_MTBLatelyLog1;

    private List<GameObject> m_listChooseServerGridPage = new List<GameObject>();
    private Camera m_camChooseServerGridList;
    private MyDragableCamera m_dragableCameraChooseServerGridList;
    private int m_iCurrentServerGridPage = 1;
    private List<GameObject> m_listChooseCharacterGrid = new List<GameObject>();
    private List<GameObject> m_listJobAttr = new List<GameObject>();
    private List<GameObject> m_listChooseServerGrid = new List<GameObject>();

    private GameObject m_goCreateCharacterDetailUIJobIconList;

    const int CHOOSECHARACTERGRIDHEIGHT = 110;
    const int JOBATTRGRIDHEIGHT = 70;
    const int CHOOSESERVERGRIDHEIGHT = 85;
    const int CHOOSESERVERGRIDWIDTH = 410;
    const int CHOOSESERVERGRIDPAGEWIDTH = 1230;


    public Action ENTERGAMEBTNUP;
    public Action DELETECHARCTERBTNUP;
    public Action ChooseCharacterUIServerBtnUp;
    public Action ChooseServerUIBackBtnUp;
    public Action CreateCharacterDetailUIEnterBtnUp;
    public Action CreateCharacterDetailUIBackBtnUp;
    public Action CreateCharacterUIBackBtnUp;

    public Action<int> CreateCharacterDetailUIJobIconUp;

    public Action<int> CreateCharacterDetailUISwitch;

    private int m_iChooseServerPageNum = 0;
    private bool inited = false;
    UISprite m_spResCtrl;
    UIAtlas m_atlasCanRelease;

    #endregion


    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Camera cam = GameObject.Find("MogoMainUI").transform.FindChild("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUITopRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUILeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;

        //m_camChooseServerGridList = m_myTransform.FindChild(m_widgetToFullName["ChooseServerUIServerGridListCamera"]).GetComponentsInChildren<UILabel>(true)[0];
       // m_camChooseServerGridList = m_myTransform.FindChild(m_widgetToFullName["ChooseServerUIServerGridListCamera"]).GetComponentsInChildren<UILabel>(true)[0];

        for (int i = 0; i < 4; ++i)
        {
            AssetCacheMgr.GetUIInstance("ChooseCharacterUIGrid.prefab", (prefab, id, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_goChooseCharacterGridList.transform;
                obj.transform.localPosition = new Vector3(0, -m_listChooseCharacterGrid.Count * CHOOSECHARACTERGRIDHEIGHT, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.AddComponent<ChooseCharacterUIGrid>().Id = m_listChooseCharacterGrid.Count;
                m_goChooseCharacterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);
                m_listChooseCharacterGrid.Add(obj);
                if (m_listChooseCharacterGrid.Count == 4)
                {
                    TruelyFillChooseCharacterGridData();
                    TruelyFillChooseCharacterGridData();
                }
            });
        }

        for (int i = 0; i < 3; i++)
        {
            AssetCacheMgr.GetUIInstance("CreateCharacterDetailUIJobAttr.prefab", (prefab, id, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_goJobAttrList.transform;
                obj.transform.localPosition = new Vector3(0, -m_listJobAttr.Count * JOBATTRGRIDHEIGHT, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);

                obj.AddComponent<CreateCharacterDetailUIJobAttr>();
                m_listJobAttr.Add(obj);
            });
        }
        Initialize();
        inited = true;
    }
    void Initialize()
    {
        NewLoginUILogicManager.Instance.Initialize();

        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUIServerBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterUIServerBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterUIServertBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUIDeleteBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterUIDeleteBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["ChooseCharacterUICommunityBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterCommunityBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIBackBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIRandomBtnUp;;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUI"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIEnterBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUILatelyLogBtn0"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseServertUILatelyLogBtn0Up;
        m_myTransform.FindChild(m_widgetToFullName["ChooseServerUILatelyLogBtn1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseServertUILatelyLogBtn1Up;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterUIBackBtnUp;
        m_myTransform.FindChild(m_widgetToFullName["RecommendServerUIEnter"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRecommendServerEnterUp;
        m_myTransform.FindChild(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRecommendServerSwitchUp;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIJobIcon1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon1Up;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIJobIcon2"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon2Up;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIJobIcon3"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon3Up;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIJobIcon4"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon4Up;
        m_myTransform.FindChild(m_widgetToFullName["CreateCharacterDetailUIRightDragArea"]).GetComponentsInChildren<MogoButton>(true)[0].dragOverHandler += OnCreateCharacterDetailUISwitch;
          

    }
    private void OnCreateCharacterDetailUISwitch(Vector2 vec)
    { }
    public void Release()
    {
    }
    void OnCreateCharacterUIServerBtnUp()
    { }
    void OnChooseCharacterUIEnterBtnUp()
    { }
    void OnChooseCharacterUIDeleteBtnUp()
    { }
    void OnChooseCharacterUIServertBtnUp()
    { }
    void OnChooseCharacterCommunityBtnUp()
    { }
    void OnCreateCharacterDetailUIBackBtnUp()
    { }
    void OnCreateCharacterDetailUIRandomBtnUp()
    { }
    void OnCreateCharacterDetailUIEnterBtnUp()
    { }
    void OnChooseServerUIBackBtnUp()
    { }
    void OnChooseServertUILatelyLogBtn0Up()
    { }
    void OnChooseServertUILatelyLogBtn1Up()
    { }
    void OnCreateCharacterUIBackBtnUp()
    { }
    void OnRecommendServerEnterUp()
    { }
    void OnRecommendServerSwitchUp()
    { }
    void OnCreateCharacterDetailUIJobIcon1Up()
    { }
    void OnCreateCharacterDetailUIJobIcon2Up()
    { }
    void OnCreateCharacterDetailUIJobIcon3Up()
    { }
    void OnCreateCharacterDetailUIJobIcon4Up()
    { }
    private void TruelyFillChooseCharacterGridData()
    { }
    List<ChooseCharacterGridData> m_listCCGD = new List<ChooseCharacterGridData>();
    public void FillChooseCharacterGridData()
    { }
    public void FillJobAttrGridData(List<JobAttrGridData> list) { }
    int m_iCurrentGridDown = 0;
    private void TruelySetCharacterGridDown()
    {
        m_goChooseCharacterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(m_iCurrentGridDown);
    }
    public void SetCharacterGridDown(int gridId)
    { }
    public void SetCreateCharacterJobDetailName(string name)
    { }
    public void SetCreateCharacterJobDetailInfo(string info)
    { }
    public string GetCharacterNameInputText()
    {
        return string.Empty;
    }
    public void SetCharacterNameInputText(string text)
    { }
    public void SetChooseCharacterServerBtnName(string name)
    { }
    public void SetCreateCharacterServerBtnName(string name)
    { }
    public void SetRecommendServerName(string name)
    { }
    public void AddChooseServerGrid(ChooseServerGridData cd)
    { }
    public void ShowLatelyServer(string name, int id, bool isShow)
    { }
    public void ClearServerList()
    { }
    public void ShowCreateCharacterUIBackBtn(bool isShow)
    { }
    public void ShowChooseCharacterUI()
    { }
    public void ShowChooseServerUI()
    { }
    public void ShowCreateCharacterDetailUI()
    { }
    public void ShowCreateCharacterUI()
    { }
    public void ShowRecommendServerUI()
    { }
    public void SetEnterButtonLabel(string str, bool enable)
    { }
    public void ShowDiceAndName(bool isShow)
    { }
    public void SelectCreateCharacterDetailUIJobIcon(int pos)
    { }
    #region 界面打开和关闭
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!SystemSwitch.DestroyResource)
            return;
    }
    public void DestroyUIAndResources()
    {
        MogoUIManager.Instance.DestroyNewLoginUI();
        if (!SystemSwitch.DestroyResource)
            return;
    }
    void OnDisable()
    {
        DestroyUIAndResources();
    }
    #endregion
}