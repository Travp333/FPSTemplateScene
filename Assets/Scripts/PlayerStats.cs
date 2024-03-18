//adapted and modified from video "How to make a HEALTH BAR in Unity!" by Brackeys
//https://www.youtube.com/watch?v=BLfNP4Sc_iA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script will just keep track of the player's various stats and allow other scripts to access and edit them
public class PlayerStats : MonoBehaviour
{
    public float hp = 100;
    public HealthBar healthBar;
    public PlayerDeath playerDeath;

    public void restoreHP(float healAmount){
        if (hp + healAmount > 100){
            hp = 100;
        }
        else{
            hp += healAmount;
        }
    }

    public void takeDamage(float damage){
        if (hp - damage < 0){
            //Debug.Log("Went from "+hp+" to 0");
            hp = 0;
            playerDeath.Death();

        }
        else {

            //Debug.Log("Went from "+hp+" to "+ Mathf.Round(hp-damage));
            if(hp != Mathf.Round(hp-damage)){
                hp = Mathf.Round(hp-damage);
            }
        }
    }

    void Start()
    {
        //Test line to see if we can set a default start point
        //when game is started, it sets the slider max value to hp value
        healthBar.SetMaxHealth(hp);
    }

    void Update()
    {
        //updates the slider value to match the current hp value
        healthBar.SetHealth(hp);
    }

}
