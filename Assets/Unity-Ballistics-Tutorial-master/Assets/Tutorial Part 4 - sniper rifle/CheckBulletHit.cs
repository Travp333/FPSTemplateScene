using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will check if the bullet hit anything since last update
public class CheckBulletHit : MonoBehaviour
{
    private Vector3 lastPos;

    

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

		//Move the bullet to where we hit
		transform.position = hit.point;

		//Deactivate the script that moves the bullet, so we can see where it hit
		GetComponent<MoveBullet>().enabled = false;
		Invoke("DestroyBullet", 5f);
	}

    //Did we hit a target
    void CheckHit()
    {
        Vector3 currentPos = transform.position;
    
        Vector3 fireDirection = (currentPos - lastPos).normalized;

        float fireDistance = (currentPos - lastPos).magnitude;

        RaycastHit hit;

        if (Physics.Raycast(currentPos, fireDirection, out hit, fireDistance))
        {
	        if (!hit.collider.CompareTag("BulletIgnore"))
            {
		        BulletHit(hit);
            }
        }
    }
}
