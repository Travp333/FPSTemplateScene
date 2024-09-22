using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ObjectDropBlocker : MonoBehaviour, IDropHandler
{
	tempHolder tmp;
	protected void Start()
	{
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")){
			if(g.GetComponent<tempHolder>() != null){
				tmp = g.GetComponent<tempHolder>();
			}
		}
	}
	public void OnDrop(PointerEventData eventData){
		eventData.pointerDrag.GetComponent<MultiClickButton>().resetPosition();
		tmp.ClearSlot();
		//Debug.Log("Dropped a thing heres a bunch of info " + eventData.pointerDrag.transform.parent.name);
	}
}
