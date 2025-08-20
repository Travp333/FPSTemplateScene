using UnityEngine;

public class HandFireAnim : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //  {

    //  }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
   // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   // {

   // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<HandAnim>().gunAnim.gameObject.GetComponent<AmmoManager>().ammoInMag <= 0)
        {
            //Okay issue here is that the previous fire is exiting right when the next fire starts. the next fire sets the ammo to zero, then the first animation is still exiting
            // and triggers this falsely, which sets the animaiton state to empty preemptively. I need to fix this somehow idk
            // PLAN move this functionality somewhere else, try to make an invoke on shoot that will then set the state accordingly after the fire cooldown, should be easy.

            //Debug.Log("Ran out of ammo, setting anim state");

            //set hand anim overrider to outofammo if you finish shooting and the ammo is zero
            //animator.gameObject.GetComponent<HandAnim>().SetOutOfAmmoState();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
