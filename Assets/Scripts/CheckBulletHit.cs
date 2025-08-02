using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Adpoted from https://www.habrador.com/tutorials/unity-realistic-bullets/

//Will check if the bullet hit anything since last update
public class CheckBulletHit : MonoBehaviour
{
    private Vector3 lastPos;
	[SerializeField]
	GameObject bulletObjectDud;
	public LayerMask mask;
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
		//Debug.DrawLine(GameObject.Find("HandCam").transform.position, hit.point, Color.magenta, 1f);
		//Move the bullet to where we hit
		transform.position = hit.point;

		//is it a rigidbody?
		if (hit.transform.gameObject.layer == 13)
		{
			//if so, apply force (F=MV), then parent the impact effect to the rigidbody
			if (hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic != true)
			{
				Debug.Log("Hit a rigidbody");
				hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition((GetComponent<MoveBullet>().currentVel * GetComponent<BulletData>().mass), hit.point);
				GameObject g = Instantiate(bulletObjectDud, hit.point, Quaternion.identity);
				g.transform.parent = hit.transform;
			}
			else if (hit.transform.parent.gameObject.GetComponent<Rigidbody>() != null && hit.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic != true)
			{
				Debug.Log("Hit a child rigidbody");
				hit.transform.parent.gameObject.GetComponent<Rigidbody>().AddForceAtPosition((GetComponent<MoveBullet>().currentVel * GetComponent<BulletData>().mass), hit.point);
				GameObject g = Instantiate(bulletObjectDud, hit.point, Quaternion.identity);
				g.transform.parent = hit.transform;
			}
			else
			{
				Debug.Log("edge case");
			}

		}
		else
		{
			Debug.Log("Hit a non-rigidbody");
			Instantiate(bulletObjectDud, hit.point, Quaternion.identity);
		}
		Destroy(this.gameObject);
	}
    //Did we hit a target
    void CheckHit()
	{
        Vector3 currentPos = transform.position;
    
        Vector3 fireDirection = (currentPos - lastPos).normalized;


        float fireDistance = (currentPos - lastPos).magnitude;
		//Debug.DrawLine(lastPos, currentPos, Color.red, 1f);

        RaycastHit hit;
		//Debug.Log(currentPos + ", " + lastPos);
        if (Physics.Raycast(currentPos, fireDirection, out hit, fireDistance, mask))
        {
	        if (!hit.collider.CompareTag("BulletIgnore"))
	        {
	        	//Debug.DrawRay(currentPos, fireDirection*fireDistance, Color.green, 1f);
		        BulletHit(hit);
            }
        }
    }
}
