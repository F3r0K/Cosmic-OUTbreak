using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementComponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_WalkableMask;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            MapClick(Input.mousePosition);
        }
    }

    void MapClick(Vector2 position) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,100f, m_WalkableMask))
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = hit.point;
        }
    }
}
