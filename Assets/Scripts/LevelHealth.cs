using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHealth : MonoBehaviour
{
    public static int levelHealth = 100;
    public Slider mySlider;
    public Image myImage;
  
    void Update()
    {
        mySlider.value = levelHealth;
        if (levelHealth < 10)
        {
            myImage.enabled = false;
        }
        else
        {
            myImage.enabled = true;
        }
    }
}