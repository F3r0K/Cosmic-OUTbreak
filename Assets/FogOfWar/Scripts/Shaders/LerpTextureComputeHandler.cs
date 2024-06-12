using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTextureComputeHandler : ComputeShaderHandler
{

    RenderTexture m_FogOfWarFinalTexture;
    RenderTexture m_StartTexture;
    int m_KernelHandle;
    int m_Width, m_Height;
    float m_LerpSpeed;
    bool m_DissapearVision;

    float m_LastTime;

    public LerpTextureComputeHandler(ComputeShader computeShader, int width, int height,float lerpspeed,bool dissapearvision) : base(computeShader)
    {
        m_Width = width;
        m_Height = height;
        m_LerpSpeed = lerpspeed;
        m_DissapearVision = dissapearvision;
    }

    public override void Dispose()
    {
        
    }

    public override void Initialize()
    {
        m_KernelHandle = m_ComputeShader.FindKernel("CSLerp");
        m_FogOfWarFinalTexture = CreateRenderTexture(m_Width, m_Height,true,RenderTextureFormat.ARGB64);

        RenderTexture.active = m_FogOfWarFinalTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;

        m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_FogOfWarFinalTexture);
    }

    public void SetLerpInput(RenderTexture lerpinputtexture)
    {
        m_StartTexture = lerpinputtexture;
        m_ComputeShader.SetTexture(m_KernelHandle, "Input", m_StartTexture);
    }

    public override RenderTexture Run()
    {

        m_ComputeShader.SetFloat("DeltaTime",(Time.time - m_LastTime)* m_LerpSpeed);
        m_ComputeShader.SetBool("DissapearVision", m_DissapearVision);

        m_ComputeShader.Dispatch(m_KernelHandle, m_Width / 8, m_Height / 8, 1);

        m_LastTime = Time.time;
        return m_FogOfWarFinalTexture;
    }
}
