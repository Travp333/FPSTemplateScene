using UnityEngine;
using System.Collections.Generic;
public class Magazine : MonoBehaviour
{
    [SerializeField]
    public int animOverriderState = 1;
    //Tracks How much ammo is in the gun. is updated when dropped 
    [SerializeField]
    public Stack<GameObject> Ammo = new Stack<GameObject>();
}
