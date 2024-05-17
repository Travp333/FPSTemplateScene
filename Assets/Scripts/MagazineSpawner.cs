using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineSpawner : MonoBehaviour
{
	[SerializeField]
	GameObject magMesh;
	[SerializeField]
	public Transform handBone;
	[SerializeField]
	Transform SpentMagSpawnPos;
	[SerializeField]
	GameObject spentMagazine;
	[SerializeField]
	GameObject magazine;
	GameObject handMag;
	void Start(){
		handBone = GameObject.Find("Player").GetComponent<Movement>().grab.gameObject.GetComponent<Interact>().MagGrabPoint;
	}
	// Start is called before the first frame update
	public void RevealHandMag(){
		handMag = Instantiate(magMesh, handBone.position, handBone.rotation);
		handMag.transform.parent = handBone;
	}
	public void HideHandMag(){
		if(handMag != null){
			Destroy(handMag);
		}
	}
	public void DropMagazine(){
		Instantiate(spentMagazine, SpentMagSpawnPos.position, SpentMagSpawnPos.rotation);
	}
	public void HideMagazine(){
		magazine.SetActive(false);
	}
	public void UnHideMagazine(){
		magazine.SetActive(true);
	}
}
