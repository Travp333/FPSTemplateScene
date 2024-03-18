//Author: Sandeep
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldShiftUI : MonoBehaviour
{

    //assigns the WorldShift class to worldShift to use its variables
	//public WorldShift worldShift;
    //assigns the PlayerStats class to playerStats to use its variables
    public PlayerStats playerStats;


    // Update is called once per frame
   // void Update()
    //{
      //if it is not possible to shift, or if the shift is blocked, or if there is not enough hunger to shift,
      //the sphere will be gray
      //if (worldShift.possibleShift == false || worldShift.shiftBlocked == true || playerStats.hunger <= worldShift.shiftCost)
      //{
       //   GetComponent<RawImage>().color = Color.gray;
      //}

      //if the Player is in the real world, the sphere will be blue
      //else if (worldShift.hollOrReal == false)
      //{
      //    GetComponent<RawImage>().color = Color.blue;
     // }

      //if the Player is in Holl, the sphere will be purple
      //else if (worldShift.hollOrReal == true)
     // {
     //   GetComponent<RawImage>().color = new Color32(201,0,255,255);
    //  }

   // }
}
