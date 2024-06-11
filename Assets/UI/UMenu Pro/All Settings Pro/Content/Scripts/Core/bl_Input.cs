using UnityEngine;
using System.Collections.Generic;

public class bl_Input : ScriptableObject
{
    public List<bl_KeyInfo> AllKeys = new List<bl_KeyInfo>();

    private static bl_Input m_Instance = null;
    public static bl_Input Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = Resources.Load("InputManager", typeof(bl_Input)) as bl_Input;
                Debug.Log("Get Input");
            }
            return m_Instance;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void InitInput()
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            string keyPrefix = string.Format("Key.{0}", AllKeys[i].Function);
            AllKeys[i].Key = (KeyCode)PlayerPrefs.GetInt(keyPrefix, (int)AllKeys[i].Key);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool GetKeyDown(string function)
    {
        return Input.GetKeyDown(bl_Input.Instance.GetKeyCode(function));
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool GetKey(string function)
    {
        return Input.GetKey(bl_Input.Instance.GetKeyCode(function));
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool GetKeyUp(string function)
    {
        return Input.GetKeyUp(bl_Input.Instance.GetKeyCode(function));
    }

    /// <summary>
    /// 
    /// </summary>
    public bool SetKey(string function, KeyCode newKey)
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (AllKeys[i].Function == function)
            {
                AllKeys[i].Key = newKey;
                string keyPrefix = string.Format("Key.{0}", function);
                PlayerPrefs.SetInt(keyPrefix, (int)newKey);
                Debug.Log(string.Format("Done, replace key function: {0} with {1} key.", function, newKey.ToString()));
                return true;
            }
        }
        Debug.LogError(string.Format("Function {0} is not setup.", function));
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public KeyCode GetKeyCode(string function)
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (AllKeys[i].Function == function)
            {
                return AllKeys[i].Key;
            }
        }
        Debug.LogError(string.Format("Key for {0} is not setup.", function));
        return KeyCode.None;
    }

    /// <summary>
    /// Use this instead of Input.GetAxis("Vertical");
    /// </summary>
    public static float VerticalAxis
    {
        get
        {
            if (GetKey("Up") && !GetKey("Down"))
            {
                return 1;
            }
            else if (!GetKey("Up") && GetKey("Down"))
            {
                return -1;
            }
            else if (GetKey("Up") && GetKey("Down"))
            {
                return 0.5f;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Use this instead of Input.GetAxis("Horizontal");
    /// </summary>
    public static float HorizontalAxis
    {
        get
        {
            if (GetKey("Right") && !GetKey("Left"))
            {
                return 1;
            }
            else if (!GetKey("Right") && GetKey("Left"))
            {
                return -1;
            }
            else if (GetKey("Right") && GetKey("Left"))
            {
                return 0.5f;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool isKeyUsed(KeyCode newKey)
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (AllKeys[i].Key == newKey)
            {
                return true;
            }
        }
        return false;
    }
}