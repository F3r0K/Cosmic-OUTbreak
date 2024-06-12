using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class VisionHandler
{
    List<Vision> m_Visions = new List<Vision>();
    List<VisionPoint> m_VisionPoints = new List<VisionPoint>();

    private static VisionHandler instance;
    public static VisionHandler INSTANCE
    {
        get
        {
            if (instance == null)
            {
                instance = new VisionHandler();
            }
            return instance;
        }
    }


    public void Dispose()
    {
        if (INSTANCE == this)
        {
            instance = null;
        }
    }

    public List<VisionPoint> GetVisionPoints() {
        for (int i = 0; i < m_Visions.Count; i++)
        {
            VisionPoint visionPoint = m_Visions[i].GetVisionPoint();

            m_VisionPoints[i] = visionPoint;
        }


        return m_VisionPoints;
    }

    public void AddVision(Vision visionPoint)
    {
        m_Visions.Add(visionPoint);

        m_VisionPoints.Add(visionPoint.GetVisionPoint());
    }

    public void RemoveVision(Vision visionPoint)
    {
        m_Visions.Remove(visionPoint);
        m_VisionPoints.RemoveAt(0);
    }

}
