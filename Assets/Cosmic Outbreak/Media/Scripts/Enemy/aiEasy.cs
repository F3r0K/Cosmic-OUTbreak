using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aiEasy : MonoBehaviour
{
    private Animator anim;
    private TopDownShooter.HitPoint hit;
    public GameObject player;
    private NavMeshAgent agent;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Walk");
        hit = GetComponent<TopDownShooter.HitPoint>();
        // Присваиваем найденный компонент NavMeshAgent переменной agent.
        agent = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
      
        // Проверяем, что agent не равен null, прежде чем использовать его.
        if (agent != null)
        {
           if(hit.CurrentHitPoint > 0)
            {
                player = GameObject.FindGameObjectWithTag("Player");

                
                agent.SetDestination(player.transform.position);
                 Vector3 direction = (player.transform.position - transform.position).normalized;
                 transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            }
           
        }
    }
}


