using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script is meant to handle a button or lever or whatever that is interacted with by the player. it will play an animation to reflect that it has been interacted with, then run whatever script is linked in the InteractableObject Component
//Travis Parks
public class buttonPush : MonoBehaviour
{
    [SerializeField]
    public bool oneTime;
    [HideInInspector]
    public Animator anim;
    InteractableObject intObj;
    bool flipflop = true;
    [SerializeField]
    bool flipflopButton;
	//[SerializeField]
	//public DoorOpenandClose door;
    [HideInInspector]
    public bool blocker = false;
    GameObject player;
    HandAnim hand;
    // Start is called before the first frame update
    void Start()
	{
		player = GameObject.Find("Player");
		hand = GameObject.Find("Hands").GetComponent<HandAnim>();
        intObj = GetComponent<InteractableObject>();
        anim = GetComponent<Animator>();
    }

    void resetblocker(){
        blocker = false;
    }

    void resetPushed(){
        anim.SetBool("Pushed", false);
    }

    void fullPress(){
        intObj.Press();
    }

    void fullPressOrRelease(){
        if(!flipflop){
            intObj.Press();
        }
        else{
            intObj.Release();
        }

    }


    public void press(){
        if(oneTime){
            if(!anim.GetBool("onePush")){
                anim.SetBool("onePush", true);
                intObj.Press();
                hand.interact();
            }
        }
        else{
            if(!blocker){
                if(!flipflopButton){
                    anim.SetBool("Pushed", true);
                    Invoke("resetPushed", .05f);
                    intObj.Press();
                    hand.interact();
                    blocker = true;
                    Invoke("resetblocker", 2f);
                }
                else {
                    if(flipflop){
                        anim.SetBool("Pushed", true);
                        Invoke("resetPushed", .05f);
                        flipflop = false;
                        intObj.Press();
                        hand.interact();
                        blocker = true;
                        Invoke("resetblocker", 2f);
                    }
                    else{
                        anim.SetBool("Pushed", true);
                        Invoke("resetPushed", .05f);
                        flipflop = true;
                        intObj.Release();
                        hand.interact();
                        blocker = true;
                        Invoke("resetblocker", 2f);
                    }
                }
            }
        }
    }

}
