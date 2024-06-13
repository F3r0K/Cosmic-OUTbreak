using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemManager : MonoBehaviour
{
    public Text MetallText;
    public Text plasticText;
    public Text ScrapLowText;
    public Text ScrapMedText;
    public Text ScrapHighText;

    [Space]

    public int metall;
    public int plastic;
    public int Scraplow;
    public int ScrapMed;
    public int ScrapHigh;

    public void ClickUpdateInfo()
    {
        MetallText.text = "�������: " + metall;
        plasticText.text = "�������: " + plastic;
        ScrapLowText.text = "������ ������� ��������: " + Scraplow;
        ScrapMedText.text = "������ �������� ��������: " + ScrapMed;
        ScrapHighText.text = "������ �������� ��������: " + ScrapHigh;
    }
}
