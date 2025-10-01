using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ItemStatAndCoords
{
    //created a class so that I can return one value with multiple values within
    public ItemStat itemStat;
    public int row;
    public int column;
}

public class AmmoManager : MonoBehaviour
{
    ItemStatAndCoords statAndCoord;
    List<GameObject> tempAmmoList = new List<GameObject>();
    public ItemStat loadedMagazine;
    Item magItem;
    Inven playerInv;
    int loopCounter;
    [SerializeField]
    string ammoType;
    // Start is called before the first frame update
    void Start()
    {
        statAndCoord = new ItemStatAndCoords();
		foreach (Inven i in GameObject.FindObjectsByType<Inven>(FindObjectsSortMode.None))
        {
            if (i.gameObject.tag == "Player")
            {
                playerInv = i;
            }
        }
    }
    ItemStatAndCoords GrabMagazine()
    {
        for (int row = 0; row < playerInv.vSize; row++)
        {
            //iterating through rows
            for (int column = 0; column < playerInv.hSize; column++)
            {
                //is the item in this inventoryslot a magazine? is it the same ammo type? have we searched the entire inventory yet?
                if ((playerInv.array[row, column].itemType == "Magazine") && (playerInv.array[row, column].ammoType == ammoType) && (loopCounter <= (playerInv.hSize * playerInv.vSize)))
                {
                    Debug.Log("Found Valid Magazine! " + playerInv.array[row, column].Objname + ", " + row + ", " + column + ", " + playerInv.array[row, column].itemType + ", " + playerInv.array[row, column].ammoType);
                    //update class info
                    //Debug.Log("Is this null?" + playerInv.array[row, column].Objname);
                    statAndCoord.itemStat = playerInv.array[row, column];
                    statAndCoord.row = row;
                    statAndCoord.column = column;
                    return statAndCoord;
                }
                else
                {
                    //Debug.Log("Could not find valid Magazine in inventory slot");
                }
            }
        }
        Debug.Log("Could not find valid Magazine in entire inventory");
        return null;
    }
    //only need ItemStat and not item since you cant go from a world object straight into your gun, it would need to be placed in your inventory, then loaded. 
    public void UpdateMagazine(ItemStat magazine)
    {
        //store magazine object that was loaded
        loadedMagazine = magazine;
    }
    public GameObject FireBullet()
    {
        if (loadedMagazine.Ammo.Count > 0 && loadedMagazine != null)
        {
            //store a temp for that one bullet
            GameObject g = loadedMagazine.Ammo[loadedMagazine.Ammo.Count - 1];
            //delete that bullet from the stack
            loadedMagazine.Ammo.Remove(loadedMagazine.Ammo[loadedMagazine.Ammo.Count - 1]);
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
        ItemStatAndCoords m = GrabMagazine();
        if (m != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UnloadMagazine()
    {
        playerInv.SmartPickUp(loadedMagazine);
        loadedMagazine = null;
    }
    public void LoadMagazine()
    {
        
    }
    public void Reload()
    {
        //check for a valid magazine object in inventory
        ItemStatAndCoords m = GrabMagazine();
        // check if you got a valid return or not
        if (m != null)
        {
            //clear the slot the old magazine is in
            playerInv.NullInvenSlot(m.row, m.column);
            //updating UI to match new change
            playerInv.plug.ClearSlot(m.row, m.column, playerInv.temp.emptyImage);
            //place empty magazine back in inventory
            
            //replace held ammo list with ammo list from new magazine and update loaded mag variable
            UpdateMagazine(m.itemStat);
            Debug.Log("Reloaded!");
        }
        else
        {
            Debug.Log("Failed to find magazine");
        }
    }


}
