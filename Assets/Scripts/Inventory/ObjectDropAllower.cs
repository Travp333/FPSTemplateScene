using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class ObjectDropAllower : MonoBehaviour, IDropHandler
{
	public void OnDrop(PointerEventData eventData){
		//Debug.Log("Get DROPPED ON SON");
		eventData.pointerDrag.GetComponent<MultiClickButton>().quickDropAll();
		eventData.pointerDrag.GetComponent<MultiClickButton>().resetPosition();
		
	}
}
