using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpscaleFogComputeHandler : ComputeShaderHandler
{
    int m_KernelHandle;
    int m_Width, m_Height;
    RenderTexture m_FogOfWarUpscaledTexture;
    RenderTexture m_FogOfWarInputTexture;

    public UpscaleFogComputeHandler(ComputeShader computeShader, int width, int height) : base(computeShader)
    {
        m_Width = width;
        m_Height = height;
    }

    public override void Initialize()
    {
        m_FogOfWarUpscaledTexture = CreateRenderTexture(m_Width * 4, m_Height * 4);

        m_KernelHandle = m_ComputeShader.FindKernel("CSUpscale");
        m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_FogOfWarUpscaledTexture);
        m_ComputeShader.SetInt("ResolutionX", m_Width);
        m_ComputeShader.SetInt("ResolutionY", m_Height);
    }

    public void SetFogOfWarInput(RenderTexture FogofwarInputTexture) {
        m_FogOfWarInputTexture = FogofwarInputTexture;
        m_ComputeShader.SetTexture(m_KernelHandle, "Input", m_FogOfWarInputTexture);
    }

    public override RenderTexture Run()
    {
        m_ComputeShader.Dispatch(m_KernelHandle, m_Width * 4 / 8, m_Height * 4 / 8, 1);
        return m_FogOfWarUpscaledTexture;
    }

    public override void Dispose()
    {
        
    }
}
