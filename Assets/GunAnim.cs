using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnim : MonoBehaviour
{
	Animator anim;
	protected void Start()
	{
		anim = this.GetComponent<Animator>();
	}
	public void PlayFire(){
		anim.Play("Fire");
	}
	public void PlayReload(){
		anim.Play("Reload");
	}
	public void PlayDraw(){
		anim.Play("Draw");
	}
}
