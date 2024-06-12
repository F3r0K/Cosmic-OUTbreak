using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComputeShaderHandler
{
    protected ComputeShader m_ComputeShader;

    public ComputeShaderHandler(ComputeShader computeShader)
    {
        m_ComputeShader = computeShader;
    }

    protected RenderTexture CreateRenderTexture(int width, int height,bool enablerandomwrite=true,RenderTextureFormat renderTextureFormat=RenderTextureFormat.ARGB32) {
        var rendertexture = new RenderTexture(width, height, 1, renderTextureFormat);
        rendertexture.enableRandomWrite = enablerandomwrite;
        rendertexture.Create();
        return rendertexture;
    }
    public abstract RenderTexture Run();
    public abstract void Initialize();

    public abstract void Dispose();
}
