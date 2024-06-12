using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(FogOfWarManager))]
public class FogOfWarManagerEditor : Editor
{

    SerializedProperty m_MapWidth;
    SerializedProperty m_MapHeight;
    SerializedProperty m_MapResolution;

    SerializedProperty m_FogPlane;
    SerializedProperty m_FogColor;

    SerializedProperty m_FogDissapear;
    SerializedProperty m_FogUpdateInterval;
    SerializedProperty m_FogAppearSpeed;
    SerializedProperty m_BlurIterations;
    SerializedProperty m_ComputeShaders;

    SerializedProperty m_FogOfWarOutput;

    SerializedProperty m_LogWarnings;
    bool ShowVisuals;
    bool ShowAdvanced;
    bool ShowOptional;

    private void OnEnable()
    {
        m_MapWidth = serializedObject.FindProperty("m_MapWidth");
        m_MapHeight = serializedObject.FindProperty("m_MapHeight");
        m_MapResolution = serializedObject.FindProperty("m_Resolution");

        m_FogPlane = serializedObject.FindProperty("m_FogPlane");
        m_FogColor = serializedObject.FindProperty("m_FogColor");

        m_FogUpdateInterval = serializedObject.FindProperty("m_FogUpdateInterval");
        m_FogAppearSpeed = serializedObject.FindProperty("m_FogAppearSpeed");
        m_BlurIterations = serializedObject.FindProperty("m_BlurIterations");
        m_ComputeShaders = serializedObject.FindProperty("m_ShaderReferencesObject");

        m_FogDissapear= serializedObject.FindProperty("m_VisionDissapear");

        m_FogOfWarOutput = serializedObject.FindProperty("m_FogOfWarOutput");
        m_LogWarnings = serializedObject.FindProperty("m_LogWarnings");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_MapWidth);
        EditorGUILayout.PropertyField(m_MapHeight);
        EditorGUILayout.PropertyField(m_MapResolution);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_FogDissapear);
        if (ShowVisuals=EditorGUILayout.Foldout(ShowVisuals, "Visuals"))
        {
            EditorGUILayout.PropertyField(m_FogPlane);
            EditorGUILayout.PropertyField(m_FogColor);
        }
        if (ShowAdvanced = EditorGUILayout.Foldout(ShowAdvanced, "Advanced"))
        {
            EditorGUILayout.PropertyField(m_FogUpdateInterval);
            EditorGUILayout.PropertyField(m_FogAppearSpeed);
            EditorGUILayout.PropertyField(m_BlurIterations);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ComputeShaders);
        }
        if (ShowOptional = EditorGUILayout.Foldout(ShowOptional, "Optional"))
        {
            EditorGUILayout.PropertyField(m_FogOfWarOutput);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_LogWarnings);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif