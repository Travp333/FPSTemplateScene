using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//This script ensures the Inventories UI changes alongside the backend. There are various methods here to change the name, image, and count of inventory slots.
//Written by Travis

//NOTE consider reversing naming scheme to make inventory stack from top again instead, that makes my brain happier
public class UiPlugger : MonoBehaviour
{
	[SerializeField]
	Transform placeHolderButton;
 	[SerializeField]
	Transform canvasBG;
	[SerializeField]
	GameObject buttonPrefab;
	[SerializeField]
	public Inven inven;
	[SerializeField]
	public List<GameObject> slots = new List<GameObject>();
	public List<Vector3> slotsPos = new List<Vector3>();
	//public GameObject[] slots;
    UIReferenceHolder reff;
    int i = 0;
	bool firstSlotSkip;
	[SerializeField]
	int hPadding = 100;
	[SerializeField]
	int vPadding = 100;
	//creates buttons for storage devices
	public void SpawnButtonsStorage(){
		//iterating through columns
		//Debug.Log("Beginning to iterate through Inven...");
		for (int i = 0; i < inven.vSize; i++)
		{
			if (firstSlotSkip)
			{
				//Debug.Log("Performing firstslotskip");
				placeHolderButton.position = placeHolderButton.position + new Vector3((vPadding * inven.hSize), hPadding, 0);
			}
			//iteraeing through rows
			for (int i2 = 0; i2 < inven.hSize; i2++)
			{
				//Debug.Log("Performing button creation...");
				firstSlotSkip = true;
				GameObject g = Instantiate(buttonPrefab, this.gameObject.GetComponent<Canvas>().transform);
				g.transform.position = placeHolderButton.position;
				placeHolderButton.position = placeHolderButton.position - new Vector3(vPadding, 0, 0);
				g.transform.SetParent(canvasBG.transform.parent);
				g.name = (i + "," + i2);
				slots.Add(g);
				slotsPos.Add(g.transform.position);
			}
		}
		firstSlotSkip = false;
		//Debug.Log("Final Step");
		Vector3 avg = GetMeanVector(slotsPos);
		RectTransform rt = canvasBG.gameObject.GetComponent (typeof (RectTransform)) as RectTransform;
		rt.sizeDelta = new Vector2 (78*(inven.hSize ), 78 * (inven.vSize ));
		rt.position = avg;
		//rt.anchorMin = new Vector2(1,0);
		//rt.anchorMax = new Vector2(0,1);
		//rt.pivot = new Vector2(.5f, .5f);
	}
	
	public void SpawnButtonsPlayer(){
		//iterating through colomns
		for (int i = 0; i < inven.vSize; i++)
		{
			//Debug.Log("Making Columns!");
			if(firstSlotSkip){
				//Debug.Log("shifting placeholder down one row and resetting y");
				placeHolderButton.position = placeHolderButton.position - new Vector3((vPadding * inven.hSize), -hPadding, 0);
			}
			//iteraeing through rows
			for (int i2 = 0; i2 < inven.hSize; i2++)
			{
				firstSlotSkip = true;
				GameObject g = Instantiate(buttonPrefab, this.gameObject.GetComponent<Canvas>().transform);
				g.transform.position = placeHolderButton.position;
				placeHolderButton.position = placeHolderButton.position + new Vector3(vPadding, 0, 0);
				g.transform.SetParent(canvasBG.transform.parent);
				g.name = (i+","+i2);
				slots.Add(g);
				slotsPos.Add(g.transform.position);
			}
		}
		firstSlotSkip = false;
			
		Vector3 avg = GetMeanVector(slotsPos);
		RectTransform rt = canvasBG.gameObject.GetComponent (typeof (RectTransform)) as RectTransform;
		rt.sizeDelta = new Vector2 (78*(inven.hSize ), 78 * (inven.vSize ));
		rt.position = avg;
		//rt.anchorMin = new Vector2(1,0);
		//rt.anchorMax = new Vector2(0,1);
		//rt.pivot = new Vector2(.5f, .5f);
	}
	public void ChangeItem(int row, int column, Sprite img, int count, string name, Stack<GameObject> ammo, int maxAmmo){
		//Debug.Log(slots.Count + this.gameObject.name);
		foreach(GameObject g in slots){

			if (slots[i].name == row + "," + column)
			{
				reff = slots[i].GetComponent<UIReferenceHolder>();
				reff.button.GetComponent<UnityEngine.UI.Image>().sprite = img;
				reff.text.GetComponent<TextMeshProUGUI>().text = name;
				reff.count.GetComponent<TextMeshProUGUI>().text = "x" + count;
				if (ammo != null)
				{
					if (maxAmmo > -1)
					{
						//Debug.Log("Made it to ChangeItem with a magazine, heres name: " + name + "heres maxAmmo: " + maxAmmo + " and Ammo: " + ammo.Count+ " and heres the ratio" + (float)ammo.Count / (float)maxAmmo);
						UpdateMagSlider(true, (float)ammo.Count / (float)maxAmmo);
					}
					else
					{
						UpdateMagSlider(false, 0);
					}
				}
				else
				{
					UpdateMagSlider(false, 0);
				}
            }
            i++;
        }
        i = 0;
    }
	void UpdateMagSlider(bool enabled, float value)
	{
		//Debug.Log("Running UpdateMagSlider, here are values: " + enabled + ", " + value);
		reff.slider.SetActive(enabled);
		reff.slider.GetComponent<Slider>().value = value;
	}
	//this is used when simply changing the amount of an inventory object.
	public void UpdateItem(int row, int column, int count, Stack<GameObject> ammo, int maxAmmo)
	{
		foreach (GameObject g in slots)
		{
			if (slots[i].name == row + "," + column)
			{
				reff = slots[i].GetComponent<UIReferenceHolder>();
				reff.count.GetComponent<TextMeshProUGUI>().text = "x" + count;
				if (ammo != null)
				{
					if (maxAmmo > -1)
					{
						//Debug.Log("Made it to Update with a magazine, heres name: " + name + "heres maxAmmo: " + maxAmmo + " and Ammo: " + ammo.Count + " and heres the ratio" + (float)ammo.Count / (float)maxAmmo);
						UpdateMagSlider(true, (float)ammo.Count / (float)maxAmmo);
					}
					else
					{
						UpdateMagSlider(false, 0);
					}
				}
				else
				{
					UpdateMagSlider(false, 0);
				}

			}
			i++;
		}
		i = 0;
	}
	//this is used when clearing all data from a slot
	public void ClearSlot(int row, int column, Sprite emp){
        foreach(GameObject g in slots){
			if (slots[i].name == row + "," + column)
			{
				reff = slots[i].GetComponent<UIReferenceHolder>();
				reff.button.GetComponent<UnityEngine.UI.Image>().sprite = emp;
				reff.text.GetComponent<TextMeshProUGUI>().text = "";
				reff.count.GetComponent<TextMeshProUGUI>().text = "x0";
				//set slider value to 0, hide it.
				UpdateMagSlider(false, 0);
            }
            i++;
        }
        i = 0;
    }
	//This is called to give feedback to the player when they simply press a button, ie dropping an object
	public void ButtonPress(int row, int column){
		ButtonSelected(row, column);
		StartCoroutine(ExecuteAfterTime(.05f, row, column));
	}
	//this allows me to execute code after a delay
	IEnumerator ExecuteAfterTime(float time, int row, int column)
	{
		 yield return new WaitForSeconds(time);
		 ButtonDeselected(row, column);
		 // Code to execute after the delay
	}
	//this is to give feedback for when a button has been selected, ie it has been stored in a temp slot preparing for a swap
	
	public void ButtonSelected(int row, int column) {
		//Debug.Log("Made it into button selected");
        foreach (GameObject g in slots)
        {
            if (slots[i].name == row + "," + column)
            {
            	//Debug.Log("Button Selected!");
            	
            	//this could probably be switched to just set the color to a set value, like color.grey, but the 
            	//multiplication is nice to ensure the methods are running the intended amount of times 
            	
	            reff = slots[i].GetComponent<UIReferenceHolder>();
	            reff.button.GetComponent<UnityEngine.UI.Image>().color *= .5f;
            }
            i++;
        }
        i = 0;
    }
	//this is for when a button has been deselected, ie it has just finished swapping and needs to return to normal state
    public void ButtonDeselected(int row, int column)
    {
        foreach (GameObject g in slots)
        {
            if (slots[i].name == row + "," + column)
            {
            	//Debug.Log("Button Deselected!");
	            reff = slots[i].GetComponent<UIReferenceHolder>();
	            reff.button.GetComponent<UnityEngine.UI.Image>().color *= 2f;
            }
            i++;
        }
        i = 0;
    }
	private Vector3 GetMeanVector(List<Vector3> positions)
	{
		if(positions.Count == 0)
		{
			return Vector3.zero;
		}
 
		Vector3 meanVector = Vector3.zero;
 
		foreach(Vector3 pos in positions)
		{
			meanVector += pos;
		}
 
		return (meanVector / positions.Count);
	}

}
