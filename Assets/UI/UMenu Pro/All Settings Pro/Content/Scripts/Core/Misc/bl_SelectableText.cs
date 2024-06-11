using UnityEngine;
using UnityEngine.UI;

public class bl_SelectableText : MonoBehaviour
{
    [SerializeField]private Color OnEnterColor = new Color(1,1,1,1);
    [SerializeField,Range(0.1f,3)]private float Duration = 1;

    private Text m_Text;
    private Button m_Button;
    private Color defaultColor;
    private ColorBlock defaultColorBlock;
    private ColorBlock OnSelectColorBlock;

    void Awake()
    {
        if (GetComponent<Text>() != null)
        {
            m_Text = GetComponent<Text>();
            defaultColor = m_Text.color;
        }
        if(GetComponent<Button>() != null)
        {
            m_Button = GetComponent<Button>();
            defaultColorBlock = m_Button.colors;
            OnSelectColorBlock = defaultColorBlock;
            OnSelectColorBlock.normalColor = OnEnterColor;
        }
    }

    public void OnEnter()
    {
        if (m_Text != null) { m_Text.CrossFadeColor(OnEnterColor, Duration, true, true); }
        if(m_Button != null) { m_Button.colors = OnSelectColorBlock; }
    }

    public void OnExit()
    {
        if (m_Text != null) { m_Text.CrossFadeColor(defaultColor, Duration, true, true); }
        if (m_Button != null) { m_Button.colors = defaultColorBlock; }
    }
}