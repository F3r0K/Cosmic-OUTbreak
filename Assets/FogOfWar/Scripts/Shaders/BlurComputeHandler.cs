using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurComputeHandler : ComputeShaderHandler
{

    RenderTexture m_BlurTexture;
    int m_Iterations;
    int m_KernelHandle;

    public BlurComputeHandler(ComputeShader computeShader, int iterations) : base(computeShader)
    {
        m_Iterations = iterations;
    }

    public override void Dispose()
    {

    }

    public override void Initialize()
    {
        m_KernelHandle = m_ComputeShader.FindKernel("CSBlur");
    }

    public void SetBlurInput(RenderTexture blurinputtexture)
    {
        m_BlurTexture = blurinputtexture;
        m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_BlurTexture);
    }

    public override RenderTexture Run()
    {
        for (int i = 0; i < m_Iterations; i++)
        {
            m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_BlurTexture);
            m_ComputeShader.Dispatch(m_KernelHandle, m_BlurTexture.width, m_BlurTexture.height, 1);
        }

        return m_BlurTexture;
    }
}
