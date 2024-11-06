using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    const int MAX_HP = 100;

    public Slider HPSlider;

    void Start()
    {
        HPSlider.maxValue = MAX_HP;
        HPSlider.value = MAX_HP;
    }
}
