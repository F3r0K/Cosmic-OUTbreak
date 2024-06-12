using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionBlockComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Should this vision blocker get updated in realtime")]
    bool m_UpdateOnTransform=false;
    [SerializeField]
    [Tooltip("The interval for updating the visionblocker in case of a transform update in seconds")]
    float m_UpdateTimeInterval = 0.1f;

    //Previous Transform
    Vector3 PreviousPosition;
    Vector3 PreviousRotation;
    Vector3 PreviousScale;

    Collider m_Collider;
    void Start()
    {

        PreviousPosition = transform.position;
        PreviousRotation = transform.eulerAngles;
        PreviousScale = transform.lossyScale;
        m_Collider = GetComponent<Collider>();
        FogOfWarManager.INSTANCE.SetVisionBlocker(m_Collider);


        if(m_UpdateOnTransform)
            InvokeRepeating("UpdateVisionBlocker", m_UpdateTimeInterval, m_UpdateTimeInterval);
    }

    private void OnDestroy()
    {
        if(FogOfWarManager.INSTANCE)
            FogOfWarManager.INSTANCE.RemoveVisionBlocker(m_Collider);
    }

    void UpdateVisionBlocker() {
        if (HasTransformUpdated() && m_UpdateOnTransform) //If we moved, we will update the vision
        {
            PreviousPosition = transform.position;
            PreviousRotation = transform.eulerAngles;
            PreviousScale = transform.lossyScale;

            FogOfWarManager.INSTANCE.SetVisionBlocker(m_Collider);
        }
    }

    bool HasTransformUpdated() {
        if (PreviousPosition != transform.position)
            return true;
        if (PreviousRotation != transform.eulerAngles)
            return true;
        if (PreviousScale != transform.lossyScale)
            return true;

        return false;
    }
}
