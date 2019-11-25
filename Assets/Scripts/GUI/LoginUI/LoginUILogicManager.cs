using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.RPC;
using Mogo.Util;


public class LoginUILogicManager
{
    public bool IsSaveUserName
    {
        get
        {
            return LoginUIViewManager.Instance.IsSaveUserName();
        }
        set 
        {
            if (true)
            {
                LoginUIViewManager.Instance.CheckSaveUserName(value);
            }
        }
    }
    public bool IsSavePassword
    {
        get
        {
            return LoginUIViewManager.Instance.IsSaveUserPassword();
        }
        set
        {
            LoginUIViewManager.Instance.CheckSavePassword(value);
        }
    }
    public bool IsAutoLogin
    {
        get
        {
            return LoginUIViewManager.Instance.IsAutoLogin();
        }
        set
        {
            if (true)
            {
                LoginUIViewManager.Instance.CheckAutoLogin(value);
            }
        }
    }
    public string UserName
    {
        get
        {
            return LoginUIViewManager.Instance.GetUserName();
        }
        set
        {
            LoginUIViewManager.Instance.SetUserName(value);
        }
    }
    public string Password
    {
        get
        {
            return LoginUIViewManager.Instance.GetPassword();
        }
        set
        {
            LoginUIViewManager.Instance.SetPassword(value);
        }
    }
    private static LoginUILogicManager m_instance;
    public static LoginUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LoginUILogicManager();
            }
            return LoginUILogicManager.m_instance;
        }
    }
    void OnLoginUILoginUp()
    {
        string passport = LoginUIViewManager.Instance.GetUserName();
        string password = LoginUIViewManager.Instance.GetPassword();
        MogoWorld.passport = passport;
        MogoWorld.password = password;

        MogoWorld.LoadCharacterScene();

        SystemConfig.Instance.Passport = passport;
        SystemConfig.Instance.Password = password;

        SystemConfig.SaveConfig();
        LoggerHelper.Debug("LoginUp");
    }
    void OnChooseServerUp()
    { }
    void OnLoginUISignUpUp()
    { }
    public void Initialize()
    {
        LoginUIViewManager.Instance.LOGINUILOGINUP += OnLoginUILoginUp;
        LoginUIViewManager.Instance.LOGINUISIGNUPUP += OnLoginUISignUpUp;
        LoginUIViewManager.Instance.NOTICE_BTN_CLICK += OnNoticeBtnClick;
        LoginUIViewManager.Instance.ChooseServerUp += OnChooseServerUp;
        LoginUIViewManager.Instance.OnShown = () =>
        {
       //     var serverInfo = SystemConfig.GetSelectedServerInfo();
       //     if (serverInfo != null)
       //         LoginUIViewManager.Instance.SetServerName(serverInfo.name);
        };
    }
    private void OnNoticeBtnClick()
    {
    }
    public void Release()
    {
        LoginUIViewManager.Instance.LOGINUILOGINUP -= OnLoginUILoginUp;
        LoginUIViewManager.Instance.LOGINUISIGNUPUP -= OnLoginUISignUpUp;
        LoginUIViewManager.Instance.NOTICE_BTN_CLICK -= OnNoticeBtnClick;
    }
}