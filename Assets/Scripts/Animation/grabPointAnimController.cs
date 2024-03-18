using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script controls where your "grab point " is, that being where a held object it pinned to. It moves depending on the size of the object, and when you charge up a throw
//Travis Parks
public class grabPointAnimController : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    Grab grab;
    [SerializeField]
    HandAnim hand;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Large", false);
        anim.SetBool("Medium", false);
        anim.SetBool("Small", false);
        anim.SetBool("Tiny", false);
    }

    void setisDropped(bool plug){
        anim.SetBool("isDropped", plug);
    }

    // Update is called once per frame
    void Update()
    {
        if(hand.getisHolding() == false){
            anim.SetBool("isDropped", true);
        }
        else if (hand.getisHolding()){
            anim.SetBool("isDropped", false);
        }

        if (hand.getisThrowing()){
            anim.SetBool("isThrowing", true);
        }
        else {
            anim.SetBool("isThrowing", false);
        }
        if (grab.isgrabCharging){
            anim.SetBool("isCharging", true);
        }
        else{
            anim.SetBool("isCharging", false);
        }
        if (grab.sizes == Grab.objectSizes.none){
            anim.SetBool("Large", false);
            anim.SetBool("Medium", false);
            anim.SetBool("Small", false);
            anim.SetBool("Tiny", false);
        }
        if (grab.sizes == Grab.objectSizes.large){
            anim.SetBool("Large", true);
            anim.SetBool("Medium", false);
            anim.SetBool("Small", false);
            anim.SetBool("Tiny", false);
        }
        if (grab.sizes == Grab.objectSizes.medium){
            anim.SetBool("Large", false);
            anim.SetBool("Medium", true);
            anim.SetBool("Small", false);
            anim.SetBool("Tiny", false);
        }
        if (grab.sizes == Grab.objectSizes.small){
            anim.SetBool("Large", false);
            anim.SetBool("Medium", false);
            anim.SetBool("Small", true);
            anim.SetBool("Tiny", false);
        }
        else if (grab.sizes == Grab.objectSizes.tiny){
            anim.SetBool("Large", false);
            anim.SetBool("Medium", false);
            anim.SetBool("Small", false);
            anim.SetBool("Tiny", true);
        }


    }
}
