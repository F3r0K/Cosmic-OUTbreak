using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarCheckComputeHandler : ComputeShaderHandler
{
    int m_KernelHandle;

    ComputeBuffer m_ComputeBuffer;
    RenderTexture m_StartTexture;

    public FogOfWarCheckComputeHandler(ComputeShader computeShader) : base(computeShader)
    {
    }

    public override void Dispose()
    {
        m_ComputeBuffer.Dispose();
    }

    public override void Initialize()
    {
        m_KernelHandle = m_ComputeShader.FindKernel("CSFogCheck");
        m_ComputeBuffer = new ComputeBuffer(1, 4);
        m_ComputeBuffer.SetData(new blittableboolean[] { false });
    }

    public void SetFogOfWarFinalTexture(RenderTexture fogofwarfinaltexture)
    {
        m_StartTexture = fogofwarfinaltexture;
        m_ComputeShader.SetTexture(m_KernelHandle, "Input", m_StartTexture);
    }

    public bool IsInFog(Vector2 TexturePosition) {
        m_ComputeShader.SetVector("TexPosition", TexturePosition);
       
        m_ComputeShader.SetBuffer(m_KernelHandle,"Result", m_ComputeBuffer);
        m_ComputeShader.Dispatch(m_KernelHandle,1,1,1);

        blittableboolean[] Result = new blittableboolean[1];
        m_ComputeBuffer.GetData(Result);

        return Result[0];
    }

    public override RenderTexture Run()
    {
        return null;
    }
}
