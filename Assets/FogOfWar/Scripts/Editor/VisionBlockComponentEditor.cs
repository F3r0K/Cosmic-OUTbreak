using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(VisionBlockComponent))]
public class VisionBlockComponentEditor : Editor
{

    SerializedProperty m_UpdateOnTransform;
    SerializedProperty m_UpdateTimeInterval;

    private void OnEnable()
    {
        m_UpdateOnTransform = serializedObject.FindProperty("m_UpdateOnTransform");
        m_UpdateTimeInterval = serializedObject.FindProperty("m_UpdateTimeInterval");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_UpdateOnTransform);
        if (m_UpdateOnTransform.boolValue)
        {
            EditorGUILayout.PropertyField(m_UpdateTimeInterval);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif