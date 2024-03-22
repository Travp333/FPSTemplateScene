using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    bool playerIsIn;
    public ListOfActions action;
    public GameObject mainObject;
    public GameObject secondaryObject;
    public float speed;
    string actionCommand;
	public bool OpenDoor;
    Vector3 mainObjectInitialPosition;
    bool moveSwap;
    float moveTimeElapsed;
    // Start is called before the first frame update
    void Start()
    {
        mainObjectInitialPosition = mainObject.transform.position;
        actionCommand = "None";
        moveSwap = false; // move main object towards secondary object
        moveTimeElapsed = 0f;
    }
	
	public void Interact(){
		OpenDoor = true;
	}
    // Update is called once per frame
	void FixedUpdate()
    {
	    if (OpenDoor)
	    {
		    SetActionCommand();
		    OpenDoor = false;
        }
        MasterFunction();
    }

    private void SetActionCommand()
    {
        switch (action)
        {
            case ListOfActions.slidingDoor: actionCommand = "Sliding Door"; break;
        }
    }

    private void MasterFunction()
    {
        if (actionCommand == "Sliding Door")
        {
            if (!moveSwap)
            {
                MoveMainObjectToTargetPosition(mainObjectInitialPosition, secondaryObject.transform.position);
	            
            }
            else if (moveSwap)
            {
                MoveMainObjectToTargetPosition(secondaryObject.transform.position, mainObjectInitialPosition);
            }
        }
    }

    private void MoveMainObjectToTargetPosition(Vector3 initialPos, Vector3 targetPos)
    {
        moveTimeElapsed += Time.deltaTime;
        mainObject.transform.position = Vector3.Lerp(initialPos, targetPos, moveTimeElapsed / speed);

	    if (Vector3.Distance(mainObject.transform.position, targetPos) <= .001f)
        {
            actionCommand = "None";
            moveSwap = !moveSwap;
            moveTimeElapsed = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsIn = false;
        }
    }

    public enum ListOfActions
    {
        slidingDoor
    }
}
