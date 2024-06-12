using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarComputeHandler : ComputeShaderHandler
{
    ComputeBuffer m_VisionBuffer;
    RenderTexture m_FogOfWarRenderTexture;
    RenderTexture m_CollisionTexture;

    int m_Width,m_Height;
    int m_MapWidth, m_MapHeight;
    float m_CellSize;
    Vector3 m_WorldPosition;

    int m_KernelHandle;

    public FogOfWarComputeHandler(ComputeShader computeShader,int width,int height,int mapwidth,int mapheight,Vector3 worldposition,RenderTexture collisionTexture) :base(computeShader)
    {
        m_Width = width;
        m_Height = height;
        m_MapWidth = mapwidth;
        m_MapHeight = mapheight;
        m_CollisionTexture = collisionTexture;
        m_WorldPosition = worldposition;
        m_CellSize = (float)m_MapWidth / (float)m_Width;
    }

    public override void Dispose()
    {
        m_VisionBuffer.Dispose();
    }

    public override void Initialize()
    {
        m_VisionBuffer = new ComputeBuffer(1024, sizeof(float) * 4);
        m_FogOfWarRenderTexture = CreateRenderTexture(m_Width, m_Height);


        m_KernelHandle = m_ComputeShader.FindKernel("CSFogOfWar");
        m_ComputeShader.SetTexture(m_KernelHandle, "Result", m_FogOfWarRenderTexture);
        m_ComputeShader.SetTexture(m_KernelHandle, "CollisionMask", m_CollisionTexture);
        m_ComputeShader.SetBuffer(m_KernelHandle, "VisionPoints", m_VisionBuffer);
        m_ComputeShader.SetFloat("CellSize", m_CellSize);
        m_ComputeShader.SetVector("WorldPosition", m_WorldPosition);
        m_ComputeShader.SetVector("Size", new Vector2(m_MapWidth, m_MapHeight));
    }

    public override RenderTexture Run()
    {
        m_VisionBuffer.SetData(VisionHandler.INSTANCE.GetVisionPoints().ToArray());
        m_ComputeShader.SetInt("VisionPointCount", VisionHandler.INSTANCE.GetVisionPoints().Count);
        m_ComputeShader.Dispatch(m_KernelHandle, m_Width / 8, m_Height / 8, 1);

        return m_FogOfWarRenderTexture;
    }
}
