using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField]
    int magazineSize;
    public int ammoInMag;
    [SerializeField]
    public int ammoCount;
    // Start is called before the first frame update
    public bool FireBullet(){
        if(ammoInMag >= 1){
            ammoInMag = ammoInMag - 1;
            return true;
        }
        else{
            return false;
        }
    }
    public bool CanReload(){
        if(ammoInMag < magazineSize && ammoCount >= 1){
            return true;
        }
        else{
            return false;
        }
    }
    public void Reload(){
        //is the magazine already full? do you have any ammo?
        Debug.Log("Reloading!");
        if(ammoInMag < magazineSize && ammoCount >= 1){
            //is the magazine empty? do you have enough ammo to fill your magazine?
            if(ammoInMag < 1 && ammoCount >= magazineSize){
                //deduct a magazines worth of ammo from ammo count
                ammoCount = ammoCount - magazineSize;
                //refill the ammo in the magazine
                ammoInMag = magazineSize;
            }
            //is there ammo in your magazine still? do you have anough ammo to fill your magazine?
            else if(ammoInMag > 0 && ammoCount >= (magazineSize - ammoInMag)){
                //deduct the amount of ammo you need to fill your magazine from your ammo count
                ammoCount = ammoCount - (magazineSize - ammoInMag);
                //refill the ammo in the magazine
                ammoInMag = magazineSize;
            }
            //does you not have enough ammo to completely fill your magazine?
            else if (ammoCount < (magazineSize - ammoInMag)){
                //add the remaining ammoCount to the magazine, since we know its less than enough
                ammoInMag = ammoInMag + ammoCount;
                //zero out ammo count since its not enough
                ammoCount = 0;
            }
        }
    }
    void Start()
    {
        ammoInMag = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
