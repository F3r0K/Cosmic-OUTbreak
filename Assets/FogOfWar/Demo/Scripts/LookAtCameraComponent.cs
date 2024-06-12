using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraComponent : MonoBehaviour
{
    Transform m_MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = -(m_MainCamera.position - transform.position).normalized;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180, transform.localEulerAngles.z);
    }
}
