using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]private int nub;
    public int maxValut;

    public GameObject Ammocam;
  
    public GameObject Gcam1;
    public GameObject CrossBowcam2;
    public GameObject MPcam3;
  
    public void Right()
    {
        nub++;

      
    }
    public void Left()
    {
        if(nub > 0)
        {
            nub--;
        }
                
        
    }
    public void ShowShop()
    {

        switch (nub)
        {
            case 0:
                Ammocam.SetActive(true);
                Gcam1.SetActive(false);
                CrossBowcam2.SetActive(false);
                MPcam3.SetActive(false);
               
                
                break;
            case 1:
                Ammocam.SetActive(false);
                Gcam1.SetActive(true);
                CrossBowcam2.SetActive(false);
                MPcam3.SetActive(false);
               

                break;
            case 2:
                Ammocam.SetActive(false);
                Gcam1.SetActive(false);
                CrossBowcam2.SetActive(true);
                MPcam3.SetActive(false);
               

                break;

            case 3:
                
                Ammocam.SetActive(false);
                Gcam1.SetActive(false);
                CrossBowcam2.SetActive(false);
                MPcam3.SetActive(true);
                break;

           
           

                


        }
       
     
    }
    public void Check()
    {
        if (nub > maxValut)
        {
            nub = 0;
        }
    }
}
