using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorOverrider : MonoBehaviour
{
	private Animator anim;
	protected void Awake()
	{
		anim = GetComponent<Animator>();
	}
	public void SetAnimations(AnimatorOverrideController overrideController){
		anim.runtimeAnimatorController = overrideController;
	}
}
