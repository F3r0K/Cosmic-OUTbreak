public static class AllOptionsKeyPro
{
    public const string AntiAliasing = "GameName.AntiAliasing";
    public const string AnisoTropic = "GameName.AnisoTropic";
    public const string Resolution = "GameName.ResolutionScreen";
    public const string ResolutionMode = "GameName.ResolutionMode";
    public const string VsyncCount = "GameName.VSyncCount";
    public const string BlendWeight = "GameName.BlendWeight";
    public const string Volumen = "GameName.Volumen";
    public const string Quality = "GameName.QualityLevel";
    public const string TextureLimit = "GameName.TextureLimit";
    public const string ShadowCascade = "GameName.ShadowCascade";
    public const string ShowFPS = "GameName.ShowFPS";
    public const string ShadowDistance = "GameName.ShadowDistance";
    public const string ShadownProjection = "GameName.ShadowProjection";
    public const string PauseAudio = "GameName.PauseAudio";
    public const string ShadowEnable = "GameName.ShadowEnable";
    public const string Brightness = "GameName.Brightness";
    public const string RealtimeReflection = "GameName.RealtimeReflection";
    public const string LodBias = "GameName.LoadBias";
    public const string HUDScale = "GameName.HudScale";

    public static int BoolToInt(bool b)
    {
        return (b == true) ? 1 : 0;
    }

    public static bool IntToBool(int i)
    {
        return (i == 1) ? true : false;
    }
}