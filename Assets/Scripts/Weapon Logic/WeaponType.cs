using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
	[SerializeField]
	public AnimatorOverrideController[] animOverride;
	[SerializeField]
	public AnimatorOverrideController[] gunAnimOverride;
	[SerializeField]
	Weapon weapon;
	[SerializeField]
	public GameObject playerModel;
	void Start(){
		
	}
}
