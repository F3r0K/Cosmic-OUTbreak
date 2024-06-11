using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UMP_ButtonGroup : MonoBehaviour, IPointerClickHandler {

    public int GroupID = 0;
    public bool Select = false;

    void Start()
    {
        if (Select)
        {
            OnSelect();
        }
    }

	public void OnPointerClick(PointerEventData e)
    {
        OnSelect();
    }

    public void OnSelect()
    {
        UMP_ButtonGroup[] all = FindObjectsOfType<UMP_ButtonGroup>();
        foreach (UMP_ButtonGroup b in all)
        {
            if (b.GroupID == this.GroupID)
            {
                b.UnSelect();
            }
        }
        button.interactable = false;
    }

    public void UnSelect()
    {

        button.interactable = true;
    }

    private Button _button;
    private Button button
    {
        get
        {
            if(_button == null)
            {
                _button = GetComponent<Button>();
            }
            return _button;
        }       
    }
}