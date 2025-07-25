﻿//Author: Travis Parks, Brian Meginness
//Debugging: Travis Parks, Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// This script deals with holding objects after you have interacted with an object that has a rigidbody, is tagged as pickupable, and isn't over your max carrying weight.
// It pins that object to an empty tied to the player, creates a collider to represent that object while it is in your hands, and switches the player to an animation set to 
// reflect that they are holding something. This script also handles the logic for throwing objects, including charging up and releasing
public class Grab : MonoBehaviour
{
    //Components
    HandAnim hand;
    Movement movement;
	public InputAction attackAction;
    [HideInInspector]
    public bool isHolding = false;
    [SerializeField]
    public float throwingforce = 5;
    
    //Throwing variables
    [Tooltip("the point that a fully charged throw will head toward")]
    [SerializeField]
    Transform LowthrowingPoint;
    [SerializeField]
    [Tooltip("the point that a light toss will head toward")]
    Transform HighthrowingPoint;
    [SerializeField]
    bool highorLow = true;
    [HideInInspector]
    public float throwingTemp;
    [SerializeField]
    [Tooltip("the heaviest possible object the player can pick up")]
    public float strength;
    [SerializeField]
    [Tooltip("the maximum force the player can throw an object at, when fully charged")]
    float maxThrowingForce;
    [SerializeField]
    [Tooltip("the rate at which the players throw charges")]
    float chargeRate;
    [HideInInspector]
    public bool isgrabCharging = false;
    //Object sizes
    public enum objectSizes{tiny, small, medium, large, none};
    [HideInInspector]
    public objectSizes sizes;
    public Interact interact;
    [HideInInspector]
    public bool justThrew;
	void Start() {
        //Set components
        interact = GetComponent<Interact>();
        throwingTemp = throwingforce;
		movement = transform.root.GetComponent<Movement>();
		attackAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Attack");

        hand = GetComponent<HandAnim>();
    }
    void setisThrowingFalse(){
        hand.setisThrowingFalse();
    }
    void resetJustThrew(){
        justThrew = false;
    }
    public void pickUp(Transform dummy, Transform prop, Rigidbody propRB, GameObject propGame)
    {      
        //Get size of held object
        if(propGame.GetComponent<objectSize>().sizes == objectSize.objectSizes.large){
            sizes = objectSizes.large;
        }
        if(propGame.GetComponent<objectSize>().sizes == objectSize.objectSizes.medium){
            sizes = objectSizes.medium;
        }
        if(propGame.GetComponent<objectSize>().sizes == objectSize.objectSizes.small){
            sizes = objectSizes.small;                
        }     
        if(propGame.GetComponent<objectSize>().sizes == objectSize.objectSizes.tiny){
            sizes = objectSizes.tiny;
        }              
        //trigger animation
        hand.setisHoldingTrue();
        // move the hit object to the grab point
        prop.position = dummy.transform.position;
        // set the hit object to be a child of the grab point
        prop.SetParent(dummy);
        // get a reference to the custom gravity rigidbody to disable gravity and sleeping
        propRB.isKinematic=(true);
        isHolding = true;
        // set the held object to the "nocollidewithplayer" layer to prevent clipping with the player
        propGame.layer = 16;
        // do the same for all children and childrens children 
        foreach ( Transform child in prop){
            child.transform.gameObject.layer = 16;
            foreach ( Transform child2 in child.transform){
                child2.transform.gameObject.layer = 16;
            }
        }
    }
    void Update()
    {
        //IF not paused
        if (!FindFirstObjectByType<PauseMenu>().isPaused)
        {
            //IF Left Mouse released and is holding an object
	        if (attackAction.WasReleasedThisFrame() && isHolding && !justThrew)
            {
                //Remove from grip
                interact.detach();
                //Add appropriate force to object
                if (highorLow)
                {
	                interact.propRB.AddForce((HighthrowingPoint.position - interact.origin.transform.position).normalized * throwingforce, ForceMode.Impulse);
                }
                else
                {
	                interact.propRB.AddForce(this.transform.forward * throwingforce, ForceMode.Impulse);
                }
                // trigger animation
                hand.setisThrowingTrue();
                //prepare to reset animation
                Invoke("setisThrowingFalse", .1f);
                throwingforce = throwingTemp;
                highorLow = true;
                justThrew = true;
                Invoke("resetJustThrew", .5f);
            }
            //IF Left Mouse pressed and is holding an object
	        if (attackAction.IsPressed() && isHolding && !justThrew)
            {
                // Start incrementing throwing force
                if (throwingforce <= maxThrowingForce)
                {
                    isgrabCharging = true;
                    throwingforce = throwingforce + (chargeRate * Time.deltaTime) * 100;
                }
                if (throwingforce > maxThrowingForce)
                {
                	throwingforce = maxThrowingForce;
                    isgrabCharging = false;
                    highorLow = false;
                }
            }
        }
    }
}
