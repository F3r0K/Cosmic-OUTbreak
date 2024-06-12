using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The range of this vision component in the fog of war")]
    float m_VisionRange=5;

    Vision m_Vision;
    void Start()
    {
        m_Vision = new Vision(m_VisionRange);
        VisionHandler.INSTANCE.AddVision(m_Vision);
    }

    // Update is called once per frame
    void Update()
    {
        m_Vision.SetPosition(transform.position);
        m_Vision.SetRange(m_VisionRange);
    }

    private void OnDestroy()
    {
        VisionHandler.INSTANCE.RemoveVision(m_Vision);
    }
}
