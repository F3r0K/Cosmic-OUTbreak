using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_AllOptionsPro : MonoBehaviour {

    [Header("Panels")]
    [SerializeField]private GameObject[] Panels;
    [SerializeField]private Button[] PanelButtons;
    [SerializeField]private Animator PanelAnimator;

    [Header("Settings")]
    public bool ApplyOnStart = false;
    public bool AutoApplyResolution = true;
    public bool SaveOnDisable = true;
    public bool AnimateHidePanel = true;
    public int StartWindow;
    [SerializeField, Range(0, 8)]
    private int DefaultQuality = 3;
    [SerializeField, Range(0, 15)]
    private int DefaultResolution = 7;
    [SerializeField, Range(0, 3)]
    private int DefaultAntiAliasing = 1;
    [SerializeField, Range(0, 2)]
    private int DefaultAnisoTropic = 1;
    [SerializeField, Range(0, 2)]
    private int DefaultVSync = 1;
    [SerializeField, Range(0, 2)]
    private int DefaultBlendWeight = 1;
    [SerializeField, Range(0, 100)]
    private int DefaultShadowDistance = 40;
    [SerializeField, Range(0, 1)]
    private int DefaultBrightness = 1;   
    [SerializeField, Range(0.01f, 3)]
    private int DefaultLoadBias = 1;


    [Header("Options Name")]
    [SerializeField] private string[] AntiAliasingNames = new string[] { "X0", "X2", "X4", "X8" };
    [SerializeField]private string[] VSyncNames = new string[] { "Don't Sync", "Every V Blank", "Every Second V Blank" };
    [SerializeField]private string[] TextureQualityNames = new string[] { "FULL RES", "HALF RES", "QUARTER RES", "EIGHTH RES", };
    [SerializeField]private string[] ShadowCascadeNames = new string[] { "NO CASCADES", "TWO CASCADES", "FOUR CASCADES", };

    [Header("References")]
    [SerializeField]private GameObject SettingsPanel;
    [SerializeField]private Animator ContentAnim;
    public Text QualityText = null;
    private int CurrentQuality = 0;

    public Text AnisotropicText = null;
    private int CurrentAS = 0;

    public Text AntiAliasingText = null;
    private int CurrentAA = 0;

    public Text vSyncText = null;
    private int CurrentVSC = 0;

    public Text blendWeightsText = null;
    private int CurrentBW = 0;

    public Text ResolutionText;
    private int CurrentRS = 0;

    [SerializeField]
    private Text FullScreenOnText;
    private bool useFullScreen = false;

    [SerializeField]
    private Text TextureLimitText;
    private int CurrentTL = 0; 

    [SerializeField]private Text RealtimeReflectionText;
    private bool _realtimeReflection;

    [SerializeField]private Text LoadBiasText;
    private float _lodBias;

    [SerializeField]
    private Text ShadowCascadeText;
    private int CurrentSC = 2;
    private int[] ShadowCascadeOptions = new int[] { 0, 2, 4, }; 

    [SerializeField]
    private Text ShowFPSText;
    private bool _showFPS = false;

    [SerializeField]
    private Text ShadowDistanceText = null;
    [SerializeField]private Slider ShadowDistanceSlider;
    private float cacheShadowDistance;
    [SerializeField]private Slider BrightnessSlider;
    [SerializeField]private Slider LoadBiasSlider;
    [SerializeField]private Slider HUDScaleFactor;
    [SerializeField]private Text HudScaleText;
    [SerializeField]private Text BrightnessText;
    private float _brightness;

    [SerializeField]private Text ShadowProjectionText;
    private bool shadowProjection = false;

    [SerializeField]private Text ShadowEnebleText;
    private bool _shadowEnable;

    [SerializeField]private Text PauseText;
    private bool _isPauseSound;

    [SerializeField]private Text VolumenText;
    [SerializeField]private Slider VolumenSlider;
    [SerializeField]private Text TitlePanelText;
    [SerializeField]private CanvasScaler HUDCanvas;
    [SerializeField]private GameObject[] FPSObject;
    private bl_BrightnessImage BrightnessImage;
    private float _hudScale;
    private bool Show = false;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if(FindObjectOfType<bl_BrightnessImage>() != null) { BrightnessImage = FindObjectOfType<bl_BrightnessImage>(); }
        if (HUDCanvas) { _hudScale = (1 - HUDCanvas.matchWidthOrHeight); }
    }

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (ApplyOnStart)
        {
            LoadAndApply();
        }
        ChangeWindow(StartWindow,false);
        ChangeSelectionButton(PanelButtons[StartWindow]);
        SettingsPanel.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        if (SaveOnDisable) { SaveOptions(); }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnApplicationQuit()
    {
        if (SaveOnDisable) { SaveOptions(); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_id"></param>
    public void ChangeWindow(int _id)
    {
        PanelAnimator.Play("Change", 0, 0);
        StartCoroutine(WaitForSwichet(_id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_id"></param>
    public void ChangeWindow(int _id,bool anim)
    {
        if (anim)
        {
            PanelAnimator.Play("Change", 0, 0);
        }
        StartCoroutine(WaitForSwichet(_id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ChangeSelectionButton(Button b)
    {
        for(int i = 0; i < PanelButtons.Length; i++)
        {
            PanelButtons[i].interactable = true;
        }
        b.interactable = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowMenu()
    {
        Show = !Show;
        if (Show)
        {
            StopCoroutine("HideAnimate");
            SettingsPanel.SetActive(true);
            ContentAnim.SetBool("Show", true);
        }
        else
        {
            if (AnimateHidePanel)
            {
                StartCoroutine("HideAnimate");
            }
            else
            {
                SettingsPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mas"></param>
    public void GameQuality(bool mas)
    {
        if (mas)
        {
            CurrentQuality = (CurrentQuality + 1) % QualitySettings.names.Length;
        }
        else
        {
            if (CurrentQuality != 0)
            {
                CurrentQuality = (CurrentQuality - 1) % QualitySettings.names.Length;
            }
            else
            {
                CurrentQuality = (QualitySettings.names.Length - 1);
            }
        }
        QualityText.text = QualitySettings.names[CurrentQuality].ToUpper();
        QualitySettings.SetQualityLevel(CurrentQuality);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void AntiStropic(bool b)
    {
        if (b) { CurrentAS = (CurrentAS + 1) % 3; } else { if (CurrentAS != 0) { CurrentAS = (CurrentAS - 1) % 3; } else { CurrentAS = 2; } }

        switch (CurrentAS)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                AnisotropicText.text = AnisotropicFiltering.Disable.ToString().ToUpper();
                break;
            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                AnisotropicText.text = AnisotropicFiltering.Enable.ToString().ToUpper();
                break;
            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                AnisotropicText.text = AnisotropicFiltering.ForceEnable.ToString().ToUpper();
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="use"></param>
    public void FullScreenMode(bool use)
    {
        useFullScreen = use;
        FullScreenOnText.text = (useFullScreen) ? "ON" : "OFF";
#if UNITY_EDITOR
        Debug.Log("Full Screen Settings just work on build.");
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void AntiAliasing(bool b)
    {
        CurrentAA = (b) ? (CurrentAA + 1) % 4 : (CurrentAA != 0) ? (CurrentAA - 1) % 4 : CurrentAA = 3;
        AntiAliasingText.text = AntiAliasingNames[CurrentAA].ToUpper();
        switch (CurrentAA)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowFPS()
    {
        _showFPS = !_showFPS;
        ShowFPSText.text = (_showFPS) ? "ON" : "OFF";
        if(FPSObject != null) { foreach (GameObject g in FPSObject) { g.SetActive(_showFPS); } }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void PauseSound(bool b)
    {
        _isPauseSound = b;
        string t = (_isPauseSound) ? "ON" : "OFF";
        PauseText.text = t;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void VSyncCount(bool b)
    {
        CurrentVSC = (b) ? (CurrentVSC + 1) % 3 : (CurrentVSC != 0) ? (CurrentVSC - 1) % 3 : CurrentVSC = 2;
        vSyncText.text = VSyncNames[CurrentVSC].ToUpper();
        switch (CurrentVSC)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                break;
            case 1:
                QualitySettings.vSyncCount = 1;
                break;
            case 2:
                QualitySettings.vSyncCount = 2;
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void TextureQuality(bool b)
    {
        CurrentTL = (b) ? (CurrentTL + 1) % 3 : (CurrentTL != 0) ? (CurrentTL - 1) % 3 : CurrentTL = 3;
        QualitySettings.masterTextureLimit = CurrentTL;
        TextureLimitText.text = TextureQualityNames[CurrentTL];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ShadowCascades(bool b)
    {
        CurrentSC = (b) ? (CurrentSC + 1) % 3 : (CurrentSC != 0) ? (CurrentSC - 1) % 3 : CurrentSC = 3;
        QualitySettings.shadowCascades = ShadowCascadeOptions[CurrentSC];
        ShadowCascadeText.text = ShadowCascadeNames[CurrentSC];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void blendWeights(bool b)
    {
        CurrentBW = (b) ? (CurrentBW + 1) % 3 : (CurrentBW != 0) ? (CurrentBW - 1) % 3 : CurrentBW = 2;
        switch (CurrentBW)
        {
            case 0:
                QualitySettings.skinWeights = SkinWeights.OneBone;
                blendWeightsText.text = SkinWeights.OneBone.ToString().ToUpper();
                break;
            case 1:
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                blendWeightsText.text = SkinWeights.TwoBones.ToString().ToUpper();
                break;
            case 2:
                QualitySettings.skinWeights = SkinWeights.FourBones;
                blendWeightsText.text = SkinWeights.FourBones.ToString().ToUpper();
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    public void SetBrightness(float v)
    {
        if (BrightnessImage == null)
            return;
       
        _brightness = v;
        BrightnessImage.SetValue(v);
        BrightnessSlider.value = v;
        BrightnessText.text = string.Format("{0}%", (v * 100).ToString("F0"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetLodBias(float value)
    {
        QualitySettings.lodBias = value;
        _lodBias = value;
        LoadBiasText.text = string.Format("{0}", value.ToString("F2"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void ShadowDistance(float value)
    {
        if (_shadowEnable)
        {
            QualitySettings.shadowDistance = value;
        }
        ShadowDistanceText.text = string.Format("{0}m", value.ToString("F0"));
        cacheShadowDistance = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetShadowEnable(bool enable)
    {
        QualitySettings.shadowDistance = (enable) ? cacheShadowDistance : 0;
        _shadowEnable = enable;
        ShadowEnebleText.text = (enable) ? "ENABLE" : "DISABLE";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void SetRealTimeReflection(bool b)
    {
        QualitySettings.realtimeReflectionProbes = b;
        _realtimeReflection = b;
        RealtimeReflectionText.text = (_realtimeReflection) ? "ENABLE" : "DISABLE";        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetHUDScale(float value)
    {
        if (HUDCanvas == null)
            return;

        HUDCanvas.matchWidthOrHeight = (1 - value);
        _hudScale = value;
        HudScaleText.text = string.Format("{0}", value.ToString("F2"));
    }

    /// <summary>
    /// Change resolution of screen
    /// NOTE: this work only in build game, this not work in
    /// Unity Editor.
    /// </summary>
    /// <param name="b"></param>
    public void Resolution(bool b)
    {
        CurrentRS = (b) ? (CurrentRS + 1) % Screen.resolutions.Length : (CurrentRS != 0) ? (CurrentRS - 1) % Screen.resolutions.Length : CurrentRS = (Screen.resolutions.Length - 1);
        ResolutionText.text = Screen.resolutions[CurrentRS].width + " X " + Screen.resolutions[CurrentRS].height;
#if UNITY_EDITOR
        Debug.Log("Resolution Settings just work on build.");
         #endif
    }

    /// <summary>
    /// 
    /// </summary>
    private float _volumen;
    public void Volumen(float v)
    {
        AudioListener.volume = v;
        _volumen = v;
        VolumenText.text = (_volumen * 100).ToString("00") + "%";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ShadowProjectionType(bool b)
    {
        if (b)
        {
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
            ShadowProjectionText.text = ShadowProjection.StableFit.ToString().ToUpper();
        }
        else
        {
            QualitySettings.shadowProjection = ShadowProjection.CloseFit;
            ShadowProjectionText.text = ShadowProjection.CloseFit.ToString().ToUpper();
        }
    }

    /// <summary>
    /// Options for apply just resolution settings
    /// </summary>
    public void ApplyResolution()
    {     
#if UNITY_EDITOR
        Debug.Log("Resolution Settings just work on build.");
        return;
#else
         bool apply = (AutoApplyResolution) ? useFullScreen : false;
        Screen.SetResolution(Screen.resolutions[CurrentRS].width, Screen.resolutions[CurrentRS].height, apply);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadAndApply()
    {
        bl_Input.Instance.InitInput();
        CurrentAA = PlayerPrefs.GetInt(AllOptionsKeyPro.AntiAliasing, DefaultAntiAliasing);
        CurrentAS = PlayerPrefs.GetInt(AllOptionsKeyPro.AnisoTropic, DefaultAnisoTropic);
        CurrentBW = PlayerPrefs.GetInt(AllOptionsKeyPro.BlendWeight, DefaultBlendWeight);
        CurrentQuality = PlayerPrefs.GetInt(AllOptionsKeyPro.Quality, DefaultQuality);
        CurrentRS = PlayerPrefs.GetInt(AllOptionsKeyPro.Resolution, DefaultResolution);
        CurrentVSC = PlayerPrefs.GetInt(AllOptionsKeyPro.VsyncCount, DefaultVSync);
        CurrentTL = PlayerPrefs.GetInt(AllOptionsKeyPro.TextureLimit, 0);
        CurrentSC = PlayerPrefs.GetInt(AllOptionsKeyPro.ShadowCascade, 0);
        _showFPS = (PlayerPrefs.GetInt(AllOptionsKeyPro.ShowFPS, 0) == 1) ? true : false;
        _volumen = PlayerPrefs.GetFloat(AllOptionsKeyPro.Volumen, 1);
        float sd = PlayerPrefs.GetFloat(AllOptionsKeyPro.ShadowDistance, DefaultShadowDistance);
        shadowProjection = (PlayerPrefs.GetInt(AllOptionsKeyPro.ShadownProjection, 0) == 1) ? true : false;
        PauseSound((PlayerPrefs.GetInt(AllOptionsKeyPro.PauseAudio,0) == 1 ? true : false));
        useFullScreen = (PlayerPrefs.GetInt(AllOptionsKeyPro.ResolutionMode, 0) == 1) ? true : false;
        _shadowEnable = AllOptionsKeyPro.IntToBool(PlayerPrefs.GetInt(AllOptionsKeyPro.ShadowEnable));
        _brightness = PlayerPrefs.GetFloat(AllOptionsKeyPro.Brightness, DefaultBrightness);
        _realtimeReflection = AllOptionsKeyPro.IntToBool(PlayerPrefs.GetInt(AllOptionsKeyPro.RealtimeReflection, 1));
        _lodBias = PlayerPrefs.GetFloat(AllOptionsKeyPro.LodBias, DefaultLoadBias);
        _hudScale = PlayerPrefs.GetFloat(AllOptionsKeyPro.HUDScale, _hudScale);

        SetBrightness(_brightness);
        ShadowDistance(sd);
        ShadowDistanceSlider.value = sd;
        Volumen(_volumen);
        VolumenSlider.value = _volumen;
        ShadowProjectionType(shadowProjection);
        SetShadowEnable(_shadowEnable);
        SetRealTimeReflection(_realtimeReflection);
        SetLodBias(_lodBias);
        SetHUDScale(_hudScale);
        ApplyResolution();

        QualitySettings.shadowCascades = ShadowCascadeOptions[CurrentSC];
        ShadowCascadeText.text = ShadowCascadeNames[CurrentSC].ToUpper();
        QualityText.text = QualitySettings.names[CurrentQuality].ToUpper();
        QualitySettings.SetQualityLevel(CurrentQuality);
        FullScreenOnText.text = (useFullScreen) ? "ON" : "OFF";
        ShowFPSText.text = (_showFPS) ? "ON" : "OFF";
        if (FPSObject != null) { foreach (GameObject g in FPSObject) { g.SetActive(_showFPS); } }
        BrightnessSlider.value = _brightness;
        LoadBiasSlider.value = _lodBias;
        HUDScaleFactor.value = _hudScale;
        switch (CurrentAS)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                AnisotropicText.text = AnisotropicFiltering.Disable.ToString().ToUpper();
                break;
            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                AnisotropicText.text = AnisotropicFiltering.Enable.ToString().ToUpper();
                break;
            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                AnisotropicText.text = AnisotropicFiltering.ForceEnable.ToString().ToUpper();
                break;
        }

        switch (CurrentAA)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                break;
        }
        AntiAliasingText.text = AntiAliasingNames[CurrentAA].ToUpper();

        switch (CurrentVSC)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                break;
            case 1:
                QualitySettings.vSyncCount = 1;
                break;
            case 2:
                QualitySettings.vSyncCount = 2;
                break;
        }
        vSyncText.text = VSyncNames[CurrentVSC].ToUpper();
        switch (CurrentBW)
        {
            case 0:
                QualitySettings.skinWeights = SkinWeights.OneBone;
                blendWeightsText.text = SkinWeights.OneBone.ToString().ToUpper();
                break;
            case 1:
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                blendWeightsText.text = SkinWeights.TwoBones.ToString().ToUpper();
                break;
            case 2:
                QualitySettings.skinWeights = SkinWeights.FourBones;
                blendWeightsText.text = SkinWeights.FourBones.ToString().ToUpper();
                break;
        }
        QualitySettings.masterTextureLimit = CurrentTL;
        TextureLimitText.text = TextureQualityNames[CurrentTL];

#if !UNITY_EDITOR
        ResolutionText.text = Screen.resolutions[CurrentRS].width + " X " + Screen.resolutions[CurrentRS].height;
        bool apply = (AutoApplyResolution) ? useFullScreen : false;
        Screen.SetResolution(Screen.resolutions[CurrentRS].width, Screen.resolutions[CurrentRS].height, apply);
#else
        ResolutionText.text = Screen.resolutions[0].width + " X " + Screen.resolutions[0].height;
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    IEnumerator WaitForSwichet(int _id)
    {
        yield return StartCoroutine(WaitForRealSeconds(0.25f));
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        Panels[_id].SetActive(true);
        if (TitlePanelText != null)
        {
            TitlePanelText.text = Panels[_id].name.ToUpper();
        }
    }

    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator HideAnimate()
    {
        if(ContentAnim != null)
        {
            ContentAnim.SetBool("Show", false);
            yield return new WaitForSeconds(ContentAnim.GetCurrentAnimatorStateInfo(0).length);
            SettingsPanel.SetActive(false);
        }
        else
        {
            SettingsPanel.SetActive(false);
        }
    }

        /// <summary>
        /// Save all options for load in a next time
        /// </summary>
    public void SaveOptions()
    {
        PlayerPrefs.SetInt(AllOptionsKeyPro.AnisoTropic, CurrentAS);
        PlayerPrefs.SetInt(AllOptionsKeyPro.AntiAliasing, CurrentAA);
        PlayerPrefs.SetInt(AllOptionsKeyPro.BlendWeight, CurrentBW);
        PlayerPrefs.SetInt(AllOptionsKeyPro.Quality, CurrentQuality);
        PlayerPrefs.SetInt(AllOptionsKeyPro.Resolution, CurrentRS);
        PlayerPrefs.SetInt(AllOptionsKeyPro.VsyncCount, CurrentVSC);
        PlayerPrefs.SetInt(AllOptionsKeyPro.AnisoTropic, CurrentAS);
        PlayerPrefs.SetInt(AllOptionsKeyPro.TextureLimit, CurrentTL);
        PlayerPrefs.SetInt(AllOptionsKeyPro.ShadowCascade, CurrentSC);
        PlayerPrefs.SetFloat(AllOptionsKeyPro.Volumen, _volumen);
        PlayerPrefs.SetFloat(AllOptionsKeyPro.ShadowDistance, cacheShadowDistance);
        PlayerPrefs.SetInt(AllOptionsKeyPro.ShadownProjection, (shadowProjection) ? 1 : 0);
        PlayerPrefs.SetInt(AllOptionsKeyPro.ShowFPS, (_showFPS) ? 1 : 0);
        PlayerPrefs.SetInt(AllOptionsKeyPro.PauseAudio, (_isPauseSound) ? 1 : 0);
        PlayerPrefs.SetInt(AllOptionsKeyPro.ResolutionMode, (useFullScreen) ? 1 : 0);
        PlayerPrefs.SetInt(AllOptionsKeyPro.ShadowEnable, AllOptionsKeyPro.BoolToInt(_shadowEnable));
        PlayerPrefs.SetFloat(AllOptionsKeyPro.Brightness, _brightness);
        PlayerPrefs.SetInt(AllOptionsKeyPro.RealtimeReflection, AllOptionsKeyPro.BoolToInt(_realtimeReflection));
        PlayerPrefs.SetFloat(AllOptionsKeyPro.LodBias, _lodBias);
        PlayerPrefs.SetFloat(AllOptionsKeyPro.HUDScale, _hudScale);  
    }
}