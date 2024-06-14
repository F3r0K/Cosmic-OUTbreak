using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickUpItems : MonoBehaviour
{
    public ItemManager itemManager;
    public AudioClip AudioPickUP;
  
    public enum Res
    {
        Metall = 0,
        Plastic = 1,
        Scraplow = 2,
        ScrapMed = 3,
        ScrapHigh = 4
    }
    public Res Items;
    public int Cost = 1;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            
            switch (Items)
            {
             
                case Res.Metall:
                    itemManager.metall += Cost;
                    GetComponent<AudioSource>().PlayOneShot(AudioPickUP);
                    
                    Destroy(this.gameObject);
                    break;


                case Res.Plastic:
                    itemManager.plastic += Cost;
                    GetComponent<AudioSource>().PlayOneShot(AudioPickUP);
                  
                    Destroy(this.gameObject);
                    break;

                case Res.Scraplow:
                    itemManager.Scraplow += Cost;
                    GetComponent<AudioSource>().PlayOneShot(AudioPickUP);
                   
                    Destroy(this.gameObject);
                    break;


                case Res.ScrapHigh:
                    itemManager.ScrapHigh += Cost;
                    GetComponent<AudioSource>().PlayOneShot(AudioPickUP);
                    
                    Destroy(this.gameObject);
                    break;

                case Res.ScrapMed:
                    itemManager.ScrapMed += Cost;
                    GetComponent<AudioSource>().PlayOneShot(AudioPickUP);
                  
                  
                    Destroy(this.gameObject);
                    break;


            }
            
        }
    }
}
