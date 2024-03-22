using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script handles the first person camera movement, sensitivity, acelleration, etc.
//Adapted from Combo by Travis Parks
public class SimpleCameraMovement : MonoBehaviour
{
	[SerializeField]
	bool cursorLock = false;
	[SerializeField]
	Transform playerCamera = null;
	Vector2 currentMouseDelta = Vector2.zero;
	Vector2 currentMouseDeltaVelocity = Vector2.zero;
	[SerializeField, Range(0f, .5f)]
	float mouseSmoothTime = 0.03f;
	float pitch = 0f;
	public GameObject player = default;
	[Range(50, 500f)] public float sens = 66;
	void Awake() {
		if (!cursorLock) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	void FixedUpdate()
	{
		UpdateMouseLook();
	}
	void UpdateMouseLook() {
		Vector2 targetMouseDelta = new Vector2(
			Input.GetAxis("Mouse X"),
			Input.GetAxis("Mouse Y")
		);
		currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
		pitch -= currentMouseDelta.y * (sens*Time.deltaTime);
		pitch = Mathf.Clamp(pitch, -90f, 90f);
		playerCamera.localEulerAngles = Vector3.right * pitch;
		transform.Rotate(Vector3.up * currentMouseDelta.x * (sens*Time.deltaTime));
	}
	Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}

