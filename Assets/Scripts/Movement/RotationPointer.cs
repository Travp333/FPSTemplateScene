using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPointer : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 10;
    [SerializeField]
    GameObject player = default;
    Movement sphere; 
    [SerializeField]
    Transform playerinputSpace = default;
    // Start is called before the first frame update
    void Start()
    {
        sphere = player.GetComponent<Movement>();
    }

    // Update is called once per frame
	void Update () {
		Vector3 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
        transform.localPosition = sphere.ProjectDirectionOnPlane(playerinputSpace.TransformDirection(playerInput.x, 0f, playerInput.y) * maxSpeed, CustomGravity.GetUpAxis(transform.position) );
        
		//transform.localPosition = new Vector3(playerInput.x, 0.5f, playerInput.y);
	}
}
