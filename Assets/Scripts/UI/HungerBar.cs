//adapted and modified from video "How to make a HEALTH BAR in Unity!" by Brackeys
//https://www.youtube.com/watch?v=BLfNP4Sc_iA
//Author: Sandeep
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Slider hungerSlider;

    //method used in PlayerStats class to set the max value of the slider
    public void SetMaxHunger(float hunger)
    {
      hungerSlider.maxValue = hunger;
      hungerSlider.value = hunger;

    }
    //method used in the PlayerStats class to set the current value of hunger to the slider
    public void SetHunger(float hunger)
    {
      hungerSlider.value = hunger;
    }
}
