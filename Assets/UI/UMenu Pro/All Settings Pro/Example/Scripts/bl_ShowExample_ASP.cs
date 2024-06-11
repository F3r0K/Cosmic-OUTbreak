using UnityEngine;

public class bl_ShowExample_ASP : MonoBehaviour
{
    private bl_AllOptionsPro AllSettings;

    void Awake()
    {
        AllSettings = FindObjectOfType<bl_AllOptionsPro>();
       // Time.timeScale = 0;
    }

    void Update()
    {
        if (bl_Input.GetKeyDown("Pause"))
        {
            AllSettings.ShowMenu();
        }        
    }
}