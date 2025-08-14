using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WallCollisionChecker : MonoBehaviour
{
    HandAnim handanim;
    Interact inter;
    private void Start()
    {
        handanim = GameObject.Find("Hands").GetComponent<HandAnim>();
        inter = handanim.gameObject.GetComponent<Interact>();
    }
    /// <summary>
    /// OnTriggerStay is called once per frame for every Collider other
    /// that is touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other)
    {
        if(!inter.isWallColliding){
            if(other.gameObject.tag != "Player" && other.gameObject.tag != "Magazine" && other.gameObject.tag != "Weapon" && other.gameObject.tag != "Item"){
                if(!handanim.reloading){
                    handanim.animator.Play("IdleToWallCollideEntry", 0, 0f);
                    handanim.animator.SetBool("WallCollision", true);
                    inter.isWallColliding = true;
                    handanim.forceIdle();
                    handanim.canShoot = false;
                    handanim.canReload = false;
                }
                else{
                    handanim.animator.SetBool("WallCollision", false);
                }
            }
        }
    }
    void StopOverlapping(){
        inter.isWallColliding = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player" && other.gameObject.tag != "Magazine" && other.gameObject.tag != "Weapon" && other.gameObject.tag != "Item"){
            if(!handanim.reloading){
                handanim.animator.Play("IdleToWallCollideEntry", 0, 0f);
                handanim.animator.SetBool("WallCollision", true);
                inter.isWallColliding = true;
                handanim.forceIdle();
                handanim.canShoot = false;
                handanim.canReload = false;
            }
            else{
                handanim.animator.SetBool("WallCollision", false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Player" && other.gameObject.tag != "Magazine" && other.gameObject.tag != "Weapon" && other.gameObject.tag != "Item"){
            handanim.animator.SetBool("WallCollision", false);
            Invoke("StopOverlapping", .2f);
            //if(handanim.holdingWeapon){
            //    handanim.ResetFireable();
            //}
            
        }
    }
}
