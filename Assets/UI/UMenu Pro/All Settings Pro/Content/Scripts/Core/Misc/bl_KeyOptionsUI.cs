using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class bl_KeyOptionsUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]private bool DetectIfKeyIsUse = true;
    [Header("References")]
    [SerializeField]private GameObject KeyOptionPrefab;
    [SerializeField]private Transform KeyOptionPanel;
    [SerializeField]private GameObject WaitKeyWindowUI;
    [SerializeField]private Text WaitKeyText;

    private bool WaitForKey = false;
    private bl_KeyInfo WaitFunctionKey;
    private List<bl_KeyInfoUI> cacheKeysInfoUI = new List<bl_KeyInfoUI>();
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        InstanceKeysUI();
        WaitKeyWindowUI.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    void InstanceKeysUI()
    {
        List<bl_KeyInfo> keys = new List<bl_KeyInfo>();
        keys = bl_Input.Instance.AllKeys;

        for(int i = 0; i < keys.Count; i++)
        {
            GameObject kui = Instantiate(KeyOptionPrefab) as GameObject;
            kui.GetComponent<bl_KeyInfoUI>().Init(keys[i], this);
            kui.transform.SetParent(KeyOptionPanel, false);
            kui.gameObject.name = keys[i].Function;
            cacheKeysInfoUI.Add(kui.GetComponent<bl_KeyInfoUI>());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void ClearList()
    {
        foreach(bl_KeyInfoUI kui in cacheKeysInfoUI) { Destroy(kui.gameObject); }
        cacheKeysInfoUI.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (WaitForKey == true && m_InterectableKey) { DetectKey(); }
    }

    /// <summary>
    /// 
    /// </summary>
    void DetectKey()
    {
        foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                if (DetectIfKeyIsUse && bl_Input.Instance.isKeyUsed(vKey) && vKey != WaitFunctionKey.Key)
                {
                    WaitKeyText.text = string.Format("KEY <b>'{0}'</b> IS ALREADY USE, \n PLEASE PRESS ANOTHER KEY FOR REPLACE <b>{1}</b>",vKey.ToString().ToUpper(),WaitFunctionKey.Description.ToUpper());
                }
                else
                {
                    KeyDetected(vKey);
                    WaitForKey = false;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetWaitKeyProcess(bl_KeyInfo info)
    {
        if (WaitForKey)
            return;

        WaitFunctionKey = info;
        WaitForKey = true;
        WaitKeyText.text = string.Format("PRESS A KEY FOR REPLACE <b>{0}</b>", info.Description.ToUpper());
        WaitKeyWindowUI.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    void KeyDetected(KeyCode KeyPressed)
    {
        if (WaitFunctionKey == null) { Debug.LogError("Empty function waiting"); return; }

        if (bl_Input.Instance.SetKey(WaitFunctionKey.Function, KeyPressed))
        {
            ClearList();
            InstanceKeysUI();
            WaitFunctionKey = null;
            WaitKeyWindowUI.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void CancelWait()
    {
        WaitForKey = false;
        WaitFunctionKey = null;
        WaitKeyWindowUI.SetActive(false);
        InteractableKey = true;
    }

    private bool m_InterectableKey = true;
    public bool InteractableKey
    {
        get
        {
            return m_InterectableKey;
        }
        set
        {
            m_InterectableKey = value;
        }
    }
}