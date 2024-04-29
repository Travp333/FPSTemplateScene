using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will check if the bullet hit anything since last update
public class CheckBulletHit : MonoBehaviour
{
    private Vector3 lastPos;
	[SerializeField]
	GameObject bulletObjectDud;
	void Start()
	{
		lastPos = transform.position;
	}
    void FixedUpdate()
	{
        CheckHit();
		lastPos = transform.position;   
    }

	void DestroyBullet(){
		GetComponent<KillBullet>().Destroy();
	}
	public void BulletHit(RaycastHit hit){
		//Debug.Log("Hit target!");
		Debug.DrawLine(GameObject.Find("HandCam").transform.position, hit.point, Color.magenta, 1f);
		//Move the bullet to where we hit
		transform.position = hit.point;
		Instantiate(bulletObjectDud, hit.point, Quaternion.identity);
		Destroy(this.gameObject);
	}
    //Did we hit a target
    void CheckHit()
	{
        Vector3 currentPos = transform.position;
    
        Vector3 fireDirection = (currentPos - lastPos).normalized;


        float fireDistance = (currentPos - lastPos).magnitude;
		Debug.DrawLine(lastPos, currentPos, Color.red, 1f);

        RaycastHit hit;
		//Debug.Log(currentPos + ", " + lastPos);
        if (Physics.Raycast(currentPos, fireDirection, out hit, fireDistance))
        {
	        if (!hit.collider.CompareTag("BulletIgnore"))
	        {
	        	Debug.DrawRay(currentPos, fireDirection*fireDistance, Color.green, 1f);
		        BulletHit(hit);
            }
        }
    }
}
