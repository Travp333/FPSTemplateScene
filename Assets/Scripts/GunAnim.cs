using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunAnim : MonoBehaviour
{
	[SerializeField]
	ParticleSystem muzzleFlare;
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
	public float reloadCooldown = 2f;
	[SerializeField]
	public float reloadFireCooldown = 2f;
	Animator anim;
	[SerializeField]
	GameObject bulletObj;
	[SerializeField]
	GameObject dudBulletObj;
	GameObject bulletParent;
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
		//pre-calculates random rotation vectors for shell casings 
		casingTorques.Add(new Vector3(Random.Range(0,20), Random.Range(0,20), Random.Range(0,20)));
		casingTorques.Add(new Vector3(Random.Range(0,20), Random.Range(0,20), Random.Range(0,20)));
		casingTorques.Add(new Vector3(Random.Range(0,20), Random.Range(0,20), Random.Range(0,20)));
		casingTorques.Add(new Vector3(Random.Range(0,20), Random.Range(0,20), Random.Range(0,20)));

		bulletParent = GameObject.Find("BulletParent");
		cam = GameObject.Find("HandCam").GetComponent<Camera>();
		anim = this.GetComponent<Animator>();
	}

	public void PlayFire(){
		anim.Play("Fire", 0, 0f);
		muzzleFlare.Play();
		GameObject newCasing = Instantiate(casing) as GameObject;
		newCasing.transform.position = casingSpawnPoint.transform.position;
		Rigidbody casingRigidBody = newCasing.GetComponent<Rigidbody>();
		casingRigidBody.velocity = (this.transform.up + this.transform.right) * casingVelocity;
		casingRigidBody.AddTorque(casingTorques[Random.Range(0,casingTorques.Count - 1)], ForceMode.Impulse);
		//DO A RAYCAST FIRST, THEN CHECK IF HIT. IF HIT, REGISTER HIT, DUH
		//IF NO HIT, THEN DO PROJECTILE STYLED BULLET
		//GARBAJ STYLED HYBRID APPROACH
		RaycastHit hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, raycastDistance, mask))
		{
			//Debug.DrawLine(cam.transform.position, hit.point, Color.cyan, 1f);
			Debug.Log("Using Raycast!");
			GameObject newBullet = Instantiate(dudBulletObj) as GameObject;
			newBullet.transform.position = hit.point;
		}
		else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, projectileCheckDistance, mask)){
			Debug.Log("Using Projectile targeted at raycast position!");
			//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.yellow, 1f);
			GameObject newBullet = Instantiate(bulletObj) as GameObject;
			//Parent it to get a clean workspace
			//newBullet.transform.parent = bulletParent.transform;
			//Give it speed and position
			Vector3 startPos = bulletSpawnPos.transform.position;
			Vector3 startDir = (hit.point - bulletSpawnPos.transform.position).normalized;
			newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir);
		}
		else{
			Debug.Log("Using Projectile!");
			//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.red, 1f);
			GameObject newBullet = Instantiate(bulletObj) as GameObject;
			//Parent it to get a clean workspace
			//newBullet.transform.parent = bulletParent.transform;
			//Give it speed and position
			Vector3 startPos = bulletSpawnPos.transform.position;
			Vector3 startDir = cam.transform.forward;
			newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir);
		}
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
