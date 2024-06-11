using UnityEngine;
using System.Collections;

public class bl_SelectableTextManager : MonoBehaviour
{

    private bl_SelectableText[] AllSelectables;

    void Awake()
    {
        AllSelectables = GetComponentsInChildren<bl_SelectableText>();
    }

    public void OnEnter()
    {
        if(AllSelectables.Length > 0)
        {
            foreach(bl_SelectableText st in AllSelectables) { st.OnEnter(); }
        }
    }

    public void OnExit()
    {
        if (AllSelectables.Length > 0)
        {
            foreach (bl_SelectableText st in AllSelectables) { st.OnExit(); }
        }
    }
}