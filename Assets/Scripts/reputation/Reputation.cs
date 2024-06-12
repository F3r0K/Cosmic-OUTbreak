using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reputation : MonoBehaviour
{
    public int currentRep = 0;
    public int maxRep = 100;
    public int lvlRep;

    public Slider Slideer;

    private void Start()
    {
        Slideer.maxValue = maxRep;
        Slideer.value = currentRep;
    }
}
