//Author: Travis Parks
//Debugging: Travis Parks
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script controls all the animations tied to the character, such as when certain animations should be played and how they should be played.
public class HandAnim : MonoBehaviour
{
	public List<string> animNames = new List<string>();
	Controls controls;
    MovementSpeedController speedController;
    [SerializeField]
    GameObject sphere;
    Movement movement;
    bool flipflop = true;
    bool blocker = true;
    float charge;
    public Animator animator;
    float playerSpeed;
    float playerSpeed2;
    float tempSpeed;
    float tempSpeed2;
    bool isOnGround;
    bool isOnSteep;
    [HideInInspector]
    public bool isOnGroundADJ;
    [SerializeField]
    [Tooltip("how long you need to be in the air before the 'onGround' bool triggers")]
    float OnGroundBuffer = .5f;
    float Groundstopwatch = 0;
    bool JumpPressed;
    bool holdingDummy;
    Grab grab;
    // Start is called before the first frame update
    public bool getisThrowing(){
        return animator.GetBool("isThrowing");
    }
    public void setisHolding(bool plug){
        animator.SetBool("isHolding", plug);
    }
    public bool getisHolding(){
        return animator.GetBool("isHolding");
    }
    public void setisCrouching(bool plug){
        animator.SetBool("isCrouched", plug);
    }
    public bool getisCrouching(){
        return animator.GetBool("isCrouched");
    }
    public void setisThrowing(bool plug){
        animator.SetBool("grabCharge", !plug);
        animator.SetBool("isThrowing", plug);
    }
	public void ReplaceAnimationClips(int layer, List<AnimationClip> anims){
		foreach(AnimationClip a in anims){
			foreach(string s in animNames){
				if(a.name == s){
					
				}
			}
		}
	}
    void BoolAdjuster(){
        isOnGround = movement.OnGround;
        isOnSteep = movement.OnSteep;
        if (!isOnGround && !JumpPressed){
            Groundstopwatch += Time.deltaTime;
            if (Groundstopwatch >= OnGroundBuffer){
                isOnGroundADJ = false;
            }
        }
        if (!isOnGround && JumpPressed){
            isOnGroundADJ = false;
        }
        if(isOnGround){
            Groundstopwatch = 0;
            isOnGroundADJ = true;
        }
    }
    void resetInteract(){
	    animator.SetBool("Interact", false);
    }
	public void interact(){
		animator.Play("InteractBlend");
	    //animator.SetBool("Interact", true);
        Invoke("resetInteract", .1f);
	}
	void FillAnimNames(){
		animNames.Add("Idle");
		animNames.Add("Walk");
		animNames.Add("Run");
		animNames.Add("Jump");
		animNames.Add("Falling");
		animNames.Add("Landing");
		animNames.Add("Fire");
		animNames.Add("Reload");
		animNames.Add("PutAway");
		animNames.Add("Draw");
	}
    void Start()
	{
		FillAnimNames();
        controls = GameObject.Find("Data").GetComponentInChildren<Controls>();
        speedController = sphere.GetComponent<MovementSpeedController>();
        //animator = GetComponent<Animator>();
        movement = sphere.GetComponent<Movement>();
        grab = GetComponent<Grab>();
    }
    void openGate(){
        blocker = true;
    }
    void closeGate(){
        blocker = true;
    }
    void resetHoldingChange(){
        animator.SetBool("holdingChange", false);
    }
    public void forceIdle(){
        animator.SetBool("isMoving", false);
        animator.SetBool("isSprinting", false);
        animator.SetBool("walkPressed", false);
    }
	void waveStartL(){
		blocker = false;
		flipflop = !flipflop;
		animator.SetBool("isPunchingLeft", true);
		Invoke("waveStop", .1f);
	}
	void waveStartR(){
		blocker = false;
		flipflop = !flipflop;
		animator.SetBool("isPunchingRight", true);
		Invoke("waveStop", .1f);

	}
	void waveStop(){
        
		animator.SetBool("isPunchingLeft", false);
		animator.SetBool("isPunchingRight", false);
		blocker = true;
       
	}

    //Update() pausing by Brian Meginness
    // Update is called once per frame
    void Update()
    {
        
        //IF not paused
        if (!FindObjectOfType<PauseMenu>().isPaused) {
            if(isOnSteep){
                animator.SetBool("OnSteep", true);
            }
            else if(!isOnSteep){
                animator.SetBool("OnSteep", false);
            }
            if (Input.GetKeyDown(controls.keys["duck"])){
                setisCrouching(true);
            }
            if (Input.GetKeyUp(controls.keys["duck"])){
                setisCrouching(false);
            }
            if (grab.isgrabCharging) {
                animator.SetBool("grabCharge", true);
            }
            else if (!grab.isgrabCharging) {
                animator.SetBool("grabCharge", false);
            }
            BoolAdjuster();
            bool JumpPressed = Input.GetKey(controls.keys["jump"]);
            isOnGround = isOnGroundADJ;
            //this OnGround stays true for a little bit after you leave the ground, hence ADJ
            if (isOnGround) {
                animator.SetBool("isOnGroundADJ", true);
            }
            else if (!isOnGround) {
                animator.SetBool("isOnGroundADJ", false);
            }

            if (Input.GetKey(controls.keys["walkUp"]) || Input.GetKey(controls.keys["walkLeft"]) || Input.GetKey(controls.keys["walkDown"]) || Input.GetKey(controls.keys["walkRight"])) {
                animator.SetBool("isMoving", true);
            }
            else {
                animator.SetBool("isMoving", false);
            }
            if (Input.GetKey(controls.keys["sprint"])) {
                animator.SetBool("isSprinting", true);
            }
            else {
                animator.SetBool("isSprinting", false);
            }
            if (Input.GetKey(controls.keys["duck"])) {
                animator.SetBool("walkPressed", true);
            }
            else {
                animator.SetBool("walkPressed", false);
            }
	        if (Input.GetKeyDown(controls.keys["throw"])) {
		        if (blocker && !grab.isHolding) {
			        if (flipflop) {
				        Invoke("waveStartL", .1f);
			        }
			        else if (!flipflop) {
				        Invoke("waveStartR", .1f);
			        }
		        }
	        }

            playerSpeed = sphere.GetComponent<Rigidbody>().velocity.magnitude;
            if (playerSpeed < .001f) {
                animator.SetFloat("Blend", 0f);
            }
            else {
                if (playerSpeed >= 15f) {
                    animator.SetFloat("Blend", 1f);
                }
                else {
                    playerSpeed2 = playerSpeed / 15f;
                    animator.SetFloat("Blend", playerSpeed2);

                    if (playerSpeed >= 10f) {
                        animator.SetFloat("walkBlend", 1f);
                    }
                    if (playerSpeed < .001f) {
                        animator.SetFloat("walkBlend", 0f);
                    }
                    playerSpeed = playerSpeed / 10f;
                    animator.SetFloat("walkBlend", playerSpeed);
                }
            }
        }
    }
}
