using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class pickUpableItem : MonoBehaviour
// This has become too specific to coins, need to make more generic!
{
    public bool block;

    [SerializeField]
    public Item item;
    [SerializeField]
    public int count = 1;
    [SerializeField]
    public GameObject[] StackMeshes;
    public void DisableAllMeshes(){
        foreach (GameObject g in StackMeshes){
            if(g!=null){
                g.SetActive(false);
            }
        }
    }

    public void EditCount(int count2, string name){
        Debug.Log(item.Objname + " (" + this.gameObject.name +") now has " + count2 + ", took from " + name);
        count = count2;
        if (StackMeshes.Count() <= 0){
            //Debug.Log("Do nothing!");
        }
        else if(count >= StackMeshes.Count()){
            //Debug.Log("Maxing out Stack!");
            DisableAllMeshes();
            StackMeshes[StackMeshes.Count()-1].SetActive(true);
        }
        else if(StackMeshes[count - 1] != null){
            //Debug.Log("Updating Stack Model!");
            DisableAllMeshes();
            StackMeshes[count - 1].SetActive(true);
        }
    }
}
