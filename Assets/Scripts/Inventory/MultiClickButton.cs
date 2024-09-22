using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
//This script was pulled from the internet, it allows buttons to be interacted with in more ways than just left clicking
//gotten here: https://answers.unity.com/questions/993336/ui-button-detecting-right-mouse-button.html
//Written by Cherno, modified by Travis

//TODO I WANNA MAKE A SLIGHT DELAY WHEN YOU HOLD BEFORE IT ACTAULLY STARTS HOLDING SO THAT WHEN YOU TAP BUTTON THE LITTLE BAR DOESN SHOW UP
//When dragging from a storage object the icon should be on the top not below the menu
//clicking and dragging from the menu to the backdrop should drop the item 
//make multiples of items stack together on ground rust styled 
//left click brings up an inspect menu that shows a description and a 3d model 
//fix shift clicking 
public class MultiClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {
 
	public UnityEvent leftDown;
	public UnityEvent leftClick;
	public UnityEvent middleClick;
	public UnityEvent rightClick;
	public UnityEvent rightHold;
	public UnityEvent leftRelease;
	public UnityEvent leftReleaseInvalid;
	public UnityEvent middleHold;
	private bool pointerDown;
	private float pointerDownTimer;
	[SerializeField]
	private float requiredHoldTime;
	[SerializeField]
	private Image fillImage;
	PointerEventData eventData2;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private Canvas canvas;
	private Vector3 startingPos;
	private bool leftBlock;
	private string heldItemName;
	
	
	private void Awake()
	
	{
		
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		
	}
	
	public void OnBeginDrag(PointerEventData eventData){
		//if object isnt an empty slot
		startingPos = this.transform.position;
		if(GetComponent<Image>().sprite.name != "empty"){
			
			GetComponent<StorageFinder>().storage.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>().sortingOrder = 999;
			
			
			heldItemName = GetComponent<Image>().sprite.name;
			startingPos = this.transform.position;
			this.transform.parent.transform.SetSiblingIndex(this.transform.parent.transform.parent.transform.childCount);
			canvas = this.gameObject.GetComponent<StorageFinder>().storageInven.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>();
			//Debug.Log("Begin Drag");
			canvasGroup.alpha = .8f;
			canvasGroup.blocksRaycasts = false;
			leftDown.Invoke();
		}
		else{
			Debug.Log("Grabbed empty");
			this.transform.position = startingPos;
		}
	}
	public void OnDrag(PointerEventData eventData){
		
		if(GetComponent<Image>().sprite.name != "empty"){
			GetComponent<StorageFinder>().storage.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>().sortingOrder = 999;
			//Debug.Log("On Drag");
			rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		}
		else{
			Debug.Log("Dragging Empty");
		}
	}
	public void OnEndDrag(PointerEventData eventData){
		GetComponent<StorageFinder>().storage.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>().sortingOrder = 0;
		Debug.Log("End Drag on "+ heldItemName);
		heldItemName = null;
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		//if(eventData != null){
			//	this.transform.position = startingPos;
			//}
		
	}
	public void OnDrop(PointerEventData eventData){
		GetComponent<StorageFinder>().storage.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>().sortingOrder = 0;
		Debug.Log("End Drop on " + this.transform.parent.name + " and " + heldItemName);
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		leftRelease.Invoke();
		heldItemName = null;
		if(eventData != null){
			eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
		}
		else{
			Debug.Log("INVALID DROP POS!!!");
		}
	}
	
	public void resetPosition(){
		if(startingPos != null){
			GetComponent<StorageFinder>().storage.GetComponent<Inven>().UIPlugger.GetComponent<Canvas>().sortingOrder = 0;
			this.transform.position = startingPos;
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = true;
			Reset();
		}
	}
	public void quickDropAll(){
		//Debug.Log("Made it into multiclickbutton");
		rightHold.Invoke();
		Reset();
		
	}
	
	public void Update(){
		//Mouse button is being held down
		if(pointerDown && GetComponent<Image>().sprite.name != "empty"){
			//Debug.Log(GetComponent<Image>().sprite.name);
			if(eventData2.button == PointerEventData.InputButton.Left){
				if(!leftBlock){
					leftClick.Invoke();
					leftBlock = true;
				}
			}
			//increment timer while mouse button besided left click is held down
			if(eventData2.button != PointerEventData.InputButton.Left){
				pointerDownTimer += Time.deltaTime;
				//have we held down the button long enough?
				if(pointerDownTimer >= requiredHoldTime){
					//if so, was it a right click?
					if(eventData2 != null && eventData2.button == PointerEventData.InputButton.Right){
						//invoke right hold event
						rightHold.Invoke();
						//reset the timer and UI
						Reset();
					}
					//was is a middle mouse click?
					if(eventData2 != null && eventData2.button == PointerEventData.InputButton.Middle){
						//invoke middle mouse hold event
						middleHold.Invoke();
						//reset the timer and UI
						Reset();
					}
				}
				//update UI only if its a right or middle click
				if(eventData2 != null){
					if(eventData2.button != PointerEventData.InputButton.Left){
						fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
					}
				}
			}
		}
	}
	//reset the timer and UI
	public void Reset(){
		//Debug.Log("eventdata2 set to Null");
		eventData2 = null;
		pointerDown = false;
		pointerDownTimer = 0;
		fillImage.fillAmount = pointerDownTimer/requiredHoldTime;
	}
	//called when releasing mouse button 
	public void OnPointerUp(PointerEventData eventData){
		leftBlock = false;
		//check to be sure the event exists, ie reset ran too early or something
		if(eventData2 != null){
			if(eventData2 != null){
				if(eventData2.button != PointerEventData.InputButton.Left){
					//was it released before the bar filled up? if so, just do the normal click event.
					if(pointerDownTimer < requiredHoldTime && eventData2 != null){
		
						if(eventData2.button == PointerEventData.InputButton.Right){
							rightClick.Invoke();
						}
						if(eventData2.button == PointerEventData.InputButton.Middle){
							middleClick.Invoke();
						}
					}
					Reset();
				}
			}
		}
		
	}
	//flips bool and stores event data when a mouse button is held
	public void OnPointerDown(PointerEventData eventData){
		pointerDown = true;
		eventData2 = eventData;
		//Debug.Log("eventdata2 set to " + eventData2.button);
	}

	//public void OnPointerClick(PointerEventData eventData)
	//{
	//	if (eventData.button == PointerEventData.InputButton.Left){
	//		leftClick.Invoke();
	//	}	 	
	//	else if (eventData.button == PointerEventData.InputButton.Middle){
	//		middleClick.Invoke();
	//	}
	//	else if (eventData.button == PointerEventData.InputButton.Right){
	//			rightClick.Invoke();
	//		}
	//}
}