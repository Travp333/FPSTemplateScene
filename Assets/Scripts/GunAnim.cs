using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GunAnim : MonoBehaviour
{
	[SerializeField]
	public float wallCollisionCheckSizeAdjust = 1.2f;
	[SerializeField]
	public float wallCollisionCheckPosAdjust = 1.2f;
	AmmoManager ammomanager;
	[SerializeField]
	ParticleSystem[] muzzleFlare;
	[SerializeField]
	GameObject casing;
	[SerializeField]
	float casingVelocity = 50f;
	public static List<Vector3> casingTorques = new List<Vector3>();
	[SerializeField]
	GameObject casingSpawnPoint;
	[SerializeField]
	public bool fullAuto;
	Camera cam;
	[SerializeField]
	public GameObject WorldModel;
	[SerializeField]
	public float fireCooldown = .2f;
	[SerializeField]
	[Tooltip("How long until you can reload again on a reload with a round still in the chamber")]
	public float reloadCooldown = 1f;
	[SerializeField]
	[Tooltip("How long until you can fire again on a reload with a round still in the chamber")]
	public float reloadFireCooldown = 1f;
	[SerializeField]
	[Tooltip("How long until you can reload again on a reload with a round not in the chamber")]
	public float noAmmoReloadCooldown = 2f;
	[SerializeField]
	[Tooltip("How long until you can fire again on a reload with a round not in the chamber")]
	public float noAmmoReloadFireCooldown = 2f;
	public Animator anim;
	[SerializeField]
	GameObject bulletObj;
	[SerializeField]
	GameObject dudBulletObj;
	public GameObject bulletParent;
	[SerializeField]
	Transform bulletSpawnPos;
	[SerializeField]
	float raycastDistance = 50f;
	[SerializeField]
	float projectileCheckDistance = 100f;
	[SerializeField]
	LayerMask mask;
	protected void Start()
	{
		ammomanager = GetComponent<AmmoManager>();
		//pre-calculates random rotation vectors for shell casings 
		if(casing != null){
			casingTorques.Add(new Vector3(2, 13, 5));
			casingTorques.Add(new Vector3(19, 0, 4));
			casingTorques.Add(new Vector3(4, 0, 6));
			casingTorques.Add(new Vector3(20, 19, 0));
		}
		bulletParent = GameObject.Find("BulletParent");
		cam = GameObject.Find("HandCam").GetComponent<Camera>();
		anim = this.GetComponent<Animator>();
	}

	public void FinishReload(){
		ammomanager.Reload();
	}
	public void StopAllMuzzleFlares(){
		foreach (ParticleSystem p in muzzleFlare){
			p.Stop();
		}
	}
	public void PlayFire(){
		if(muzzleFlare.Count() > 0){
			int randomMuzzleIndex = Random.Range(0,muzzleFlare.Count());
			if(muzzleFlare[randomMuzzleIndex] != null){
				muzzleFlare[randomMuzzleIndex].Play();
			}
		}
		anim.Play("Fire", 0, 0f);
		
		if(casing != null){
			GameObject newCasing = Instantiate(casing) as GameObject;
			newCasing.transform.position = casingSpawnPoint.transform.position;
			Rigidbody casingRigidBody = newCasing.GetComponent<Rigidbody>();
			casingRigidBody.velocity = (this.transform.up + this.transform.right) * casingVelocity;
			casingRigidBody.AddTorque(casingTorques[Random.Range(0,casingTorques.Count - 1)], ForceMode.Impulse);
			newCasing.transform.parent = bulletParent.transform;		
		}



		//DO A RAYCAST FIRST, THEN CHECK IF HIT. IF HIT, REGISTER HIT, DUH
		//IF NO HIT, THEN DO PROJECTILE STYLED BULLET
		//GARBAJ STYLED HYBRID APPROACH
		RaycastHit hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, raycastDistance, mask))
		{
			//Debug.DrawLine(cam.transform.position, hit.point, Color.cyan, 1f);
			//Debug.Log("Using Raycast!");
			GameObject newBullet = Instantiate(dudBulletObj) as GameObject;
			newBullet.transform.position = hit.point;
			newBullet.transform.parent = bulletParent.transform;
		}
		else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, projectileCheckDistance, mask)){
			//Debug.Log("Using Projectile targeted at raycast position!");
			//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.yellow, 1f);
			GameObject newBullet = Instantiate(bulletObj) as GameObject;
			//Parent it to get a clean workspace
			newBullet.transform.parent = bulletParent.transform;
			//Give it speed and position
			Vector3 startPos = bulletSpawnPos.transform.position;
			Vector3 startDir = (hit.point - bulletSpawnPos.transform.position).normalized;
			newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir);
		}
		else{
			//Debug.Log("Using Projectile!");
			//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.red, 1f);
			GameObject newBullet = Instantiate(bulletObj) as GameObject;
			//Parent it to get a clean workspace
			newBullet.transform.parent = bulletParent.transform;
			//Give it speed and position
			Vector3 startPos = bulletSpawnPos.transform.position;
			Vector3 startDir = cam.transform.forward;
			newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir);
		}
	}
	public void PlayWallCollision(){

	}
	public void PlayReload(){
		anim.Play("Reload");
	}
	public void PlayOutOfAmmoReload(){
		anim.Play("OutOfAmmoReload");
	}
	public void PlayDraw(){
		anim = this.GetComponent<Animator>();
		//Debug.Log("PLAY DRAW!");
		anim.Play("Draw");
	}
}
