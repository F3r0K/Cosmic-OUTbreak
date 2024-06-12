using UnityEngine;
using UnityEngine.UI;

public class REPe : MonoBehaviour
{
    public int currectrep = 0;
    public int maxrep = 100;
    public int lvlRep= 0;
    public Slider slideer;
    public Text LvlText;

    private void Start()
    {
        slideer.maxValue = maxrep;
        slideer.value = currectrep;
        
    }
   public void OpenNoote()
    {
        if(slideer.maxValue <= slideer.value)
        {
            lvlRep++;
            LvlText.text = "Уровень: " + lvlRep;
           
            slideer.value = 0;
            currectrep = 0;
        }
    }
}
