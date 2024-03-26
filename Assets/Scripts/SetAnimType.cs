using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimType : MonoBehaviour
{
	[SerializeField] private AnimatorOverrideController[] overrideControllers;
	[SerializeField] private animatorOverrider overrider;
	
	public void Set(int value){
		overrider.SetAnimations(overrideControllers[value]);
	}
}
