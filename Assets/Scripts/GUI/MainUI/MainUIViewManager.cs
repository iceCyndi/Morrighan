
using UnityEngine;
using System;
using System.Collections;
using Mogo.Util;
//using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;


public class BloodColorSriteName
{

}

public class MainUIViewManager : MogoUIBehaviour
{
    private static MainUIViewManager m_instance;
    public static MainUIViewManager Instance { get { return MainUIViewManager.m_instance; } }

    private float m_fNormalAttackHover = 0f;
    private bool m_bNormalAttackDown = false;
    private bool m_bChargingStart = false;
    private GameObject m_goSelfAttack;
    private GameObject m_goCancelManaged;

    private UIFilledSprite m_fsNormalAttackCD;
    private float m_fNormalAttackPowerTime = 1.5f;

    private float m_fSpellOneCD = 0;
    private float m_fSpellTwoCD = 0;
    private float m_fSpellThreeCD = 0;
    private float m_fHPBottleCD = 0;
    private float m_fSpriteSkillCD = 0;

    private float m_fSpellOneCnt = 0;
    private float m_fSpellTwoCnt = 0;
    private float m_fSpellThreeCnt = 0;
    private float m_fHPBottleCnt = 0;
    private float m_fSpriteSkillCnt = 0;

    private UILabel m_lblPlayerBlood;
    private UILabel m_lblSelfAttack;
    private UILabel m_lblInstanceCountDown;

    private bool isLockOut = false;
    public bool LockOut
    {
        get { return isLockOut;  }
        set { isLockOut = value;  }
    }

