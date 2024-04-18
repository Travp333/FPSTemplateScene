using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will check if the bullet hit anything since last update
public class CheckBulletHit : MonoBehaviour
{
    private Vector3 lastPos;
	[SerializeField]
	GameObject bulletObjectDud;
    void FixedUpdate()
    {
        CheckHit();

        lastPos = transform.position;
    }

	void DestroyBullet(){
		GetComponent<KillBullet>().Destroy();
	}
	public void BulletHit(RaycastHit hit){
		Debug.Log("Hit target!");
		Debug.DrawLine(GameObject.Find("HandCam").transform.position, hit.point, Color.cyan, 5f);
		//Move the bullet to where we hit
		transform.position = hit.point;
		Instantiate(bulletObjectDud, hit.point, Quaternion.identity);
		Destroy(this.gameObject);
	}

    //Did we hit a target
    void CheckHit()
	{
		//LASTPOS IS (0,0,0) when shit fucks up 
		//FIX LIKELY HERE!!!
        Vector3 currentPos = transform.position;
    
        Vector3 fireDirection = (currentPos - lastPos).normalized;

        float fireDistance = (currentPos - lastPos).magnitude;
	
        RaycastHit hit;
	    Debug.Log(currentPos + ", " + lastPos);
        if (Physics.Raycast(currentPos, fireDirection, out hit, fireDistance))
        {
        	Debug.DrawRay(currentPos, fireDirection * fireDistance, Color.red, 1f);
	        if (!hit.collider.CompareTag("BulletIgnore"))
            {
		        BulletHit(hit);
            }
        }
    }
}
