using UnityEngine;
using System.Collections.Generic;
// a FILO stack that represents ammo loaded into a magazine. This should ideally store ammo loaded into the magazine in the order it was loaded in,
// then fire the bullets in a first in last out order, meaning the most recently loaded bullet will fire first, then count down. 
public class AmmoList : MonoBehaviour
{
    [SerializeField]
    public Stack<GameObject> Ammo = new Stack<GameObject>();
    [SerializeField]
    [Tooltip("This list will be iterated through and loaded into the magazine")]
    public List<GameObject> startingAmmo = new List<GameObject>();
    void Start()
    {
        for(var i = startingAmmo.Count - 1; i >= 0; i--){
            //Debug.Log("Pushing " + startingAmmo[i] + " into stack");
            Ammo.Push(startingAmmo[i]);
        }
    }
}
