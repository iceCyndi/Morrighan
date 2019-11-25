using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoGlobleLoadingUI : MonoBehaviour
{
    UISprite m_spLoadingLogo;
    UILabel m_lblLoadingTip;
    UILabel m_lblLoadingStatus;
    UISprite m_spLoadingBar;
    UISprite m_spLoadingAnim;

    Transform m_myTransform;
    Transform m_transLoadingBarTopLeft;
    Transform m_transLoadingBarBottomRight;

    UILabel title;
    UILabel tip;
    AudioSource audioSource;

    bool isFirstLoadingUI = false;
    int textOffset = 0;
    int defaultTextOffset = 999;
    string m_strLoadingTip;
    public string LoadingTip
    {
        get { return m_strLoadingTip; }
        set
        {
            m_strLoadingTip = value;
            if (m_strLoadingTip != null)
            {
                m_lblLoadingTip.text = m_strLoadingTip;
            }
        }
    }
    int m_iLoadingStatus;
    public int LoadingStatus
    {
        get { return m_iLoadingStatus; }
        set
        {
            m_iLoadingStatus = value;
            if (m_iLoadingStatus != null)
            {
                m_lblLoadingStatus.text = m_iLoadingStatus.ToString() + " % ";
                SetLoadingBarStatus(m_iLoadingStatus);
            }
        }
    }
    string m_strLoadingLogoImg;
    public string LoadingLogoImg
    {
        get { return m_strLoadingLogoImg; }
        set
        {
            m_strLoadingLogoImg = value;
            if (m_spLoadingLogo != null)
            {
                m_spLoadingLogo.spriteName = m_strLoadingLogoImg;
            }
        }
    }
    string m_strLoadingAnimImg;
    private UITexture m_loadingImgBg;
    private UILabel m_lblLoadingStatusTip;
    public string LoadingImgBg
    {
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            if (m_loadingImgBg == null) return;
            if (m_loadingImgBg.mainTexture != null)
                AssetCacheMgr.ReleaseResource(m_loadingImgBg.mainTexture);

            if (value == null)
            {
                LoggerHelper.Error("LoadingImgBg value == null");
            }
            AssetCacheMgr.GetUIResource(value, (obj) =>
            {
                if (obj == null)
                {

                }
                m_loadingImgBg.mainTexture = obj as Texture;
                AssetCacheMgr.GetResource("MogoUIBackGround.shader", (shader) =>
                {
                    m_loadingImgBg.shader = shader as Shader;
                });
            });
        }
    }
    public string LoadingAnimImg
    {
        get { return m_strLoadingAnimImg; }
        set
        {
            m_strLoadingAnimImg = value;
            if (m_spLoadingAnim != null)
            {
                m_spLoadingAnim.spriteName = m_strLoadingAnimImg;
            }
        }
    }
    public string StatusTip
    {
        set
        {
            if (m_lblLoadingStatusTip != null)
            {
                m_lblLoadingStatusTip.text = value;
            }
        }
    }

    void Awake()
    {
        m_myTransform = transform;
        m_transLoadingBarTopLeft = m_myTransform.FindChild("MogoGlobleLoadingUIBottom/MogoGlobleLoadingUILoading/MogoGlobleLoadingUILoadingBar/MogoGlobleLoadingUILoadingBarTopLeft").transform;
        m_transLoadingBarBottomRight = m_myTransform.FindChild("MogoGlobleLoadingUIBottom/MogoGlobleLoadingUILoading/MogoGlobleLoadingUILoadingBar/MogoGlobleLoadingUILoadingBarBottomRight").transform;

        m_loadingImgBg = m_myTransform.FindChild("MoogoGlobleLoadingBG").GetComponentsInChildren<UITexture>(true)[0];
        m_spLoadingLogo = m_myTransform.FindChild("MogoGlobleLoadingFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblLoadingTip = m_myTransform.FindChild("MogoGlobleLoadingTip").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLoadingStatus = m_myTransform.FindChild("MogoGlobleLoadingStatus").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLoadingStatusTip = m_myTransform.FindChild("MogoGlobleLoadingStatusTip").GetComponentsInChildren<UILabel>(true)[0];
        m_spLoadingBar = m_myTransform.FindChild("MogoGlobleLoadingUIBottom/MogoGlobleLoadingUILoading/MogoGlobleLoadingBar").GetComponentsInChildren<UISprite>(true)[0];
        m_spLoadingAnim = m_myTransform.FindChild("MogoGlobleLoadingUIBottom/MogoGlobleLoadingUILoading/MogoGloleLoadingAnim").GetComponentsInChildren<UISprite>(true)[0];
        title = m_myTransform.FindChild("MogoGlobleLoadingTitle").GetComponentsInChildren<UILabel>(true)[0];
        tip = m_myTransform.FindChild("MogoGlobleLoadingTip").GetComponentsInChildren<UILabel>(true)[0];
        isFirstLoadingUI = true;
        if (GetComponentsInChildren<AudioSource>(true) != null && GetComponentsInChildren<AudioSource>(true).Length > 0)
            audioSource = GetComponentsInChildren<AudioSource>(true)[0];
        if (title == null || tip == null || audioSource == null)
            isFirstLoadingUI = false;
        EventDispatcher.AddEventListener("MogoGlobleLoadingUIOnPress", MogoGlobleLoadingUIOnPress);

    }
    void Start()
    {
        if (m_strLoadingAnimImg != null)
            m_spLoadingAnim.spriteName = m_strLoadingAnimImg;
        if (m_strLoadingLogoImg != null)
            m_spLoadingLogo.spriteName = m_strLoadingLogoImg;
        if (m_strLoadingTip != null)
            m_lblLoadingTip.text = m_strLoadingTip;

        SetLoadingBarStatus(m_iLoadingStatus);
        if (isFirstLoadingUI)
            SetNextText();
    }
    void OnDestroy()
    {
        m_spLoadingLogo = null;
        m_lblLoadingTip = null;
        m_lblLoadingStatus = null;
        m_spLoadingBar = null;
        m_spLoadingAnim = null;
        m_myTransform = null;
        m_transLoadingBarTopLeft = null;
        m_transLoadingBarBottomRight = null;
        title = null;
        tip = null;
        audioSource = null;
        m_loadingImgBg = null;
        m_lblLoadingStatusTip = null;
        EventDispatcher.RemoveEventListener("MogoGlobleLoadingUIOnPress", MogoGlobleLoadingUIOnPress);
    }
    void SetLoadingBarStatus(int status)
    {
        m_spLoadingBar.fillAmount = status * 0.01f;
        m_spLoadingAnim.transform.position = new Vector3();
    }
    
    void OnPress(bool isPress)
    {
        if (!isFirstLoadingUI)
            return;
        if (isPress)
        { }
        else
        {
            if (SetNextText())
                PlaySound();
        }
    }
    protected void MogoGlobleLoadingUIOnPress()
    {
        SetNextText();
    }
    protected bool SetNextText()
    {
        string nextTitle = string.Empty;
        if (DefaultUI.dataMap.ContainsKey(defaultTextOffset + textOffset + 1))
        {
            textOffset++;
            nextTitle = DefaultUI.dataMap.Get(defaultTextOffset + textOffset).content;
            title.text = nextTitle;
        }
        else
        {
            textOffset = 0;
            textOffset++;
            nextTitle = DefaultUI.dataMap.Get(defaultTextOffset + textOffset).content;
            title.text = nextTitle;
        }

        string nextTip = string.Empty;
        if (DefaultUI.dataMap.ContainsKey(defaultTextOffset + textOffset + 1))
        {
            textOffset++;
            nextTip = DefaultUI.dataMap.Get(defaultTextOffset + textOffset).content;
            tip.text = nextTip;
        }

        if (nextTitle != string.Empty && nextTip != string.Empty)
            return true;

        return false;
    }
    protected void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
            audioSource.PlayOneShot(audioSource.clip, 1);
    }
}