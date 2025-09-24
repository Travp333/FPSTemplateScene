using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script allows inventory objects to be stored in a 2D array. This script should be able to be placed on both a player and a storage device. 
//This scirpt handles all the storage and manipulation of this array on the backend, ie picking up or dropping an object.
//Written by Conor and Travis

//This holds all the info we pull from a valid inventory object. We effectively copy the date from an Item object, then paste it here to be stored in a inventory slot
public class ItemStat
{

	public string Objname = "";
	public float weight = 0;
	public int Amount = 0;
	public int stackSize = 0;
	public GameObject prefab = null;
	public Sprite img = null;
	public string itemType;
	public List<GameObject> Ammo = new List<GameObject>();
	//what is the type of the ammo? ie 9mm, 8x22, shotgun, etc
	public string ammoType;
	// if it is a magazine, how much ammo can this magazine hold?
	public int ammoSize = -1;
}
public class Inven : MonoBehaviour
{
	[SerializeField]
	[Tooltip("What object to spawn in the player's inventory (match with startingInvenCount, ie startingInvenPrefabs[0] will spawn startingInvenCount[0] times")]
	public GameObject[] startingInvenPrefabs;
	[SerializeField]
	public int[] startingInvenCount;
	[Tooltip("how many objects to spawn in the player's inventory (match with startingInvenCount, ie startingInvenPrefabs[0] will spawn startingInvenCount[0] times")]
	[SerializeField]
	Transform droppedItemSpawnPoint;
    [SerializeField]
    public GameObject UIPlugger;
    [SerializeField]
    public int hSize = 4;
    [SerializeField]
    public int vSize = 4;
    public bool isPickedUp = false;
    [HideInInspector]
    public Item item;
	public ItemStat [,] array;
	public UiPlugger plug;
	public tempHolder temp;
	int loopCounter;
	int column;
	int i3;
	RecyclableItem recyclableItem;
	public void Start()
	{
		temp = FindFirstObjectByType<tempHolder>();
		//stores reference to Ui object
		//Debug.Log("STEP #1 in " + this.gameObject.name);
		//This creates our 2D array based on the size given in editor
		array = new ItemStat[vSize,hSize];
		for (int row = 0; row < vSize; row++)
        {
			//Debug.Log("STEP #2 in " + this.gameObject.name);
			for (int column = 0; column < hSize; column++)
			{

				//Debug.Log("STEP #3 in " + this.gameObject.name);
				array[row, column] = new ItemStat();
				array[row, column].img = temp.emptyImage;
			}
        }
		//Debug.Log("STEP #4 in " + this.gameObject.name);
		Invoke("lateStart", .1f);
	}
	//Runs .1 second after start, allows inventory slots and objects to be created before creating UI reference and filling inventory with starting objects
	public void lateStart(){
		//Reference to UI object, allows inventory to update Ui elemnents 
		plug = UIPlugger.GetComponent<UiPlugger>();
		//Fills inventory with objects placed in startingInvenPrefabs in editor ( any were placed, basically just lets you force an inventory to start with objects already in it)
		foreach( GameObject g in startingInvenPrefabs){
			for (int i = 0; i < startingInvenCount[column]; i++) {
				//if it has an ammolist component, then we know it is a magazine. if so, pass the ammo list
				if (g.GetComponent<AmmoList>() != null)
				{
					SmartPickUp(g.GetComponent<pickUpableItem>().item, g.GetComponent<AmmoList>().Ammo);
					if (isPickedUp)
					{
						//Debug.Log("Successfull pickup!");
						isPickedUp = false;
					}
					else
					{
						//Debug.Log("No room in inventory, dropping on floor");
						SpawnItem(g.GetComponent<pickUpableItem>().item.prefab, g.GetComponent<AmmoList>().Ammo);
					}
				}
				// not a magazine, then pass null so it does not matter. 
				else
				{
					SmartPickUp(g.GetComponent<pickUpableItem>().item, null);
					if (isPickedUp)
					{
						//Debug.Log("Successfull pickup!");
						isPickedUp = false;
					}
					else
					{
						//Debug.Log("No room in inventory, dropping on floor");
						SpawnItem(g.GetComponent<pickUpableItem>().item.prefab, null);
					}
				}
			}
			column++;
		}
		column = 0;
		
	}
	//ItemStat refers to the object while it is in your inventory, just he information about that item stored. 
	//Item refers to the object in the overworld, the physical item you pick up

