using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script stores information about Inventory Objects
//Written by Travis
[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : ScriptableObject
{
	[SerializeField]
	public Weapon me;
    [SerializeField]
    public string Objname;
	

}
