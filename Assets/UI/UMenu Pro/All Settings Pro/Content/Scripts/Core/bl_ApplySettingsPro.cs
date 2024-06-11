using UnityEngine;
using UnityEngine.UI;
public class bl_ApplySettingsPro : MonoBehaviour
{

    
    [SerializeField]private CanvasScaler HUDCanvas;
    [SerializeField]private GameObject[] FPSObject;
    private int[] ShadowCascadeOptions = new int[] { 0, 2, 4, };
    private bl_BrightnessImage BrightnessImage;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (FindObjectOfType<bl_BrightnessImage>() != null) { BrightnessImage = FindObjectOfType<bl_BrightnessImage>(); }
        LoadAndApply();
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
        }
        else
        {
            QualitySettings.shadowProjection = ShadowProjection.CloseFit;
        }       
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadAndApply()
    {
        int CurrentAA = PlayerPrefs.GetInt(AllOptionsKeyPro.AntiAliasing);
        int CurrentAS = PlayerPrefs.GetInt(AllOptionsKeyPro.AnisoTropic);
        int CurrentBW = PlayerPrefs.GetInt(AllOptionsKeyPro.BlendWeight);
        int CurrentQuality = PlayerPrefs.GetInt(AllOptionsKeyPro.Quality);
        #if !UNITY_EDITOR
        int CurrentRS = PlayerPrefs.GetInt(AllOptionsKeyPro.Resolution);
       #endif
        int CurrentVSC = PlayerPrefs.GetInt(AllOptionsKeyPro.VsyncCount);
        int CurrentTL = PlayerPrefs.GetInt(AllOptionsKeyPro.TextureLimit, 0);
        int CurrentSC = PlayerPrefs.GetInt(AllOptionsKeyPro.ShadowCascade, 0);
        bool _showFPS = (PlayerPrefs.GetInt(AllOptionsKeyPro.ShowFPS, 0) == 1) ? true : false;
        float _volumen = PlayerPrefs.GetFloat(AllOptionsKeyPro.Volumen, 1);
        float sd = PlayerPrefs.GetFloat(AllOptionsKeyPro.ShadowDistance);
        bool shadowProjection = (PlayerPrefs.GetInt(AllOptionsKeyPro.ShadownProjection, 0) == 1) ? true : false;
        bool _shadowEnable = AllOptionsKeyPro.IntToBool(PlayerPrefs.GetInt(AllOptionsKeyPro.ShadowEnable));
        float _brightness = PlayerPrefs.GetFloat(AllOptionsKeyPro.Brightness, 1);
        bool _realtimeReflection = AllOptionsKeyPro.IntToBool(PlayerPrefs.GetInt(AllOptionsKeyPro.RealtimeReflection, 1));
        float _lodBias = PlayerPrefs.GetFloat(AllOptionsKeyPro.LodBias, 1);
        float _hudScale = PlayerPrefs.GetFloat(AllOptionsKeyPro.HUDScale, 0);

        QualitySettings.shadowDistance = sd;
        AudioListener.volume = _volumen;
        AudioListener.pause = (PlayerPrefs.GetInt(AllOptionsKeyPro.PauseAudio, 0) == 1 ? true : false);
        ShadowProjectionType(shadowProjection);
        QualitySettings.masterTextureLimit = CurrentTL;
        QualitySettings.shadowCascades = ShadowCascadeOptions[CurrentSC];
        QualitySettings.SetQualityLevel(CurrentQuality);
        QualitySettings.realtimeReflectionProbes = _realtimeReflection;
        QualitySettings.shadowDistance = (_shadowEnable) ? sd : 0;
        QualitySettings.lodBias = _lodBias;
        if(BrightnessImage != null) { BrightnessImage.SetValue(_brightness); } else
        {
            Debug.LogWarning("You have not the brightness prefab in this scene, brightness will not work");
        }
        if (HUDCanvas != null) { HUDCanvas.matchWidthOrHeight = (1 - _hudScale); }
        if (FPSObject != null) { foreach (GameObject g in FPSObject) { g.SetActive(_showFPS); } }

        switch (CurrentAS)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;
            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                break;
            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
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
        switch (CurrentBW)
        {
            case 0:
                QualitySettings.skinWeights = SkinWeights.OneBone;
                break;
            case 1:
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                break;
            case 2:
                QualitySettings.skinWeights = SkinWeights.FourBones;
                break;
        }

#if !UNITY_EDITOR
        Screen.SetResolution(Screen.resolutions[CurrentRS].width, Screen.resolutions[CurrentRS].height, false);
#endif
    }

}