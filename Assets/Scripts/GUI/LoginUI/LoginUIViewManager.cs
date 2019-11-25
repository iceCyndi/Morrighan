using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Game;
using Mogo.Util;

public class LoginUIViewManager : MonoBehaviour
{
    public Action LOGINUILOGINUP;
    public Action LOGINUISIGNUPUP;
    public Action NOTICE_BTN_CLICK;
    public Action ChooseServerUp;
    public Action OnShown;
    public Action OnSwitchAccount;
    private static LoginUIViewManager m_instance;
    public static LoginUIViewManager Instance
    {
        get
        {
            return LoginUIViewManager.m_instance;
        }
    }
    private Transform m_myTransform;
    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    GameObject m_chooseCharacterUI;
    UIInput m_userName;
    UIInput m_password;

    UILabel m_lblServerName;
    UITexture m_textLogoFG;
    UITexture m_textLogoBG;

    UITexture m_texLogoFlashObj;
    UISprite m_spResCtrl;
    UIAtlas m_atlasCanRelease;
    public void AddWidgetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }
    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";
        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;
            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }
        return fullName;
    }
    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWidgetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }
    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.FindChild(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[1].transform.localScale = new Vector3(18, 18, 18);
        }
    }
    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.FindChild(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }
    void OnLoginUILoginUp()
    {
        if (LOGINUILOGINUP != null)
            LOGINUILOGINUP();
    }
    void OnLoginUISignUpUp()
    {
        if (LOGINUISIGNUPUP != null)
            LOGINUISIGNUPUP();
    }
    void OnSwitchUp()
    {
        if (ChooseServerUp != null)
            ChooseServerUp();
    }
    void Awake()
    {
        m_instance = this;
        Initialize();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_userName = m_myTransform.FindChild(m_widgetToFullName["LoginUIUserNameInput"]).GetComponentsInChildren<UIInput>(true)[0];
        m_password = m_myTransform.FindChild(m_widgetToFullName["LoginUIUserPasswordInput"]).GetComponentsInChildren<UIInput>(true)[0];
        m_lblServerName = m_myTransform.FindChild(m_widgetToFullName["RecommendServerUIServerName"]).GetComponentsInChildren<UILabel>(true)[0];

        m_myTransform.FindChild(m_widgetToFullName["RecommedServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSwitchUp;
        m_textLogoBG = m_myTransform.FindChild(m_widgetToFullName["LoginUILogoBG"]).GetComponentsInChildren<UITexture>(true)[0];
        m_textLogoFG = m_myTransform.FindChild(m_widgetToFullName["LoginUILogoFG"]).GetComponentsInChildren<UITexture>(true)[0];
        m_texLogoFlashObj = m_myTransform.FindChild(m_widgetToFullName["LoginUILogoFlashTex"]).GetComponentsInChildren<UITexture>(true)[0];

        m_spResCtrl = m_myTransform.FindChild(m_widgetToFullName["LoginUIResCtrl"]).GetComponentsInChildren<UISprite>(true)[0];

        m_atlasCanRelease = m_spResCtrl.atlas;
    }
    void OnEnable()
    {
        if (OnShown != null)
            OnShown();
    }
    void Initialize()
    {
        LoginUILogicManager.Instance.Initialize();

        LoginUIDict.ButtonTypeToEventUp.Add("LoginUILogin", OnLoginUILoginUp);
        LoginUIDict.ButtonTypeToEventUp.Add("LoginUISignUp", OnLoginUISignUpUp);
        LoginUIDict.ButtonTypeToEventUp.Add("LoginUINotice", OnLoginUINotice);
    }
    private void OnLoginUINotice()
    {
        if (NOTICE_BTN_CLICK != null)
            NOTICE_BTN_CLICK();
    }
    public string GetUserName()
    {
        return m_userName.text;
    }
    public string GetPassword()
    {
        return m_password.text;
    }
    public void SetUserName(string name)
    {
        m_userName.text = name;
    }
    public void SetPassword(string password)
    {
        m_password.text = password;
        m_password.label.password = true;
    }
    public void Release()
    {
        LoginUILogicManager.Instance.Release();
        m_myTransform.FindChild(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnSwitchUp;
        LoginUIDict.ButtonTypeToEventUp.Clear();
    }
    public bool IsSaveUserName()
    {
        return true;
    }
    public bool IsSaveUserPassword()
    {
        return true;
    }
    public bool IsAutoLogin()
    {
        return false;
    }
    public void CheckSaveUserName(bool check)
    { }
    public void CheckSavePassword(bool check)
    { }
    public void CheckAutoLogin(bool check)
    { }
    public void SetServerName(string name)
    {
        m_lblServerName.text = name;
    }
    public void SwitchToAndroidMode()
    { }
    public void ReleaseUIAndResources()
    {
        this.gameObject.SetActive(false);
        //MogoUIManager.Instance.DestroyLoginUI();
    }
    void OnDisable()
    { }
}