	//This method just copied data from one object to another. This could be Item to ItemStat, or ItemStat to ItemStat
	// basically this method is called when you pick up an object from the world and when you pick one up from an inventory
	// Item is an object in the world, ItemStat is an object in an inventory. Effectively same thing, just presented differently
	public void CopyItemData(int row, int column, Item item, List<GameObject> Ammo)
	{
		array[row, column].Objname = item.Objname;
		array[row, column].weight = item.weight;
		array[row, column].Amount = array[row, column].Amount + 1;
		array[row, column].stackSize = item.stackSize;
		array[row, column].prefab = item.prefab;
		array[row, column].img = item.img;
		array[row, column].itemType = item.itemType;
		array[row, column].ammoType = item.ammoType;
		array[row, column].ammoSize = item.ammoSize;
		array[row, column].Ammo = Ammo;
	}
	//Overload method for itemstat objects
	public void CopyItemData(int row, int column, ItemStat item)
	{
		array[row, column].Objname = item.Objname;
		array[row, column].weight = item.weight;
		array[row, column].Amount = array[row, column].Amount + 1;
		array[row, column].stackSize = item.stackSize;
		array[row, column].prefab = item.prefab;
		array[row, column].img = item.img;
		array[row, column].itemType = item.itemType;
		array[row, column].ammoType = item.ammoType;
		array[row, column].ammoSize = item.ammoSize;
		array[row, column].Ammo = item.Ammo;
	}
	//This method clears all the info from a given inventory slot
	public void NullInvenSlot(int row, int column)
	{
		array[row, column].Objname = "";
		array[row, column].weight = 0;
		array[row, column].Amount = 0;
		array[row, column].stackSize = 0;
		array[row, column].prefab = null;
		array[row, column].img = temp.emptyImage;
		array[row, column].itemType = "";
		array[row, column].ammoType = "";
		array[row, column].ammoSize = -1;
		array[row, column].Ammo = null;
	}

