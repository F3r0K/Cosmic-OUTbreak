using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Craft : MonoBehaviour
{
    public ItemManager ItemManager;
    public Item items;
    public GameObject gun;
    public Transform CraftPoint;
    public Text Crafttext;
    public int TimeCraft = 10;
    public AudioClip CraftAudio;
   public void Craft1()
    {
      if(ItemManager.metall >= 500)
        {
           
            ItemManager.metall -= 500;
            StartCoroutine(StartCraft());
        }
            

        
    }
    IEnumerator StartCraft()
    {
        Crafttext.text = "Оружие создаётся:Пожалуйста подождите";
        GetComponent<AudioSource>().PlayOneShot(CraftAudio);
        yield return new WaitForSeconds(TimeCraft);
        Instantiate(gun, CraftPoint);
        Crafttext.text ="Оружие Создано! Возьмите его на Площадке";
        yield return new WaitForSeconds(5);
        Crafttext.text = "";
    }
}
