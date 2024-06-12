using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DamagePlayer : MonoBehaviour
{
    private bool isTrig = false;
   private Animator anim;
    public AudioClip myClip;
    private AudioSource mySource;
    void Start()
    {
        anim = GetComponent<Animator>();
        mySource = GetComponent<AudioSource>();
    }

  void OnTriggerEnter(Collider myCollider)
    {
        if (myCollider.tag == ("Player"))
        {
            isTrig = true;
            StartCoroutine(Attack());
          
            
          
        }
    }
    private void OnTriggerExit(Collider myCollider)
    {
        if (myCollider.tag == ("Player"))
        {
            isTrig = false;
            



        }
    }
    IEnumerator Attack()
    {
        if(isTrig == true)
        {
            isTrig = false;
            yield return new WaitForSeconds(0.7f);
            anim.SetTrigger("Attack");
            mySource.PlayOneShot(myClip);
            LevelHealth.levelHealth -= 10;
            isTrig = true;
        }
        
    }
  
}