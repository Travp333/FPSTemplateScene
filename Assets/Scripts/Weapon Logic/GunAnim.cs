using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GunAnim : MonoBehaviour
{
	[HideInInspector]
	public AnimatorOverrideController[] gunInHandAnimOverride;
	[SerializeField]
	[Tooltip("Where does the hand point to?")]
	public GameObject iKTarget;
	[SerializeField]
	[Tooltip("Tweak specifics of the wall collision")]
	public float wallCollisionCheckSizeAdjust = 1.2f;
	[SerializeField]
	[Tooltip("Tweak specifics of the wall collision")]
	public float wallCollisionCheckPosAdjust = 1.2f;
	[SerializeField]
	ParticleSystem[] muzzleFlare;

	[SerializeField]
	public GameObject WorldModel;
	public Animator anim;
	[SerializeField]
	public bool offHandIK;

	protected void Start()
	{
		anim = this.GetComponent<Animator>();
	}
	// allows you to switch animation overriders on the fly, called via animation
	// this would be used to do incremental animation changes, like individual hammers on a shotgun moving per shot. 
	public void SwitchAnimOverrider(int index)
	{
		if (gunInHandAnimOverride.Length > 0)
		{
			anim.runtimeAnimatorController = gunInHandAnimOverride[index];
		}
	}
	public void PlayFire(){
		//plays the given fire animation
		anim.Play("Fire", 0, 0f);
		//spawns a muzzle flare if one is given
		if(muzzleFlare.Count() > 0){
			int randomMuzzleIndex = Random.Range(0,muzzleFlare.Count());
			if(muzzleFlare[randomMuzzleIndex] != null){
				muzzleFlare[randomMuzzleIndex].Play();
			}
		}
	}
	public void PlayReload(){
		//plays given reload animation
		anim.Play("Reload");
	}
	public void PlayOutOfAmmoReload(){
		//plays given out of ammo reload animation
		anim.Play("OutOfAmmoReload");
	}
	public void PlayDraw(){
		//plays given draw animation (calls anim faster to ensure it is ready for the picked up weapon)
		if(this.GetComponent<Animator>() != null){
			anim = this.GetComponent<Animator>();
		}
		anim.Play("Draw");
	}
}
