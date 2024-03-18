using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
//this script allows you to drag and drop game obejcts in the editor and select a a specific method to be called in one way or another. has functionality to work on volumes as well. 
//Travis Parks
public class InteractableObject : MonoBehaviour {
	
	[SerializeField]
	public bool isVolume;
	[SerializeField]
	bool playerOnly;
	[SerializeField]
	bool isStay;
	bool gate;

	[SerializeField]
	UnityEvent onFirstEnter = default, onLastExit = default;
	List<Collider> colliders = new List<Collider>();

	void Awake () {
		enabled = false;
	}
	void OnDisable () {
	#if UNITY_EDITOR
		if (enabled && gameObject.activeInHierarchy) {
			return;
		}
	#endif
		if (colliders.Count > 0) {
			colliders.Clear();
			onLastExit.Invoke();
		}
	}

	void FixedUpdate () {
		if(isVolume){
			for (int i = 0; i < colliders.Count; i++) {
				Collider collider = colliders[i];
				if (!collider || !collider.gameObject.activeInHierarchy) {
					colliders.RemoveAt(i--);
					if (colliders.Count == 0) {
						onLastExit.Invoke();
						enabled = false;
					}
				}
			}
		}
		if(gate){
			Press();
		}
	}



	public void Press(){
		onFirstEnter.Invoke();
	}
	public void Release(){
		onLastExit.Invoke();
	}

	private void OnTriggerStay(Collider other) {
		if(isStay){
			gate = true;
		}
	}

    void OnTriggerEnter (Collider other) {
		if(playerOnly){
			if(other.gameObject.tag == "Player"){
				if(isVolume){
					if (colliders.Count == 0) {
						onFirstEnter.Invoke();
						enabled = true;
					}
					colliders.Add(other);
				}
			}
		}
		else{
			if(isVolume){
				if (colliders.Count == 0) {
					onFirstEnter.Invoke();
					enabled = true;
				}
				colliders.Add(other);
			}
		}
	}

	void OnTriggerExit (Collider other) {
		if(playerOnly){
			if(other.gameObject.tag == "Player"){
				if(isVolume){
					if (colliders.Remove(other) && colliders.Count == 0) {
						onLastExit.Invoke();
						enabled = false;
					}
				}
			}
		}
		else{
			if(isVolume){
				if (colliders.Remove(other) && colliders.Count == 0) {
					onLastExit.Invoke();
					enabled = false;
				}
			}
		}
		if(isStay){
			gate = false;
		}
	}
}
