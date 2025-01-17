using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemStackChecker : MonoBehaviour
{
    //stackmodels are not updating!
    int ID;
    public bool block;
    private void Start()
    {
        ID = GetInstanceID();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("CollisioN!");
        //is this a valid object?
        if(other.GetComponent<ItemStackChecker>()!= null){
            //Debug.Log("Collision with valid Object " + other.GetComponent<ItemStackChecker>().transform.parent.name);
            //are these two of the same type?
            pickUpableItem otherPickUp;
            pickUpableItem pickUp;
            otherPickUp = other.transform.parent.gameObject.GetComponent<pickUpableItem>();
            pickUp = transform.parent.gameObject.GetComponent<pickUpableItem>();
            if(otherPickUp.item.name == pickUp.item.name){
                //Debug.Log("Collision with " + otherPickUp.count + " of same item " + pickUp.item + ", " + this.transform.parent.gameObject.name + ", " + other.transform.parent.gameObject.name, transform.parent.gameObject );
                //if yes, delete the "other" one
                if(other.transform.parent.gameObject != null){
                    if(pickUp.count + otherPickUp.count <= pickUp.item.stackSize){
                        if(!block){
                            other.GetComponent<ItemStackChecker>().block = true;
                            pickUp.count += otherPickUp.count;
                            Destroy(other.transform.parent.gameObject);
                            otherPickUp.count = 0;
                            pickUp.EditCount(pickUp.count, pickUp.item.name);
                            //Debug.Log("Deleted Valid other object, now have " + pickUp.count + ", " + pickUp.item.name + " in " + this.transform.parent.gameObject.name, transform.parent.gameObject );
                        }
                        else{
                           // Debug.Log("BLOCKED");
                        }
                    }
                    else if (pickUp.count < pickUp.item.stackSize && otherPickUp.count < otherPickUp.item.stackSize){
                        otherPickUp.count = (pickUp.count + otherPickUp.count) - pickUp.item.stackSize;
                        pickUp.count = pickUp.item.stackSize;
                        pickUp.EditCount(pickUp.count, pickUp.item.name);
                        //Add some logic here to stack remainders, ie 80 + 70 is too much for a 99 max, so create one stack of 99 and one stack of 51

                    }
                }
            }
        }
    }

}