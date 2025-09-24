using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script stores information about Inventory Objects
//Written by Travis
[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    public string Objname;
    [SerializeField]
    public float weight;
    [SerializeField]
    public int stackSize;
    [SerializeField]
    public Sprite img;
    [SerializeField]
    public GameObject prefab;
    [SerializeField]
    [Tooltip("What type of item is this? Options are Ammo, Magazine, Other")]
    public string itemType;
    [SerializeField]
    [Tooltip("What type of ammo is this, or what type of ammo does this magazine hold? Options are 9mm, Shotgun, 8x22, N/A")]
    public string ammoType;
    [SerializeField]
    [Tooltip("how much ammo can this magazine hold? Leave -1 if N/A")]
    public int ammoSize;

}
