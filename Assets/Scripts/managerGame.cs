using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class managerGame : MonoBehaviour
{
    public static int coins;
    public Text textCoin;


    private void Update()
    {
        textCoin.text = "Монет: " + coins;
    }
}
