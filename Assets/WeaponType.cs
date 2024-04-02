using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
	[SerializeField]
	Weapon weapon;
	//[SerializeField]
	//AnimationClip Idle, Walk, Run, Reload, Fire, Falling, Jump, Draw, PutAway, Landing;
	[SerializeField]
	public List<AnimationClip> animationClips = new List<AnimationClip>();
	[SerializeField]
	public GameObject worldModel;
	void Start(){
		
	}
}
