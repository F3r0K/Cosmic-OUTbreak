using UnityEngine;
using UnityEngine.UI;

public class UMP_DialogUI : MonoBehaviour
{
    [Header("Settings")]
    public string AnimationParamenter = "show";
    [Header("References")]
    [SerializeField]private Text mText;
    [SerializeField]private Animator m_Animator;

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        if (m_Animator)
        {
            m_Animator.SetBool(AnimationParamenter, true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Close()
    {
        if (m_Animator)
        {
            m_Animator.SetBool(AnimationParamenter, false);
            Invoke("Desactive", m_Animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="infoText"></param>
    public void SetText(string infoText)
    {
        mText.text = infoText;
    }

    /// <summary>
    /// 
    /// </summary>
    void Desactive()
    {
        gameObject.SetActive(false);
    }
}