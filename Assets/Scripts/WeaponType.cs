using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
	[SerializeField]
	public Vector3 initialSpawnRotation;
	[SerializeField]
	public AnimatorOverrideController animOverride;
	[SerializeField]
	Weapon weapon;
	[SerializeField]
	public GameObject playerModel;
	void Start(){
		
	}
}
