using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UMP_Manager : MonoBehaviour {

    [Header("Level Manager")]
    public List<LevelInfo> Levels = new List<LevelInfo>();

    [Header("Settings")]
    public string PlayButtonName = "QUICKPLAY >";

    [Header("References")]
    public List<GameObject> Windows = new List<GameObject>();
    public List<UMP_DialogUI> Dialogs = new List<UMP_DialogUI>();
    public GameObject LevelPrefab;
    public Transform LevelPanel;

    private int CurrentWindow = -1;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        InstanceLevels();
    }

    /// <summary>
    /// 
    /// </summary>
    void InstanceLevels()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            GameObject l = Instantiate(LevelPrefab) as GameObject;

            UMP_LevelInfo li = l.GetComponent<UMP_LevelInfo>();
            li.GetInfo(Levels[i].Title, Levels[i].Description, Levels[i].Preview, Levels[i].LevelName,PlayButtonName);

            l.transform.SetParent(LevelPanel, false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">window to active</param>
    /// <param name="disable">disabled currents window?</param>
    public void ChangeWindow(int id)
    {
        if (CurrentWindow == id)
            return;

        if (id != 2)
        {
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].SetActive(false);
            }
           
        }
        CurrentWindow = id;
        Windows[id].SetActive(true);
    }

    /// <summary>
    /// Open URL
    /// </summary>
    /// <param name="url"></param>
    public void SocialButton(string url) { Application.OpenURL(url); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexDialog"></param>
    public void ShowDialog(int indexDialog)
    {
        if(Dialogs[indexDialog] != null)
        {
            Dialogs[indexDialog].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(string.Format("Does't exits a dialog in the index {0} of list", indexDialog));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexDialog"></param>
    public void ShowDialog(int indexDialog,string text)
    {
        if (Dialogs[indexDialog] != null)
        {
            Dialogs[indexDialog].gameObject.SetActive(true);
             if (!string.IsNullOrEmpty(text)) { Dialogs[indexDialog].SetText(text); }
        }
        else
        {
            Debug.Log(string.Format("Does't exits a dialog in the index {0} of list", indexDialog));
        }
    }

    /// <summary>
    /// Quit
    /// </summary>
    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    [System.Serializable]
    public class LevelInfo
    {
        /// <summary>
        /// Name of scene of build setting
        /// </summary>
        public string LevelName;
        [Space(5)]
        public string Title;
        public string Description;
        public Sprite Preview;
    }
}