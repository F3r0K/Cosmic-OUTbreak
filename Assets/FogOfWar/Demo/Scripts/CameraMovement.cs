using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] float m_MaxZoomDistance= 25;
    [SerializeField] float m_MinZoomDistance=10;
    [SerializeField] float m_ZoomSpeed = 300;
    [SerializeField] float m_Angle=30;

    [Header("Movement")]
    [SerializeField] float m_MoveSpeed=20;
    [SerializeField] float m_BorderDistance = 5;
    [SerializeField] float m_MinX=-64;
    [SerializeField] float m_MaxX=64;
    [SerializeField] float m_MinY=-64;
    [SerializeField] float m_MaxY=64;

    Transform m_Child;
    float m_CurrentZoomLevel;
    private void Start()
    {
        m_CurrentZoomLevel = m_MaxZoomDistance;
        m_Child = transform.GetChild(0);
    }
    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        HandleMovement();
    }

    void HandleZoom() {
        float zoomdelta = Input.GetAxis("Mouse ScrollWheel") * m_ZoomSpeed * Time.deltaTime;

        m_CurrentZoomLevel = Mathf.Clamp(m_CurrentZoomLevel - zoomdelta, m_MinZoomDistance, m_MaxZoomDistance);

        m_Child.transform.localPosition = Quaternion.AngleAxis(m_Angle, -m_Child.transform.right) * Vector3.up * m_CurrentZoomLevel;
        m_Child.transform.localEulerAngles = new Vector3(90 - m_Angle, 180, 0);
    }

    void HandleMovement() {
        Vector2 screenpos =  Input.mousePosition*100.0f;
        screenpos.x /= (float)Screen.width;
        screenpos.y /= (float)Screen.height;


        if(screenpos.x>=0 && screenpos.x <= 100.0f){
            if (screenpos.x < m_BorderDistance)
            {
                transform.position -= Vector3.left * Time.deltaTime * m_MoveSpeed;
            }
            if (screenpos.x > 100 - m_BorderDistance)
            {
                transform.position -= Vector3.right * Time.deltaTime * m_MoveSpeed;
            }
        }
        if (screenpos.y >= 0 && screenpos.y <= 100.0f)
        {
           
                if (screenpos.y < m_BorderDistance)
                {
                    transform.position -= Vector3.back * Time.deltaTime * m_MoveSpeed;
                }
                if (screenpos.y > 100 - m_BorderDistance)
                {
                    transform.position -= Vector3.forward * Time.deltaTime * m_MoveSpeed;
                }
            
            
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, m_MinX, m_MaxX), transform.position.y, Mathf.Clamp(transform.position.z, m_MinY, m_MaxY));
    }
}
