using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script creates a reference to the storage object and calls methods on it. 
//This was a workaround to allow calling methods with arguments through the unity event system.
//Written by Travis
public class StorageFinder : MonoBehaviour
{
	public GameObject storage;
	public Inven storageInven;
	GameObject player;
	
	tempHolder tH;
	void Start()
	{
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")){
			if(g.GetComponent<tempHolder>() != null){
				player = g;
			}
		}
		if(this.transform.parent.parent.parent.GetComponent<UiPlugger>() != null){
			storage = this.transform.parent.parent.parent.GetComponent<UiPlugger>().inven.gameObject;
		}
		if(storage.GetComponent<Inven>() != null){
			storageInven = storage.GetComponent<Inven>();
		}
		if(player.GetComponent<tempHolder>() != null){
			tH = player.GetComponent<tempHolder>();
		}
		
	}
	public void SendDropItem(){
		string[] coords2 = this.gameObject.transform.parent.name.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		storageInven.DropSpecificItem(row, column, storageInven.array[row, column].Ammo);
	}
	public void SendDropAllItems(){
		//Debug.Log("MAde it to storage finder");
		string[] coords2 = this.gameObject.transform.parent.name.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		storageInven.DropWholeStack(row, column, storageInven.array[row, column].Ammo);
		tH.ClearSlot();
	}
	//public void SendSwap() {	 
		//	tH.Swap(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
		//}
	public void SendPickUp(){
		tH.HoldItem(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
	}
	public void SendReleaseItem(){
		//Debug.Log("Made it to release Item in storage Finder");
		tH.DropItem(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
	}
	public void TryShiftClickCheck(){
		//Debug.Log("Shift click check #1");
		string[] coords2 = this.gameObject.transform.parent.name.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		tH.ShiftClickCheck(storage.GetComponent<Inven>(), row, column, storage.GetComponent<Inven>().array[row, column].Ammo);
	}
}
