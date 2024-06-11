using UnityEngine;
using UnityEngine.UI;

public class bl_KeyInfoUI : MonoBehaviour
{
    [SerializeField]private Text FunctionText;
    [SerializeField]private Text KeyText;

    private bl_KeyInfo cacheInfo;
    private bl_KeyOptionsUI KeyOptions;

    public void Init(bl_KeyInfo info,bl_KeyOptionsUI koui)
    {
        cacheInfo = info;
        KeyOptions = koui;
        FunctionText.text = info.Description;
        KeyText.text = info.Key.ToString();
    }

    public void SetKeyChange()
    {
        KeyOptions.SetWaitKeyProcess(cacheInfo);
    }
 
}