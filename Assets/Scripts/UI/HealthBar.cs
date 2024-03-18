//adapted and modified from video "How to make a HEALTH BAR in Unity!" by Brackeys
//https://www.youtube.com/watch?v=BLfNP4Sc_iA
//Author: Sandeep
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  public Slider healthSlider;

  //method used in PlayerStats class to set the max value of the slider
  public void SetMaxHealth(float health)
  {
    //sets max value of health equal to health
    healthSlider.maxValue = health;
    //sets current value of health equal to health
    healthSlider.value = health;

  }
  //method used in the PlayerStats class to set the current value of health to the slider
  public void SetHealth(float health)
  {
    healthSlider.value = health;
  }
}
