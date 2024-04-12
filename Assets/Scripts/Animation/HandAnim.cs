//Author: Travis Parks
//Debugging: Travis Parks
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//this script controls all the animations tied to the character, such as when certain animations should be played and how they should be played.
public class HandAnim : MonoBehaviour
{
	public bool reloading;
	public bool firing;
	public InputAction reloadAction;
	public InputAction attackAction;
	Interact inter;
	public GunAnim gunAnim;
	public bool holdingWeapon;
	public bool canShoot = true;
	public bool canReload = true;
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
	public void PickUpWeapon(){
		animator.Play("Draw");
		if(gunAnim != null){
			gunAnim.PlayDraw();	
		}
		holdingWeapon = true;
	}
	public void DropWeapon(){
		animator.Play("Drop");
		holdingWeapon = false;
	}
    public bool getisThrowing(){
        return animator.GetBool("isThrowing");
    }
	public void setisHoldingTrue(){
		if(holdingWeapon){
			
			
		}
        animator.Play("Grab");
        animator.SetBool("isHolding", true);
    }
    public void setisHoldingFalse(){
        animator.SetBool("isHolding", false);
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
    public void setisThrowingTrue(){
        animator.Play("Throw");
        animator.SetBool("grabCharge", false);
        animator.SetBool("isThrowing", true);
    }
    public void setisThrowingFalse(){
        animator.SetBool("grabCharge", true);
        animator.SetBool("isThrowing", false);
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
	void resetIsJumping(){
		animator.SetBool("isJumping", false);
	}
    void resetInteract(){
	    animator.SetBool("Interact", false);
    }
	public void interact(){
		animator.Play("Interact");
	    //animator.SetBool("Interact", true);
        Invoke("resetInteract", .1f);
	}
    void Start()
	{
		attackAction = sphere.GetComponent<PlayerInput>().currentActionMap.FindAction("Attack");
		inter = this.GetComponent<Interact>();
        speedController = sphere.GetComponent<MovementSpeedController>();
        //animator = GetComponent<Animator>();
		movement = sphere.GetComponent<Movement>();
		reloadAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Reload");
        grab = GetComponent<Grab>();
    }
    void openGate(){
        blocker = true;
    }
    void closeGate(){
        blocker = true;
    }
    public void forceIdle(){
        animator.SetBool("isMoving", false);
        animator.SetBool("isSprinting", false);
        animator.SetBool("walkPressed", false);
    }
	void waveStartL(){
		blocker = false;
		flipflop = !flipflop;
        animator.Play("Left Hook");
		//animator.SetBool("isPunchingLeft", true);
		Invoke("waveStop", .1f);
	}
	void waveStartR(){
		blocker = false;
		flipflop = !flipflop;
        animator.Play("Right Hook");
		//animator.SetBool("isPunchingRight", true);
		Invoke("waveStop", .1f);
	}
	void waveStop(){
		animator.SetBool("isPunchingLeft", false);
		animator.SetBool("isPunchingRight", false);
		blocker = true;
	}
	void ResetCanShoot(){
		canShoot = true;
		reloading = false;
		firing = false;
	}
	void ResetCanReload(){
		canReload = true;
		reloading = false;
		firing = false;
	}
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
	        if (movement.crouching){
                setisCrouching(true);
            }
	        if (!movement.crouching){
                setisCrouching(false);
            }
            if (grab.isgrabCharging) {
                animator.SetBool("grabCharge", true);
            }
            else if (!grab.isgrabCharging) {
                animator.SetBool("grabCharge", false);
            }
            BoolAdjuster();
            //this OnGround stays true for a little bit after you leave the ground, hence ADJ
            if (isOnGround) {
                animator.SetBool("isOnGround", true);
            }
            else if (!isOnGround) {
                animator.SetBool("isOnGround", false);
            }
            if (isOnGroundADJ) {
                animator.SetBool("isOnGroundADJ", true);
            }
            else if (!isOnGroundADJ) {
                animator.SetBool("isOnGroundADJ", false);
            }
	        if(movement.jumpAction.WasPressedThisFrame() && (isOnGroundADJ || isOnSteep)&& !grab.isHolding && !reloading && !firing){
                animator.Play("Jump");
	        	animator.SetBool("isJumping", true);
	        	Invoke("resetIsJumping", .1f);
	        }
	        if (movement.movementAction.ReadValue<Vector2>().magnitude > 0) {
                animator.SetBool("isMoving", true);
            }
            else {
                animator.SetBool("isMoving", false);
            }
	        if (speedController.sprintAction.IsPressed()) {
                animator.SetBool("isSprinting", true);
            }
            else {
                animator.SetBool("isSprinting", false);
            }
	        if (movement.crouching) {
	            animator.SetBool("walkPressed", true);
            }
            else {
                animator.SetBool("walkPressed", false);
            }
	        if (attackAction.WasPressedThisFrame()) {
	        	if(holdingWeapon && canShoot){
		        	if(gunAnim != null){
			        	canShoot = false;
			        	canReload = false;
			        	animator.Play("Fire");
		        		gunAnim.PlayFire();
			        	reloading = false;
		        		firing = true;
			        	Invoke("ResetCanShoot", gunAnim.fireCooldown);
		        		Invoke("ResetCanReload", gunAnim.fireCooldown);
		        	}
	        	}
		        else if (!holdingWeapon && blocker && !grab.isHolding) {
			        if (flipflop) {
				        Invoke("waveStartL", .1f);
			        }
			        else if (!flipflop) {
				        Invoke("waveStartR", .1f);
			        }
		        }
	        }
	        if (reloadAction.WasPressedThisFrame())
	        {
	        	//&& ammo < max ammo?
	        	if(holdingWeapon && canReload){
		        	if(gunAnim != null){
			        	canShoot = false;
			        	canReload = false;
			        	animator.Play("Reload");
		        		gunAnim.PlayReload();
		        		reloading = true;
		        		firing = false;
		        		Invoke("ResetCanShoot", gunAnim.reloadFireCooldown);
		        		Invoke("ResetCanReload", gunAnim.reloadCooldown);
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
