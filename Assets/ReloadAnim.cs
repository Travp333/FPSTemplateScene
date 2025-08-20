using UnityEngine;

public class ReloadAnim : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.GetComponent<HandAnim>().gunLogic.gameObject.GetComponent<AmmoManager>().ammoInMag <= 0)
        {
            animator.gameObject.GetComponent<HandAnim>().CallFinishFullReload();
        }
        else
        {
            animator.gameObject.GetComponent<HandAnim>().CallFinishReload();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //MOVE IT INTO START, THEN FOLLOW INVOKE METHOD YOUVE BEEN DOIGN THIS WHOEL TIME
        //reset to full ammo overrider
      //  animator.gameObject.GetComponent<HandAnim>().SwitchHandAnimOverrider(1);
      //  animator.gameObject.GetComponent<HandAnim>().gunAnim.SwitchGunAnimOverrider(1);
       // animator.gameObject.GetComponent<HandAnim>().gunLogic.FinishReload();
        //finishreload
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
