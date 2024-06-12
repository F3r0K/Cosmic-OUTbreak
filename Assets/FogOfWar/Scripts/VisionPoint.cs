using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct VisionPoint
{
    [FieldOffset(0)]
    public float m_Range;
    [FieldOffset(4)]
    public Vector3 m_Position;
}

public class Vision
{
    float m_Range;
    Vector3 m_Position;

    public Vision(float range)
    {
        m_Range = range;
    }

    public void SetPosition(Vector3 worldposition) {
        m_Position = worldposition;
    }

    public void SetRange(float range) {
        m_Range = range;
    }

    public VisionPoint GetVisionPoint() {
        VisionPoint visionPoint;
        visionPoint.m_Range = m_Range;
        visionPoint.m_Position = m_Position;

        return visionPoint;
    }
}