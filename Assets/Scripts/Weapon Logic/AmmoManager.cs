using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    //change all this shit to just be a reference to the magazine itself 
    [SerializeField]
    List<GameObject> Ammo = new List<GameObject>();
    public GameObject loadedMagazine;
    Item magItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void UpdateMagazine(GameObject magazine)
    {
        //store magazine object that was loaded
        loadedMagazine = magazine;
        //carry over ammo list from loaded in magazine
        if (loadedMagazine != null)
        {
            if (loadedMagazine.GetComponent<AmmoList>() != null)
            {
                Ammo = loadedMagazine.GetComponent<AmmoList>().Ammo;
            }
            else
            {
                Debug.Log("Loaded a magazine without an ammolist component!");
            }
            if (loadedMagazine.GetComponent<pickUpableItem>() != null)
            {
                magItem = loadedMagazine.GetComponent<pickUpableItem>().item;
            }
        }
        else
        {
            Debug.Log("No Magazine Loaded!");
        }
    }
    public GameObject FireBullet()
    {
        if (Ammo.Count > 0 && loadedMagazine != null)
        {
            //store a temp for that one bullet
            GameObject g = Ammo[Ammo.Count - 1];
            //delete that bullet from the stack
            Ammo.Remove(Ammo[Ammo.Count - 1]);
            //return the temp value to the gun so it knows which round to fire.
            return g;
        }
        else
        {
            return null;
        }
    }
    public bool CanReload()
    {
        if (Ammo.Count < magItem.ammoSize && Ammo.Count >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Reload(){
        //is the magazine already full? do you have any ammo?
        
        if(Ammo.Count < magItem.ammoSize && Ammo.Count >= 1){
            //is the magazine empty? do you have enough ammo to fill your magazine?
            if (Ammo.Count < 1 && Ammo.Count >= magItem.ammoSize)
            {
                //deduct a magazines worth of ammo from ammo count
                Ammo.Count = Ammo.Count - magItem.ammoSize;
                //refill the ammo in the magazine

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                Ammo.Count = magItem.ammoSize;
                Debug.Log("Reloaded!");
            }
            //is there ammo in your magazine still? do you have anough ammo to fill your magazine?
            else if (Ammo.Count > 0 && Ammo.Count >= (magItem.ammoSize - Ammo.Count))
            {
                //deduct the amount of ammo you need to fill your magazine from your ammo count
                Ammo.Count = Ammo.Count - (magItem.ammoSize - Ammo.Count);
                //refill the ammo in the magazine

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                //Ammo.Count = magItem.ammoSize;
                Debug.Log("Reloaded!");
            }
            //does you not have enough ammo to completely fill your magazine?
            else if (Ammo.Count < (magItem.ammoSize - Ammo.Count))
            {
                //add the remaining ammoCount to the magazine, since we know its less than enough

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                //Ammo.Count = Ammo.Count + ammoCount;
                //zero out ammo count since its not enough
                Ammo.Count = 0;
                Debug.Log("Reloaded!");
            }
        }
    }


}
