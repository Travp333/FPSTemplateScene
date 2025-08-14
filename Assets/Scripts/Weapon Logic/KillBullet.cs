using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adpoted from https://www.habrador.com/tutorials/unity-realistic-bullets/

//We fire bullets continuously, so we need to remove them 
public class KillBullet : MonoBehaviour
{    
    void Update()
    {
        //Remove the bullet if it is below ground level
        if (transform.position.y < -10f)
        {
	        Destroy(gameObject);
        }
    }
	public void Destroy(){
		Destroy(gameObject);
	}
	public void DestroyAfterXTime(){
		Invoke("Destroy", 5f);
	}
}
