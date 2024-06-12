using UnityEngine;

public class ShaderReferencesScriptableObject : ScriptableObject
{
    public ComputeShader m_FogOfWarComputeShader;
    public ComputeShader m_UpscaleComputeShader;
    public ComputeShader m_LerpComputeShader;
    public ComputeShader m_FogOfWarCheckComputeShader;
    public Shader m_BlurShader;
}