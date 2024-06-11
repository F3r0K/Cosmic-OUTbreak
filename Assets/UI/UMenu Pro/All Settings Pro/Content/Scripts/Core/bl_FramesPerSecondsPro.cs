using UnityEngine;
using UnityEngine.UI;

public class bl_FramesPerSecondsPro : MonoBehaviour
{
    [SerializeField]
    private Text FPSText = null;
    float deltaTime = 0.0f;

    void Update()
    {
        if (FPSText == null)
            return;

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} FPS)", msec, fps);
        FPSText.text = text.ToUpper();
    }
}