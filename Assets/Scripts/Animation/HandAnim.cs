//Author: Travis Parks
//Debugging: Travis Parks
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;
using UnityEngine.InputSystem;
//this script controls all the animations tied to the character, such as when certain animations should be played and how they should be played.
public class HandAnim : MonoBehaviour
{
    [SerializeField]
    public FastIKFabric offHandIK;
    public AmmoManager ammomanager;
	public bool reloading;
	public bool firing;
	public InputAction reloadAction;
	public InputAction attackAction;
	Interact inter;
	public GunAnim gunAnim;
    [HideInInspector]
	public AnimatorOverrideController[] handInHandAnimOverride;

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
    PauseMenu pause;
	// Start is called before the first frame update
    public void EnableOffHandIK(){
        if (offHandIK.enabled == false)
        {
            if (gunAnim != null)
            {
                //Debug.Log("gunanim not null, checking these: is holding weapon true? " + holdingWeapon + " is gunanim true? " + gunAnim.offHandIK + " is the offhank IK enabled? " + offHandIK.enabled);
                if (holdingWeapon)
                {
                    if (gunAnim.iKTarget != null)
                    {
                        //Debug.Log("everything hooked up properly");
                        offHandIK.enabled = true;
                        offHandIK.Target = gunAnim.iKTarget.transform;
                    }
                }
            }
        }
    }
    public void DisableOffHandIK(){
        if (offHandIK.enabled == true)
        {
            if (gunAnim != null)
            {
                //Debug.Log("gunanim not null, checking these: is holding weapon true? " + holdingWeapon + " is gunanim true? " + gunAnim.offHandIK + " is the offhank IK enabled? " + offHandIK.enabled);
                if (holdingWeapon)
                {
                    if (gunAnim.iKTarget != null)
                    {
                        offHandIK.enabled = false;
                        offHandIK.Target = null;
                    }
                }
            }
        }
    }
	public void PickUpWeapon(){
        //EnableOffHandIK();
        //Debug.Log("Playing Draw Anim");
		animator.Play("Draw");
		if(gunAnim != null){
			gunAnim.PlayDraw();	
		}
		holdingWeapon = true;
	}
	public void DropWeapon(){
        DisableOffHandIK();
		animator.Play("Drop");
		holdingWeapon = false;
	}
    public bool getisThrowing(){
        return animator.GetBool("isThrowing");
    }
	public void setisHoldingTrue(){
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
        pause = FindFirstObjectByType<PauseMenu>();
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
        //Debug.Log("Can Shoot Again!");
		canShoot = true;
        //Debug.Log("reloading set to false via resetCanShoot()");
		reloading = false;
		firing = false;
	}
	void ResetCanReload(){
        //Debug.Log("Can Reload!");
		canReload = true;
        //Debug.Log("reloading set to false via resetCanReload()");
		reloading = false;
		firing = false;
	}
    void Shoot(){
        if(canShoot && !reloading){
            animator.Play("Fire", 0, 0f);
            //Debug.Log("Firing");
            canShoot = false;
            canReload = false;
            //Debug.Log("reloading set to false via Shoot");
            reloading = false;
            firing = true;
            Invoke("ResetCanShoot", gunAnim.fireCooldown);
            Invoke("ResetCanReload", gunAnim.fireCooldown);
            gunAnim.PlayFire();
        }
    }
    public void ResetFireable(){
        //Debug.Log("Resetting ability to fire");
        Invoke("ResetCanShoot", gunAnim.fireCooldown);
        Invoke("ResetCanReload", gunAnim.fireCooldown);
    }
    void OutOfAmmoShoot()
    {
        if (canShoot && !reloading)
        {
            Debug.Log("Trying to fire, no ammo!");
            animator.Play("OutOfAmmoFire", 0, 0f);
            canShoot = false;
            canReload = false;
            //Debug.Log("reloading set to false via outofammoshoot()");
            reloading = false;
            firing = true;
            Invoke("ResetCanShoot", gunAnim.fireCooldown);
            Invoke("ResetCanReload", gunAnim.fireCooldown);
        }
    }
    public void ForceGunAnimIdle(){
        if(gunAnim != null){
            gunAnim.anim.Play("Idle", 0, 0f);
        }
    }
    public void playJumpAnim(){
        if(!grab.isHolding && !reloading && !firing){
            animator.Play("Jump");
            animator.SetBool("isJumping", true);
            Invoke("resetIsJumping", .1f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //IF not paused
        if (!pause.isPaused) {
            if(isOnSteep){
                animator.SetBool("OnSteep", true);
            }
            else if(!isOnSteep){
                animator.SetBool("OnSteep", false);
            }
	        if (movement.crouching && !movement.moveBlocked){
                setisCrouching(true);
            }
	        if (!movement.crouching && !movement.moveBlocked) {
                setisCrouching(false);
            }
            if (grab.isgrabCharging && !movement.moveBlocked) {
                animator.SetBool("grabCharge", true);
            }
            else if (!grab.isgrabCharging && !movement.moveBlocked) {
                animator.SetBool("grabCharge", false);
            }
            BoolAdjuster();
            //this OnGround stays true for a little bit after you leave the ground, hence ADJ
            if (isOnGround ) {
                animator.SetBool("isOnGround", true);
            }
            else if (!isOnGround ) {
                animator.SetBool("isOnGround", false);
            }
            if (isOnGroundADJ ) {
                animator.SetBool("isOnGroundADJ", true);
            }
            else if (!isOnGroundADJ) {
                animator.SetBool("isOnGroundADJ", false);
            }
	       // if(movement.desiredJump && (isOnGroundADJ || isOnSteep)&& !grab.isHolding && !reloading && !firing){
          //      animator.Play("Jump");
	      //  	animator.SetBool("isJumping", true);
	       // 	Invoke("resetIsJumping", .1f);
	       // }
	        if (movement.movementAction.ReadValue<Vector2>().magnitude > 0 && !movement.moveBlocked) {
                animator.SetBool("isMoving", true);
            }
            else {
                animator.SetBool("isMoving", false);
            }
	        if (speedController.sprintAction.IsPressed() && !movement.moveBlocked) {
                animator.SetBool("isSprinting", true);
            }
            else {
                animator.SetBool("isSprinting", false);
            }
	        if (movement.crouching && !movement.moveBlocked) {
	            animator.SetBool("walkPressed", true);
            }
            else {
                animator.SetBool("walkPressed", false);
            }
	        if(gunAnim != null && !inter.isWallColliding && !movement.moveBlocked){
                //Full Auto
		        if(gunAnim.fullAuto && attackAction.IsPressed()){
                    if (!reloading && holdingWeapon && canShoot && ammomanager.FireBullet())
                    {
                        Shoot();
                        //checking to see if the mag is now empty after firing
                        if (ammomanager.ammoInMag == 0)
                        {
                            gunAnim.anim.SetBool("OutofAmmo", true);
                            canReload = true;
                        }
		        	}
                    else if (ammomanager.ammoInMag == 0)
                    {

                        if (canShoot)
                        {
                            gunAnim.anim.SetBool("OutofAmmo", true);
                            OutOfAmmoShoot();
                            canReload = true;
                        }

                    }
		        }
                else if(!reloading && gunAnim.fullAuto && attackAction.WasReleasedThisFrame() && holdingWeapon && !movement.moveBlocked && canShoot){
                    ResetFireable();
                }
                //Semi Auto
		        if (attackAction.WasPressedThisFrame() && !gunAnim.fullAuto && !movement.moveBlocked) {
                    if (!reloading && holdingWeapon && canShoot && ammomanager.FireBullet())
                    {
                        Shoot();
                        //checking to see if the mag is now empty after firing
                        if (ammomanager.ammoInMag == 0)
                        {
                            gunAnim.anim.SetBool("OutofAmmo", true);
                            canReload = true;
                        }
		        	}
                    else if (ammomanager.ammoInMag == 0)
                    {

                        if (canShoot)
                        {
                            gunAnim.anim.SetBool("OutofAmmo", true);
                            OutOfAmmoShoot();
                            canReload = true;
                        }
                    }
		        }
	        }
	        if (attackAction.WasPressedThisFrame() && !holdingWeapon && blocker && !grab.isHolding && !movement.moveBlocked) {
		        if (flipflop) {
			        Invoke("waveStartL", .1f);
		        }
		        else if (!flipflop) {
			        Invoke("waveStartR", .1f);
		        }
	        }
	        
	        if (reloadAction.WasPressedThisFrame() && !inter.isWallColliding && !movement.moveBlocked)
	        {
	        	if(holdingWeapon && canReload){
		        	if(gunAnim != null){
                        if (ammomanager.CanReload()){
                            Debug.Log("Starting Reload");
                            if(ammomanager.ammoInMag == 0){
                                CancelInvoke();
                                Debug.Log("Doing No Ammo Reload!");
                                canShoot = false;
                                canReload = false;
                                reloading = true;
                                firing = false;
                                animator.Play("OutOfAmmoReload");
                                gunAnim.PlayOutOfAmmoReload();
                                Invoke("ResetCanShoot", gunAnim.noAmmoReloadFireCooldown);
                                Invoke("ResetCanReload", gunAnim.noAmmoReloadCooldown);  
                                gunAnim.anim.SetBool("OutofAmmo", false);
                            }
                            else{
                                CancelInvoke();
                                Debug.Log("Doing Reload!");
                                canShoot = false;
                                canReload = false;
                                reloading = true;
                                firing = false;
                                animator.Play("Reload");
                                gunAnim.PlayReload();
                                Invoke("ResetCanShoot", gunAnim.reloadFireCooldown);
                                Invoke("ResetCanReload", gunAnim.reloadCooldown);
                            }

                        }
		        	}
	        	}
	        }

            playerSpeed = sphere.GetComponent<Rigidbody>().linearVelocity.magnitude;
            if (playerSpeed < .001f && !movement.moveBlocked) {
                animator.SetFloat("Blend", 0f);
            }
            else {
                if (playerSpeed >= 15f && !movement.moveBlocked) {
                    animator.SetFloat("Blend", 1f);
                }
                else {
                    playerSpeed2 = playerSpeed / 15f;
                    animator.SetFloat("Blend", playerSpeed2);

                    if (playerSpeed >= 10f && !movement.moveBlocked) {
                        animator.SetFloat("walkBlend", 1f);
                    }
                    if (playerSpeed < .001f && !movement.moveBlocked) {
                        animator.SetFloat("walkBlend", 0f);
                    }
                    playerSpeed = playerSpeed / 10f;
                    animator.SetFloat("walkBlend", playerSpeed);
                }
            }
        }
    }
}
