using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    Stack<GameObject> tempAmmoList = new Stack<GameObject>();
    Item magItem;
    Inven playerInv;
    int loopCounter;
    [SerializeField]
    string ammoType;
    //temp copies of itemstat properties
    //-------------------------------------------------

	public string LoadedMagazineObjname = "";
	public float LoadedMagazineweight = 0;
	public int LoadedMagazineAmount = 0;
	public int LoadedMagazinestackSize = 0;
	public GameObject LoadedMagazineprefab = null;
	public Sprite LoadedMagazineimg = null;
    public string LoadedMagazineitemType = "";
    [SerializeField]
	public Stack<GameObject> LoadedMagazineAmmo = new Stack<GameObject>();
	//what is the type of the ammo? ie 9mm, 8x22, shotgun, etc
	public string LoadedMagazineammoType = "";
    // if it is a magazine, how much ammo can this magazine hold?
    public int LoadedMagazineammoSize = -1;
    //-------------------------------------------------------------
    public bool loadedMag;
    ItemStat I;
    GameObject player;
    //basically set it up to be how a real gun functions, reloading simply replaces the magazine, but racking the slide replaces the next round in the chamber.
    // now when i do OutofAmmo checks i am checking this one round, not the ammo count
    [SerializeField]
    // made it an array so that top loaded / individual loaded weapons like the DB or a SKS could still work using this logic, just put everything 
    // in the round in chamber slot and skip the magazine logic altogether. 
    public Stack<GameObject> roundInChamber = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        statAndCoord = new ItemStatAndCoords();
        foreach (Inven i in GameObject.FindObjectsByType<Inven>(FindObjectsSortMode.None))
        {
            if (i.gameObject.tag == "Player")
            {
                playerInv = i;
                player = i.gameObject;
            }
        }

    }

    // this is assuming an autoload, in the case of like a pump shotgun or a bolt action this would need to be modified / replaced
    // something is wrong here, it is sorting the ammo stack for some reason and firing all the shotgun shells first, then the 8x22 ammo idk weird
    public GameObject FireBullet()
    {
        Debug.Log("Called Fire Bullet");
        if (roundInChamber.Count > 0)
        {
            Debug.Log("Firing this bullet: " + roundInChamber.Peek().name);
            //store a temp for that one bullet
            GameObject g = roundInChamber.Peek();
            // GameObject g = roundInChamber[roundInChamber.Count - 1];
            //delete that bullet from the stack
            roundInChamber.Pop();
            //roundInChamber.Remove(roundInChamber[roundInChamber.Count - 1]);
            // is there another bullet in the chamber
            if(LoadedMagazineAmmo.Count > 0)
            {
                Debug.Log("Loading this bullet: " + LoadedMagazineAmmo.Peek().name);
                // take the top bullet from your magazine and place in in the chamber
                roundInChamber.Push(LoadedMagazineAmmo.Pop());
                //roundInChamber.Add(LoadedMagazineAmmo[LoadedMagazineAmmo.Count - 1]);
                // delete a bullet from the magazine to simulate loading the next available round
                //LoadedMagazineAmmo.Remove(LoadedMagazineAmmo[LoadedMagazineAmmo.Count - 1]);
            }

            //return the temp value to the gun so it knows which round to fire.
            return g;
        }
        else
        {
            return null;
        }
    }
    public void CopyLoadedMagDataFromX(ItemStat X )
    {
        LoadedMagazineObjname = X.Objname;
        LoadedMagazineweight = X.weight;
        LoadedMagazineAmount = X.Amount;
        LoadedMagazinestackSize = X.stackSize;
        LoadedMagazineprefab = X.prefab;
        LoadedMagazineimg = X.img;
        LoadedMagazineitemType = X.itemType;
        LoadedMagazineammoType = X.ammoType;
        LoadedMagazineammoSize = X.ammoSize;
        LoadedMagazineAmmo = X.Ammo;
        loadedMag = true;
    }
    public void CopyLoadedMagDataToI()
    {
        I = null;
        I.Objname = LoadedMagazineObjname;
        I.weight = LoadedMagazineweight;
        I.Amount = LoadedMagazineAmount;
        I.stackSize = LoadedMagazinestackSize;
        I.prefab = LoadedMagazineprefab;
        I.img = LoadedMagazineimg;
        I.itemType = LoadedMagazineitemType;
        I.ammoType = LoadedMagazineammoType;
        I.ammoSize = LoadedMagazineammoSize;
        I.Ammo = LoadedMagazineAmmo;
    }
    public void ClearLoadedMagData()
    {
        LoadedMagazineObjname = "";
        LoadedMagazineweight = 0;
        LoadedMagazineAmount = 0;
        LoadedMagazinestackSize = 0;
        LoadedMagazineprefab = null;
        LoadedMagazineimg = null;
        LoadedMagazineitemType = "";
        LoadedMagazineammoType = "";
        LoadedMagazineAmmo = new Stack<GameObject>();
        LoadedMagazineammoSize = -1;
        loadedMag = false;
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
    //These get called via animation, that way it can be interrupted
    public void UnloadMagazine()
    {
        Debug.Log("Called Unload Magazine " + Time.deltaTime);
        CopyLoadedMagDataToI();
        playerInv.SmartPickUp(I);
        Debug.Log("Changed loadedMagazine in Unload Magazine from: " + LoadedMagazineObjname + " to null");
        ClearLoadedMagData();
    }
    //These get called via animation, that way it can be interrupted
    public void LoadMagazine()
    {
        Debug.Log("Called Load Magazine " + Time.deltaTime);
        //check for a valid magazine object in inventory
        ItemStatAndCoords m = GrabMagazine();
        // check if you got a valid return or not
        if (m != null)
        {
            Debug.Log("??"+m.itemStat.Ammo.Count);
            UpdateMagazine(m);
            // V V below is making itemstat.ammo null before rackslide for some reason 
            //clear the slot the old magazine is in
            playerInv.NullInvenSlot(m.row, m.column);
            //updating UI to match new change
            playerInv.plug.ClearSlot(m.row, m.column, playerInv.temp.emptyImage);
            Debug.Log("From Load Magazine " + LoadedMagazineAmmo.Count);
            Debug.Log("Reloaded!");
        }
        else
        {
            Debug.Log("Failed to find magazine");
        }
    }
    ItemStatAndCoords GrabMagazine()
    {
        Debug.Log("Called Grab Magazine " + Time.deltaTime);
        for (int row = 0; row < playerInv.vSize; row++)
        {
            //iterating through rows
            for (int column = 0; column < playerInv.hSize; column++)
            {
                //is the item in this inventoryslot a magazine? is it the same ammo type? have we searched the entire inventory yet?
                if ((playerInv.array[row, column].itemType == "Magazine") && (playerInv.array[row, column].ammoType == ammoType) && (loopCounter <= (playerInv.hSize * playerInv.vSize)))
                {
                    Debug.Log("Found Valid Magazine! " + playerInv.array[row, column].Objname + ", " + row + ", " + column + ", " + playerInv.array[row, column].itemType + ", " + playerInv.array[row, column].ammoType + ", " + playerInv.array[row, column]+ ", " + playerInv.array[row, column].Ammo.Count);
                    //update class info
                    //Debug.Log("Is this null?" + playerInv.array[row, column].Objname);
                    statAndCoord.itemStat = playerInv.array[row, column];
                    statAndCoord.row = row;
                    statAndCoord.column = column;
                    Debug.Log(statAndCoord.itemStat.Ammo.Count);
                    return statAndCoord;
                }
                else
                {
                    //Debug.Log("Could not find valid Magazine in inventory slot");
                }
            }
        }
        Debug.Log("Could not find valid magazine in entire inventory");
        return null;
    }
        //only need ItemStat and not item since you cant go from a world object straight into your gun, it would need to be placed in your inventory, then loaded. 
    public void UpdateMagazine(ItemStatAndCoords m)
    {
        Debug.Log("Called Update Magazine " + Time.deltaTime);
        if (m != null)
        {
            Debug.Log("Changed loadedMagazine in Update Magazine from: " + LoadedMagazineObjname + " to " + m.itemStat.Objname);
            CopyLoadedMagDataFromX(m.itemStat);
            //Debug.Log("Why is this null?? " + magazine.Ammo.Count);
            Debug.Log("From update Magazine " + LoadedMagazineAmmo.Count);

        }
        else
        {
            Debug.Log("Called Update Magazine with an invalid magazine!");
        }


    }
    public void RackSlide()
    {
        Debug.Log("Called Rack Slide ");
        

        if (LoadedMagazineAmmo != null)
        {
            if (LoadedMagazineAmmo.Count > 0)
            {

                //store a temp for one bullet
                GameObject g = LoadedMagazineAmmo.Peek();
                //GameObject g = LoadedMagazineAmmo[LoadedMagazineAmmo.Count - 1];
                //delete that bullet from the stack
                LoadedMagazineAmmo.Pop();
                //LoadedMagazineAmmo.Remove(LoadedMagazineAmmo[LoadedMagazineAmmo.Count - 1]);
                // is there a bullet in the chamber already?
                if (roundInChamber.Count > 0)
                {
                    //spawn the existing bullet on the ground, idk when this would fire but thats how it would work irl so putting it here just in case. 
                    Instantiate(roundInChamber.Pop(), this.transform.position, Quaternion.identity);
                    //remove that bullet from the list
                    //roundInChamber.Remove(roundInChamber[roundInChamber.Count - 1]);
                    // add the new bullet from the top of the loaded magazine into the chamber
                    roundInChamber.Push(g);
                }
                else
                {
                    // add the new bullet from the top of the loaded magazine into the chamber
                    roundInChamber.Push(g);
                }
                Debug.Log("After Calling Rack slide, here is RoundinChamber count and name of first entry: " + roundInChamber.Count + ", " + roundInChamber.Peek().name + ", and here is loaded magazine name and count: " + LoadedMagazineObjname + ", " + LoadedMagazineAmmo.Count);
            }
            else
            {
                Debug.Log("No ammo in magazine!");
            }
        }
        else
        {
            Debug.Log("LoadedMagazineAmmo is null for some reason: " + LoadedMagazineObjname + ", " + LoadedMagazineammoType);
        }
        
    }


}
