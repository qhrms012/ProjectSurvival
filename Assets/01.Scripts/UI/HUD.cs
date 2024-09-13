using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health}
    public InfoType type;


    TextMeshProUGUI myText;
    Slider mySlider;

    private void Awake()
    {
        mySlider = GetComponent<Slider>();
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:

                break;
            case InfoType.Level:

                break;
            case InfoType.Kill:

                break;
            case InfoType.Time:

                break;
            case InfoType.Health:

                break;

        }
    }
}
