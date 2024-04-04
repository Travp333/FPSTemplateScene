using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnim : MonoBehaviour
{
	[SerializeField]
	public float fireCooldown = .2f;
	[SerializeField]
	public float reloadCooldown = 2f;
	[SerializeField]
	public float reloadFireCooldown = 2f;
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
		anim = this.GetComponent<Animator>();
		Debug.Log("PLAY DRAW!");
		anim.Play("Draw");
	}
}