    bool m_bIsNeedSecondAnim = false;
    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();



    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.FindChild(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
        }
    }
    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.FindChild(UIName).GetComponentsInChildren<UISlicedSprite>(true);
    }


    #region 右下角战斗角色

    #region 战斗按钮CD
    public float HpBottleCD
    {
        get { return m_fHPBottleCD; }
        set { m_fHPBottleCD = 1000 * value; }
    }

    public void SpellOneCD(int cd)
    {
        m_fSpellOneCD = cd;
    }
    public void SpellTwoCD(int cd)
    {
        m_fSpellOneCD = cd;
    }
    public void SpellThreeCD(int cd)
    {
        m_fSpellOneCD = cd;
    }
    public void SpriteSkillCD(int cd)
    {
        m_fSpriteSkillCD = cd;
    }


    #endregion


    #region 暴气
    #endregion


    #region 技能1按钮
    public void SetAffectImage(string imageName) { }
    UIFilledSprite m_fsAffectCD;
    public void SetAffectCD(int cd) { }
    private UISprite spAffectCDDown1;
    private UISprite spAffectCDDown10;
    private void SetAffectCountDown(int down) { }
    #endregion


    #region 技能2按钮
    public void SetOutputImage(string imageName)
    {
        m_myTransform.FindChild(m_widgetToFullName["OutputBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["OutputDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["OutputUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["OutputFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["OutputBG"], imageName);
        SetUITexture(m_widgetToFullName["OutputFG"], "bb_daojugeguangzhe");
    }
    UIFilledSprite m_fsOutputCD;
    public void SetOutputCD(int cd)
    {
        UISprite spCD = m_myTransform.FindChild(m_widgetToFullName["OutputCD"]).GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsOutputCD.fillAmount = (float)cd / 100.0f;
    }

    private UISprite spOutputCDDown1;
    private UISprite spOutputCDDown10;
    private void SetOutputCountDown(int down) 
    {
        if (spOutputCDDown1 == null || spOutputCDDown10 == null)
        {
            spOutputCDDown1 = FindTransform("OutputCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spOutputCDDown10 = FindTransform("OutputCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }

        if (down > 0)
        {
            if (down >= 10)
            {
                spOutputCDDown10.spriteName = "red_" + down / 10;
                spOutputCDDown10.MakePixelPerfect();
                spOutputCDDown10.gameObject.SetActive(true);

                spOutputCDDown1.spriteName = "red_" + down % 10;
                spOutputCDDown1.MakePixelPerfect();
                spOutputCDDown1.transform.localPosition = new Vector3(10, spOutputCDDown1.transform.localPosition.y, spOutputCDDown1.transform.localPosition.z);
                spOutputCDDown1.gameObject.SetActive(true);

            }
            else
            {
                spOutputCDDown1.spriteName = "red_" + down;
                spOutputCDDown1.MakePixelPerfect();
                spOutputCDDown1.transform.localPosition = new Vector3(0, spOutputCDDown1.transform.localPosition.y, spOutputCDDown1.transform.localPosition.z);
                spOutputCDDown1.gameObject.SetActive(true);
                spOutputCDDown10.gameObject.SetActive(false);
            }
        }
        else
        {
            spOutputCDDown1.gameObject.SetActive(false);
            spOutputCDDown10.gameObject.SetActive(false);
        }
    }
    #endregion


    #region 技能3按钮
    public void SetMoveImage(string imageName)
    {
        m_myTransform.FindChild(m_widgetToFullName["MoveBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["MoveDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["MoveUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["MoveFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["MoveBG"], imageName);
        SetUITexture(m_widgetToFullName["MoveFG"], "bb_daojugeguangzhe");
    }
    UIFilledSprite m_fsMoveCD;
    public void SetMoveCD(int cd)
    {
        UISprite spCD = m_myTransform.FindChild(m_widgetToFullName["MoveCD"]).GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsMoveCD.fillAmount = (float)cd / 100.0f;
    }

    private UISprite spMoveCDDown1;
    private UISprite spMoveCDDown10;
    private void SetMoveCountDown(int down)
    {
        if (spMoveCDDown1 == null || spMoveCDDown10 == null)
        {
            spMoveCDDown1 = FindTransform("OutputCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spMoveCDDown10 = FindTransform("OutputCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }

        if (down > 0)
        {
            if (down >= 10)
            {
                spMoveCDDown10.spriteName = "red_" + down / 10;
                spMoveCDDown10.MakePixelPerfect();
                spMoveCDDown10.gameObject.SetActive(true);

                spMoveCDDown1.spriteName = "red_" + down % 10;
                spMoveCDDown1.MakePixelPerfect();
                spMoveCDDown1.transform.localPosition = new Vector3(10, spMoveCDDown1.transform.localPosition.y, spMoveCDDown1.transform.localPosition.z);
                spMoveCDDown1.gameObject.SetActive(true);

            }
            else
            {
                spMoveCDDown1.spriteName = "red_" + down;
                spMoveCDDown1.MakePixelPerfect();
                spMoveCDDown1.transform.localPosition = new Vector3(0, spMoveCDDown1.transform.localPosition.y, spMoveCDDown1.transform.localPosition.z);
                spMoveCDDown1.gameObject.SetActive(true);
                spMoveCDDown10.gameObject.SetActive(false);
            }
        }
        else
        {
            spMoveCDDown1.gameObject.SetActive(false);
            spMoveCDDown10.gameObject.SetActive(false);
        }
    }
    #endregion


    #region 普通攻击按钮
    public void SetNormalAttackIconByID(int id)
    {
        string imgName = "";
        switch (id)
        {
            case 1:
                imgName = "dajian";
                break;
            case 2:
                imgName = "quantao";
                break;
            case 3:
                imgName = "bishou";
                break;
            case 4:
                imgName = "yueren";
                break;
            case 5:
                imgName = "fazhan";
                break;
            case 6:
                imgName = "shanzi";
                break;
            case 7:
                imgName = "gongjian";
                break;
            case 8:
                imgName = "nupao";
                break;
        }
        SetNormalAttackIcon(imgName);
    }
    public void SetNormalAttackIcon(string imgName) 
    {
        m_myTransform.FindChild(m_widgetToFullName["NormalAttackBGDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["NormalAttackBGUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.FindChild(m_widgetToFullName["NormalAttackBGDown"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = "jinengdiquan_2";
        m_myTransform.FindChild(m_widgetToFullName["NormalAttackBGUp"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }
    public void SetNormalAttackImage(string imageName) 
    {
        SetUITexture(m_widgetToFullName["NormalAttackUp"], imageName);
    }
    public void SetNormalAttackCD(int cd)
    {
        m_fsNormalAttackCD.fillAmount = (float)cd / 100.0f;
    }
    #endregion 

    #endregion

    private GameObject m_goMainUIBottom;
    private GameObject m_goMainUIBottomLeft;
    private GameObject m_goMainUIBottomRight;
    private GameObject m_goMainUITop;
    private GameObject m_goMainUITopLeft;
    private GameObject m_goMainUITopRight;
    private GameObject m_goController;

    private GameObject m_goNormalAttackBtn;
    private GameObject m_goSkill0Btn;
    private GameObject m_goSkill1Btn;
    private GameObject m_goSkill2Btn;
    private GameObject m_goBottle;
    private GameObject m_goSpriteSkillBtn;
    private GameObject m_goCommunityBtn;

    private GameObject m_goMainUIPlayerExpList;
    private GameObject m_goGOMainUIPlayerExpListFG;
    private UILabel m_lblMainUIPlayerExpListNum;
    private List<UISprite> m_listMainUIPlayerExpFG = new List<UISprite>();

    #region 框架
    public override void CallWhenLoadResources()
    {
        m_instance = this;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        FillFullNameData(m_myTransform);

        m_goMainUIBottom = m_myTransform.FindChild(m_widgetToFullName["Bottom"]).gameObject;
        m_goMainUIBottomLeft = m_myTransform.FindChild(m_widgetToFullName["BottomLeft"]).gameObject;
        m_goMainUIBottomRight = m_myTransform.FindChild(m_widgetToFullName["BottomRight"]).gameObject;
        m_goMainUITop = m_myTransform.FindChild(m_widgetToFullName["Top"]).gameObject;
        m_goMainUITopLeft = m_myTransform.FindChild(m_widgetToFullName["TopRight"]).gameObject;
        m_goMainUITopRight = m_myTransform.FindChild(m_widgetToFullName["TopRight"]).gameObject;
       // m_goController = m_myTransform.FindChild("BottomLeft/Controller").gameObject;
        m_goController = m_myTransform.FindChild(m_widgetToFullName["Controller"]).gameObject;
        m_goController.AddComponent<ControlStick>();

        Camera camera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_goMainUIBottom.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUIBottomLeft.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUIBottomRight.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITop.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITopLeft.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITopRight.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goController.GetComponentsInChildren<ControlStick>(true)[0].RelatedCamera = camera;

        m_myTransform.FindChild(m_widgetToFullName["MainUIBG"]).gameObject.AddComponent<TouchControll>();

        m_fsNormalAttackCD = m_myTransform.FindChild(m_widgetToFullName["NormalAttackCD"]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        m_fsAffectCD = m_myTransform.FindChild(m_widgetToFullName["AffectCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsAffectCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_fsOutputCD = m_myTransform.FindChild(m_widgetToFullName["OutputCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsOutputCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_fsMoveCD = m_myTransform.FindChild(m_widgetToFullName["MoveCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsMoveCD.color = new Color(0.5f, 0.5f, 0.5f);

        Initialize();
       // MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
        m_myGameObject.SetActive(false);

    }
    UIAnchor[] temp;
    public override void CallWhenCreate()
    {
        if (SystemConfig.Instance.IsDragMove)
        {
            MogoUIManager.Instance.ChangeSettingToControlStick();
        }
        else
        {
            MogoUIManager.Instance.ChangeSettingToTouch();
        }
        temp = m_myTransform.GetComponentsInChildren<UIAnchor>(true);
    }
    public override void CallWhenShow()
    {
        //ShowSpecailSkillFX(true);
        if (temp.Length > 0)
        {
            TimerHeap.AddTimer(2000, 0, () =>
            {
                for (int  i = 0; i < temp.Length; ++i)
                {
                    temp[i].enabled = false;
                }
            });
        }
    }
    #endregion

    #region 事件
    void Initialize()
    {
        MainUILogicManager.Instance.Initialize();
        m_uiLoginManager = MainUILogicManager.Instance;

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACKDOWN, OnNormalAttackDown);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACTUP, OnNormalAttackUp);

        MainUIDict.ButtonTypeToEventUp.Add("Special", MainUIDict.MainUIEvent.SPECIALUP);
        MainUIDict.ButtonTypeToEventUp.Add("Move", MainUIDict.MainUIEvent.MOVEUP);
        MainUIDict.ButtonTypeToEventUp.Add("Affect", MainUIDict.MainUIEvent.AFFECTUP);
        MainUIDict.ButtonTypeToEventUp.Add("Output", MainUIDict.MainUIEvent.OUTPUTUP);
        MainUIDict.ButtonTypeToEventUp.Add("NormalAttack", MainUIDict.MainUIEvent.NORMALATTACTUP);
    }
    public void Release()
    {
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACKDOWN, OnNormalAttackDown);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACTUP, OnNormalAttackUp);

        MainUIDict.ButtonTypeToEventUp.Clear();
    }
    #endregion

    void OnNormalAttackDown()
    {
        if (!MainUILogicManager.Instance.IsAttackable)
            return;
        m_bNormalAttackDown = true;
        EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.NORMALATTACK);
    }
    void OnNormalAttackUp()
    {
        if (!MainUILogicManager.Instance.IsAttackable)
            return;

        if (m_fNormalAttackHover > m_fNormalAttackPowerTime)
        {
            m_bChargingStart = false;
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.POWERCHARGECOMPLETE);
        }
        else if (m_bChargingStart)
        {
            m_bChargingStart = false;
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.POWERCHARGEINTERRUPT);
        }

        m_bNormalAttackDown = false;
        m_fsNormalAttackCD.fillAmount = 0;
        m_fNormalAttackHover = 0;
    }

    public void SetControllStickEnable(bool isEnable)
    {
        m_myTransform.FindChild(m_widgetToFullName["Controller"]).GetComponentsInChildren<ControlStick>(true)[0].enabled = isEnable;
        m_myTransform.FindChild(m_widgetToFullName["Controller"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
        m_myTransform.FindChild(m_widgetToFullName["MainUIBG"]).GetComponentsInChildren<TouchControll>(true)[0].enabled = !isEnable;
        m_myTransform.FindChild(m_widgetToFullName["MainUIBG"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = !isEnable;

    }
}
