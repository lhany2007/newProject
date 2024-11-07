using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SliderManager : MonoBehaviour
{
    public List<Slider> SliderList;

    void Awake()
    {
        foreach (Slider slider in SliderList)
        {
            slider.interactable = false;
        }
    }
}
