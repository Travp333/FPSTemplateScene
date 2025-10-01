using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class GunLogic : MonoBehaviour
{
    //is this necessary?
	float gravity = -9.81f;
    [SerializeField]
	[Tooltip("How long until you can fire another bullet")]
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
    [SerializeField]
	public bool fullAuto;
	[SerializeField]
	public bool burst;
	[SerializeField]
	[Tooltip("How long until you can burst again")]
	public float burstCooldown;
	[SerializeField]
	[Tooltip("How long is inbetween each bullet")]
	public float inBetweenBurstCooldown;
	[SerializeField]
	[Tooltip("How many extra rounds to shoot? ie a three burst would be 2, as the first round is not included")]
	public int burstCount;
	[SerializeField]
	public bool bursting;
	GameObject bulletObj;
    [SerializeField]
    [Tooltip("Bullet impact effect, spawns when a bulelt hits something")]
	GameObject dudBulletObj;
    [SerializeField]
	Transform bulletSpawnPos;
    public GameObject bulletParent;
    Camera cam;
    AmmoManager ammomanager;
    Vector3 recoilVectorX;
	Vector3 recoilVectorY;
	Vector3 recoilVector;
    RecoilManager recoil;
    [SerializeField]
	[Tooltip("Physics object to spawn at casingSpawnPoint for casing ejection")]
	GameObject casing;
    [SerializeField]
	[Tooltip("Where does the casing spawn?")]
	GameObject casingSpawnPoint;
    [SerializeField]
	[Tooltip("how fast does the casing shoot out?")]
	float casingVelocity = 50f;
    public static List<Vector3> casingTorques = new List<Vector3>();
    BulletData bulletData;
    [SerializeField]
	LayerMask mask;

    [SerializeField]
	[Tooltip("How far ahead this gun will check to get a valid target to launch a projectile at")]
	float projectileCheckDistance = 100f;

    [SerializeField]
	[Tooltip("How far will this gun's raycast go before the bullet stops being a hitscan")]
	float raycastDistance = 50f;
	Vector3 recoil2;
	Vector3 rotation;
	RaycastHit hit;
	Vector3 previousVelocity;
	Vector3 force;

    void Start()
    {
        //bulletData = bulletObj.GetComponent<BulletData>();
        bulletParent = GameObject.Find("BulletParent");
        cam = GameObject.Find("HandCam").GetComponent<Camera>();
        ammomanager = GetComponent<AmmoManager>();
        recoil = GetComponent<RecoilManager>();
		//pre-calculates random rotation vectors for shell casings 
		if (casing != null)
		{
			casingTorques.Add(new Vector3(2, 13, 5));
			casingTorques.Add(new Vector3(19, 0, 4));
			casingTorques.Add(new Vector3(4, 0, 6));
			casingTorques.Add(new Vector3(20, 19, 0));
		}
		
    }
    public void Fire(){
        //determines recoil
        recoil.DetermineHeat();
        //spawns a casing if one exists
		if(casing != null){
			GameObject newCasing = Instantiate(casing) as GameObject;
			newCasing.transform.position = casingSpawnPoint.transform.position;
			Rigidbody casingRigidBody = newCasing.GetComponent<Rigidbody>();
			casingRigidBody.linearVelocity = (this.transform.up + this.transform.right) * casingVelocity;
			casingRigidBody.AddTorque(casingTorques[Random.Range(0,casingTorques.Count - 1)], ForceMode.Impulse);
			newCasing.transform.parent = bulletParent.transform;		
		}
    }
	public void SpawnProjectile(){
		//Try to fire a bullet, and store the result. If the magazine is not empty (which it should be if this is firing) 
		// g will be a reference to the next bullet to be fired
		bulletObj = ammomanager.FireBullet();
		bulletData = bulletObj.GetComponent<BulletData>();
		for (int i = 0; i < bulletData.bulletAmount; i++)
		{
			//DO A RAYCAST FIRST, THEN CHECK IF HIT. IF HIT, REGISTER HIT, DUH
			//IF NO HIT, THEN DO PROJECTILE STYLED BULLET
			//GARBAJ STYLED HYBRID APPROACH

			recoilVectorX = bulletSpawnPos.transform.up * recoil.weaponHeatList[recoil.weaponHeat].recoilOffset.x;
			recoilVectorY = bulletSpawnPos.transform.right * recoil.weaponHeatList[recoil.weaponHeat].recoilOffset.y;
			recoilVector = recoilVectorX + recoilVectorY;
			recoil2 = this.transform.up * (bulletData.recoilAmount * Random.Range(-1f, 1f));
			rotation = Quaternion.AngleAxis(Random.Range(0, 360), cam.transform.forward) * recoil2;
			recoilVector += rotation;


			if (Physics.Raycast(cam.transform.position, cam.transform.forward + recoilVector, out hit, raycastDistance, mask))
			{
				//Debug.DrawLine(cam.transform.position, hit.point, Color.cyan, 1f);
				if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
				{
					// magnitude * direction(aka velocity) / time (aka acceleration) * mass ( aka force ), so f = ma
					force = (((bulletObj.GetComponent<BulletData>().muzzleVelocity / Time.fixedDeltaTime) * (hit.point - cam.transform.position).normalized) * bulletObj.GetComponent<BulletData>().mass);
					hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(force, hit.point);
				}
				//Debug.Log("Using Raycast to spawn dud at hit.point");
				GameObject g = Instantiate(dudBulletObj, hit.point, Quaternion.identity);
				g.transform.parent = hit.transform;
			}
			else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, projectileCheckDistance, mask))
			{
				//Debug.Log("Using Projectile targeted at raycast position!");
				//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.yellow, 1f);
				GameObject newBullet = Instantiate(bulletObj) as GameObject;
				//Parent it to get a clean workspace
				newBullet.transform.parent = bulletParent.transform;
				//Give it speed and position
				Vector3 startPos = bulletSpawnPos.transform.position;
				Vector3 startDir = (hit.point - bulletSpawnPos.transform.position).normalized;
				Debug.DrawRay(startPos, startDir, Color.red, 5f);
				//Pull current x recoil offset value, dependent on current heat
				newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir + recoilVector);
			}
			else
			{
				//Debug.Log("Using Projectile launched straight forward!");
				//Debug.DrawLine(cam.transform.position, hit.point * 5000f, Color.red, 1f);
				GameObject newBullet = Instantiate(bulletObj) as GameObject;
				//Parent it to get a clean workspace
				newBullet.transform.parent = bulletParent.transform;
				//Give it speed and position
				Vector3 startPos = bulletSpawnPos.transform.position;
				Vector3 startDir = cam.transform.forward;
				newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, (startDir + transform.TransformDirection(recoil.weaponHeatList[recoil.weaponHeat].recoilOffset)));
				Debug.DrawRay(startPos, startDir, Color.red, 5f);
				//Debug.Log("recoil offset is "+ recoil.weaponHeatList[recoil.weaponHeat].recoilOffset.ToString("G") + " and heat value is " + recoil.weaponHeat);
				//Pull current x recoil offset value, dependent on current heat
				// this works correctly, but is just bloom recoil. Need to make it take the recoil pattern into account
				recoilVector = this.transform.up * (bulletData.recoilAmount * Random.Range(-1f, 1f));
				rotation = Quaternion.AngleAxis(Random.Range(0, 360), startDir) * recoilVector;
				newBullet.GetComponent<MoveBullet>().SetStartValues(startPos, startDir + rotation);
			}
		}
	}
}
