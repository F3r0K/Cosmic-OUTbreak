using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class bl_BrightnessImage : MonoBehaviour {

    private float Value = 1;

    void Start()
    {
        transform.SetAsLastSibling();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="val">brightness value 0 to 1</param>
    public void SetValue(float val)
    {
        Value = val;
        Alpha.alpha = (1 - Value);
    }

    private CanvasGroup _Alpha;
    private CanvasGroup Alpha
    {
        get
        {
            if(_Alpha == null) { _Alpha = GetComponent<CanvasGroup>(); }
            return _Alpha;
        }
    }
}