	//This handles picking up a new valid Inventory Item 
	//Item Objects are items in the world attached to 3d models storing info, ie the physical coin
	public void PickUp(Item item, List<GameObject> Ammo)
	{
		//iterating through colomns
		for (int row = 0; row < vSize; row++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int column = 0; column < hSize; column++)
			{
				//Debug.Log("Row" + column);
				//is this slot empty?
				if (array[row, column].Objname == "")
				{
					//yes empty, filling slot
					//Debug.Log("Slot (" + i + " , "+ column + " ) is empty, putting " + item.Objname + " in slot");
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					CopyItemData(row, column, item, Ammo);
					//updating UI to match new change
					plug.ChangeItem(row, column, item.img, array[row, column].Amount, array[row, column].Objname, Ammo, array[row,column].ammoSize);
					row = 0;
					column = 0;
					return;
				}
				//no theres something here
				else
				{
					//Debug.Log("Slot (" + i + " , " + column + " ) has " + array[row,column].Amount + " " + array[row,column].Objname + " in it, checking if it matches the new " + item.Objname);
					//basically is there room for it, is it the same object
					if (array[row, column].Objname == item.Objname && array[row, column].stackSize! >= array[row, column].Amount + 1)
					{
						//Debug.Log("Slot (" + i + " , "+ column + " ) has room, adding " + item.Objname + " to stack");
						//same object, room in the stack, adding to stack
						isPickedUp = true;
						//Debug.Log("ispickedup set to "+ isPickedUp);
						array[row, column].Amount = array[row, column].Amount + 1;
						//Debug.Log("we now have " + array[row,column].Amount + " "+ array[row,column].Objname + " in " + "Slot (" + i + " , "+ column + " ) ");
						//updating UI to match new change
						plug.UpdateItem(row, column, array[row, column].Amount, Ammo, array[row,column].ammoSize);
						row = 0;
						column = 0;
						return;
					}
					else if (array[row, column].stackSize <= array[row, column].Amount + 1)
					{
						//Debug.Log("cant hold more than " + array[row,column].Amount + " " + array[row,column].Objname + " in one stack, starting new stack... ");
					}
					//otherwise theres something here but its not the same type or theres no room for it
				}
			}
		}
	}
	
	//Overload that allows picking up itemstat objects
	// ItemStat Objects are inventory objects that are already stored in an inventory, ie the coin you just picked up from inside a box
	public void PickUp(ItemStat item){
		//iterating through colomns
		for (int row = 0; row < vSize; row++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int column = 0; column < hSize; column++)
			{
				//Debug.Log("Row" + column);
				//is this slot empty?
				if(array[row,column].Objname == ""){
					//yes empty, filling slot
					//Debug.Log("Slot (" + i + " , "+ column + " ) is empty, putting " + item.Objname + " in slot");
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					CopyItemData(row, column, item);
					//updating UI to match new change
					plug.ChangeItem(row, column, item.img, array[row,column].Amount, array[row,column].Objname, item.Ammo, array[row,column].ammoSize);
					row=0;
					column=0;
					return;
				}
				//no theres something here
				else{
					//Debug.Log("Slot (" + i + " , " + column + " ) has " + array[row,column].Amount + " " + array[row,column].Objname + " in it, checking if it matches the new " + item.Objname);
					//basically is there room for it, is it the same object
					if(array[row,column].Objname == item.Objname && array[row,column].stackSize !>= array[row,column].Amount + 1){
						//Debug.Log("Slot (" + i + " , "+ column + " ) has room, adding " + item.Objname + " to stack");
						//same object, room in the stack, adding to stack
						isPickedUp = true;
						//Debug.Log("ispickedup set to "+ isPickedUp);
						array[row,column].Amount = array[row,column].Amount + 1;
						//Debug.Log("we now have " + array[row,column].Amount + " "+ array[row,column].Objname + " in " + "Slot (" + i + " , "+ column + " ) ");
						//updating UI to match new change
						plug.UpdateItem(row, column, array[row,column].Amount, item.Ammo, array[row,column].ammoSize);
						row=0;
						column=0;
						return;
					}
					else if(array[row,column].stackSize <= array[row,column].Amount + 1){
						//Debug.Log("cant hold more than " + array[row,column].Amount + " " + array[row,column].Objname + " in one stack, starting new stack... ");
					}
					//otherwise theres something here but its not the same type or theres no room for it
				}
			}
		}
	}
	//this handles picking up new inventory items in a way that tries to prioritize existing stacks and falls back to the default pickup method if not necessary
	public void SmartPickUp(Item item, List<GameObject> Ammo)
	{
		//Debug.Log("Starting "+ this.gameObject.Objname + " with a " + item.Objname);
		//iterating through colomns
		for (int row = 0; row < vSize; row++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int column = 0; column < hSize; column++)
			{
				if ((array[row, column].Objname == item.Objname) && (loopCounter <= (hSize * vSize)) && (array[row, column].stackSize! >= array[row, column].Amount + 1))
				{
					//found a stack of the existing item in inventory
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					array[row, column].Amount = array[row, column].Amount + 1;
					plug.UpdateItem(row,column,array[row,column].Amount, Ammo, array[row,column].ammoSize);
					row = 0;
					column = 0;
					loopCounter = 0;
					return;
				}
				else
				{
					//this slot doesnt have the same name or doesnt have space
					//Debug.Log(loopCounter);
					loopCounter++;
					if (loopCounter >= (hSize * vSize))
					{
						//Debug.Log("made it to finishLine");
						//searched whole inventory, nothing shares name, calling normal PickUp()
						PickUp(item, Ammo);
						loopCounter = 0;
						return;
					}
				}
			}
		}
	}
	//overload that allows picking up itemstat objects
	//this is for shift clicking from one inventory to another, prioritizes stacks but reverts to normal pickup if not necessary
	// do not need to directly referense the ammo list since we have it stored in the itemstat version, just not the item version. 
	public void SmartPickUp(ItemStat item)
	{
		//Debug.Log("Starting "+ this.gameObject.Objname + " with a " + item.Objname);
		//iterating through colomns
		for (int row = 0; row < vSize; row++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int column = 0; column < hSize; column++)
			{
				if ((array[row, column].Objname == item.Objname) && (loopCounter <= (hSize * vSize)) && (array[row, column].stackSize! >= array[row, column].Amount + 1))
				{
					//found a stack of the existing item in inventory
					// again, should not be possible for magazines to stack but the logic is still there just in case
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					array[row, column].Amount = array[row, column].Amount + 1;
					plug.UpdateItem(row, column, array[row, column].Amount, item.Ammo, array[row, column].ammoSize);
					row = 0;
					column = 0;
					loopCounter = 0;
					return;
				}
				else
				{
					//this slot doesnt have the same name or doesnt have space
					//Debug.Log(loopCounter);
					loopCounter++;
					if (loopCounter >= (hSize * vSize))
					{
						//Debug.Log("made it to finishLine");
						//searched whole inventory, nothing shares name, calling normal PickUp()
						PickUp(item);
						loopCounter = 0;
						return;
					}
				}
			}
		}
	}
	//This script actually Instantiates a new object with the same stats as the object stored in the inventory

	//used for dropping an inventory object out of your inventory into the world. currently works with item stacks forming immediatly
	// after dropping all the items in one spot but could be cleaned up a bit
	public GameObject SpawnItem(GameObject item, List<GameObject> ammo)
	{
		GameObject b = Instantiate(item, droppedItemSpawnPoint.position, this.transform.rotation);
		if (b.GetComponent<AmmoList>() != null)
		{
			if (ammo != null)
			{
				b.GetComponent<AmmoList>().Ammo = ammo;
			}
		}
		b.GetComponent<Rigidbody>().linearVelocity = this.gameObject.GetComponent<Rigidbody>().linearVelocity * 2f;
		return b;
	}
	//this drops one specific item that is found using its exact coordinates
	public void DropSpecificItem(int row, int column, List<GameObject> Ammo)
	{
		if (!((temp.tempRow == row) && (temp.tempColumn == column)))
		{
			//parsing incoming string into ints
			//This slot does in fact have an object in it
			if (array[row, column].Amount > 0)
			{
				//make the button flash grey for a second to give feedback
				plug.ButtonPress(row, column);
				//Debug.Log("Dropping one " + array[row, column].Objname + " from slot (" + row + " , "+ column + " ) , now we have" + (array[row,column].Amount - 1));
				//deduct one of the items from the stack
				array[row, column].Amount = array[row, column].Amount - 1;
				//spawn a prefab with the same info as that item
				SpawnItem(array[row, column].prefab, Ammo);
				//you just dropped the last item in that slot, reverting to default
				if (array[row, column].Amount <= 0)
				{
					//Debug.Log("Out of " + array[row, column].Objname + " in slot (" + row + " , "+ column + " ) , slot now empty ");
					NullInvenSlot(row, column);
					//updating UI to match new change
					plug.ClearSlot(row, column, temp.emptyImage);
				}
				else
				{
					//there are still more of that item in the slot, updating UI to match new change
					//( the magazine angle should be uneccesary here, as magazines do not stack, but ehh oh well)
					plug.UpdateItem(row, column, array[row, column].Amount, Ammo, array[row, column].ammoSize);
				}
				return;
			}
		}
	}
	//Drops entire stack of objects from given coordinates
	public void DropWholeStack(int row, int column, List<GameObject> Ammo){
		//This slot does in fact have an object in it
		if(array[row, column].Amount > 0){
			//make the button flash grey for a second to give feedback
			plug.ButtonPress(row, column);
			//spawn a prefab with the same info as that item
			for (int i = 0; i < array[row, column].Amount; i++) {
				SpawnItem(array[row, column].prefab, Ammo);
			}
			array[row, column].Amount = 0;
			//you just dropped the last item in that slot, reverting to default
			if(array[row, column].Amount <= 0){
				//Debug.Log("Out of " + array[row, column].Objname + " in slot (" + row + " , "+ column + " ) , slot now empty ");
				NullInvenSlot(row, column);
				//updating UI to match new change
				plug.ClearSlot(row, column, temp.emptyImage);
			}
			else{
				//there are still more of that item in the slot, updating UI to match new change
				//( the magazine angle should be uneccesary here, as magazines do not stack, but ehh oh well)
				plug.UpdateItem(row, column, array[row,column].Amount, Ammo, array[row,column].ammoSize);
			}
			return;
		}
		
	}
}
