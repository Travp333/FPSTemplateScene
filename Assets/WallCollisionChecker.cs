using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WallCollisionChecker : MonoBehaviour
{
    HandAnim handanim;
    private void Start()
    {
        handanim = GameObject.Find("Hands").GetComponent<HandAnim>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player"){
            handanim.animator.SetBool("WallCollision", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Player"){
            handanim.animator.SetBool("WallCollision", false);
        }
    }
}
