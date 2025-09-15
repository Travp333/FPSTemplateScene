using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> Ammo = new List<GameObject>();
    [SerializeField]
    int magazineSize;
    [SerializeField]
    public int ammoCount;
    // Start is called before the first frame update
    public GameObject FireBullet()
    {
        if (Ammo.Count > 0)
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
    //Old version, just treating bullets as a number instead of as a game object
    //public bool FireBullet()
    // {
    //     if (ammoInMag >= 1)
    //     {
    //         ammoInMag = ammoInMag - 1;
    //         return true;
    //      }
    //     else
    //     {
    //          return false;
    //      }
    //  }
    public bool CanReload()
    {
        if (Ammo.Count < magazineSize && ammoCount >= 1)
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
        
        if(Ammo.Count < magazineSize && ammoCount >= 1){
            //is the magazine empty? do you have enough ammo to fill your magazine?
            if (Ammo.Count < 1 && ammoCount >= magazineSize)
            {
                //deduct a magazines worth of ammo from ammo count
                ammoCount = ammoCount - magazineSize;
                //refill the ammo in the magazine

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                //Ammo.Count = magazineSize;
                Debug.Log("Reloaded!");
            }
            //is there ammo in your magazine still? do you have anough ammo to fill your magazine?
            else if (Ammo.Count > 0 && ammoCount >= (magazineSize - Ammo.Count))
            {
                //deduct the amount of ammo you need to fill your magazine from your ammo count
                ammoCount = ammoCount - (magazineSize - Ammo.Count);
                //refill the ammo in the magazine

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                //Ammo.Count = magazineSize;
                Debug.Log("Reloaded!");
            }
            //does you not have enough ammo to completely fill your magazine?
            else if (ammoCount < (magazineSize - Ammo.Count))
            {
                //add the remaining ammoCount to the magazine, since we know its less than enough

                //RELOADING IS BROKEN UNTIL I MAKE IT LOAD IN AMMO FROM INVEN

                //Ammo.Count = Ammo.Count + ammoCount;
                //zero out ammo count since its not enough
                ammoCount = 0;
                Debug.Log("Reloaded!");
            }
        }
    }
    void Start()
    {
        //ammoInMag = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
