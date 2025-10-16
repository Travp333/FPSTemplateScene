//Author: Travis Parks
//Debugging: Travis Parks
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;
using UnityEngine.InputSystem;
//this script controls all the animations tied to the character's hands, such as when certain animations should be played and how they should be played.
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
    public GunLogic gunLogic;
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
    int handBurstCount;
    bool burstBlock;
    // Start is called before the first frame update
    //enable and disable IK are called by the animation, and basically are just there to lock the off hand to the weapon. Does not need to be called, can be decided per weapon
    public void EnableOffHandIK()
    {
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
	public void PickUpWeapon()
    {
        //Debug.Log("Playing Draw Anim");
        animator.Play("Draw");
        if (gunAnim != null)
        {
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
    //coyote time, how long you are still considered on ground after leaving ground. This lets the hand animations sync in a more satisfying way
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
        // reference to movement script, needed to check movememnt state and match animations
		movement = sphere.GetComponent<Movement>();
        // reference to pause menu, needed to ensure input is ignored while paused
        pause = FindFirstObjectByType<PauseMenu>();
        // reference to attack keybind, needed for attacking
		attackAction = sphere.GetComponent<PlayerInput>().currentActionMap.FindAction("Attack");
        // reference to reload keybind, needed for reloading
		reloadAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Reload");
        // reference to interact script, needed for checking if you are colliding with a wall
        inter = this.GetComponent<Interact>();
        // reference to script that adjust your movement speed, this lets you slow down the player when holding something.
        speedController = sphere.GetComponent<MovementSpeedController>();

        // reference to grab component, needed to check holding item state
        grab = GetComponent<Grab>();
    }
    public void forceIdle(){
        animator.SetBool("isMoving", false);
        animator.SetBool("isSprinting", false);
        animator.SetBool("walkPressed", false);
    }
    //punch left
	void PunchStartL(){
		blocker = false;
		flipflop = !flipflop;
        animator.Play("Left Hook");
		//animator.SetBool("isPunchingLeft", true);
		Invoke("PunchStop", .1f);
	}
    //punch right
	void PunchStartR(){
		blocker = false;
		flipflop = !flipflop;
        animator.Play("Right Hook");
		//animator.SetBool("isPunchingRight", true);
		Invoke("PunchStop", .1f);
	}
    // end punch anim
	void PunchStop(){
		animator.SetBool("isPunchingLeft", false);
		animator.SetBool("isPunchingRight", false);
		blocker = true;
	}
    //enable ability to shoot
	void ResetCanShoot(){
        //Debug.Log("Can Shoot Again!");
		canShoot = true;
        //Debug.Log("reloading set to false via resetCanShoot()");
		reloading = false;
		firing = false;
	}
    // enable ability to reload
	void ResetCanReload(){
        //Debug.Log("Can Reload!");
		canReload = true;
        //Debug.Log("reloading set to false via resetCanReload()");
		reloading = false;
		firing = false;
	}
    //play shooting animation on hand, then on gun, then recover based on cooldowns from weapon
    void Shoot(){
        if (canShoot && !reloading)
        {
            //play hand animation fire
            animator.Play("Fire", 0, 0f);
            //Debug.Log("Firing");
            //block ability to shoot and reload
            canShoot = false;
            canReload = false;
            //Debug.Log("reloading set to false via Shoot");
            //update states
            reloading = false;
            firing = true;
            //reset blocks based on cooldowns assigned to weapon
            Invoke("ResetCanShoot", gunLogic.fireCooldown);
            Invoke("ResetCanReload", gunLogic.fireCooldown);
            // play gun animation fire
            gunAnim.PlayFire();
            //check if this weapon is burst fire, if so recursively call shoot again after checking some flags
            if (gunLogic.burst && !gunLogic.bursting && ammomanager.roundInChamber.Count > 0)
            {
                burstBlock = true;
                Debug.Log("Starting Burst");
                gunLogic.bursting = true;
                handBurstCount = gunLogic.burstCount;
                handBurstCount--;
                ResetCanShoot();
                Invoke("Shoot", gunLogic.inBetweenBurstCooldown);
            }
            // continue bursting if this was called recursively by burst
            else if (gunLogic.burst && gunLogic.bursting && handBurstCount > 0 && ammomanager.roundInChamber.Count > 0)
            {
                Debug.Log("Continuing Burst");
                handBurstCount--;
                ResetCanShoot();
                Invoke("Shoot", gunLogic.inBetweenBurstCooldown);

            }
            // end bursting if this was called recursively by burst at the end of the burst
            else if (gunLogic.burst)
            {
                Debug.Log("Ending Burst");
                gunLogic.bursting = false;
                handBurstCount = 0;
                Invoke("ResetBurstBlock", gunLogic.burstCooldown);
            }
        }
    }
    public void ResetBurstBlock()
    {
        Debug.Log("Resetting Burst Block");
        burstBlock = false;
    }
    public void ResetFireable()
    {
        if (gunAnim != null)
        {
            Debug.Log("Resetting ability to fire");
            Invoke("ResetCanShoot", gunLogic.fireCooldown);
            Invoke("ResetCanReload", gunLogic.fireCooldown);
        }
    }
    //call when trying to shoot with no ammo
    void OutOfAmmoShoot()
    {
        if (canShoot && !reloading)
        {
            Debug.Log("Trying to fire, no ammo!");
            // play out of ammo fire animation on hand
            animator.Play("OutOfAmmoFire", 0, 0f);
            canShoot = false;
            canReload = false;
            //Debug.Log("reloading set to false via outofammoshoot()");
            reloading = false;
            firing = true;
            Invoke("ResetCanShoot", gunLogic.fireCooldown);
            Invoke("ResetCanReload", gunLogic.fireCooldown);
        }
    }
   // public void ForceGunAnimIdle(){
   //     if(gunAnim != null){
   //         gunAnim.anim.Play("Idle", 0, 0f);
   //     }
   // }
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
        //If NOT paused, basically just ensuring none of this stuff happens while the game is paused
        if (!pause.isPaused) {
            // update locational states, ie on steep, crouching, etc
            if (isOnSteep)
            {
                animator.SetBool("OnSteep", true);
            }
            else if (!isOnSteep)
            {
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
            if (isOnGround ) {
                animator.SetBool("isOnGround", true);
            }
            else if (!isOnGround ) {
                animator.SetBool("isOnGround", false);
            }
            //this OnGround stays true for a little bit after you leave the ground, hence ADJ
            if (isOnGroundADJ)
            {
                animator.SetBool("isOnGroundADJ", true);
            }
            else if (!isOnGroundADJ)
            {
                animator.SetBool("isOnGroundADJ", false);
            }
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
            //are you holding a gun, not colliding with a wall, and not moveblocked?
            if (gunAnim != null && !inter.isWallColliding && !movement.moveBlocked)
            {
                // do you have a magazine loaded?
                if (ammomanager.loadedMag != false)
                {
                    //Full Auto
                    if (gunLogic.fullAuto && attackAction.IsPressed())
                    {
                        // not relaoding, holding a weapon, have ability to shoot, and you have ammo in your magazine
                        if (!reloading && holdingWeapon && canShoot && ammomanager.roundInChamber.Count > 0)
                        {
                            Shoot();
                            //checking to see if the mag is now empty after firing
                            if (ammomanager.roundInChamber.Count == 0)
                            {
                                gunAnim.anim.SetBool("OutofAmmo", true);
                                canReload = true;
                            }
                        }
                        // no ammo? do noammoshoot
                        if (ammomanager.roundInChamber.Count == 0)
                        {
                            // can you shoot?
                            if (canShoot)
                            {
                                // if so, play empty gun firing animation
                                gunAnim.anim.SetBool("OutofAmmo", true);
                                OutOfAmmoShoot();
                                canReload = true;
                            }
                        }
                    }
                    // you are not reloading, your gun is full auto, you just stopped holding attack, you are holding a weapon, you are not moveblocked, and you can shoot
                    // basically just allow you to shoot again once you release
                    else if (!reloading && gunLogic.fullAuto && attackAction.WasReleasedThisFrame() && holdingWeapon && !movement.moveBlocked && canShoot)
                    {
                        //Debug.Log("Why is this firing?");
                        ResetFireable();
                    }
                    //Semi Auto / burst
                    if (attackAction.WasPressedThisFrame() && !gunLogic.fullAuto && !movement.moveBlocked)
                    {
                        if (!burstBlock && !reloading && holdingWeapon && canShoot && ammomanager.roundInChamber.Count > 0)
                        {
                            Shoot();
                            //checking to see if the mag is now empty after firing
                            if (ammomanager.roundInChamber.Count == 0)
                            {
                                gunAnim.anim.SetBool("OutofAmmo", true);
                                canReload = true;
                            }
                        }
                        else if (burstBlock)
                        {
                            Debug.Log("BLOCKED BY BURSTBLOCK");
                        }
                        else if (ammomanager.roundInChamber.Count == 0)
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
                // no magazine loaded, play empty magazine anim
                else if (attackAction.WasPressedThisFrame() && canShoot)
                {
                    gunAnim.anim.SetBool("OutofAmmo", true);
                    OutOfAmmoShoot();
                    canReload = true;
                }
            }
	        if (attackAction.WasPressedThisFrame() && !holdingWeapon && blocker && !grab.isHolding && !movement.moveBlocked) {
		        if (flipflop) {
			        Invoke("PunchStartL", .1f);
		        }
		        else if (!flipflop) {
			        Invoke("PunchStartR", .1f);
		        }
	        }

            if (reloadAction.WasPressedThisFrame() && !inter.isWallColliding && !movement.moveBlocked)
            {
                if (holdingWeapon)
                {
                    if (gunAnim != null)
                    {
                        if (ammomanager.CanReload())
                        {
                            Debug.Log("Starting Reload");
                            // you have a valid magazine loaded
                            if (ammomanager.loadedMag != false)
                            {
                                // no bullet in the chamber, gun empty
                                if (ammomanager.roundInChamber.Count == 0)
                                {
                                    burstBlock = false;
                                    CancelInvoke();
                                    Debug.Log("Doing No Round Reload!");
                                    canShoot = false;
                                    canReload = false;
                                    reloading = true;
                                    firing = false;
                                    animator.Play("NoRoundReload");
                                    gunAnim.PlayNoRoundReload();
                                    Invoke("ResetCanShoot", gunLogic.noAmmoReloadFireCooldown);
                                    Invoke("ResetCanReload", gunLogic.noAmmoReloadCooldown);
                                    gunAnim.anim.SetBool("OutofAmmo", false);
                                }
                                // there is a round in the chamber, so only replace magazine
                                else
                                {
                                    burstBlock = false;
                                    CancelInvoke();
                                    Debug.Log("Doing Base Reload!");
                                    canShoot = false;
                                    canReload = false;
                                    reloading = true;
                                    firing = false;
                                    animator.Play("Reload");
                                    gunAnim.PlayReload();
                                    Invoke("ResetCanShoot", gunLogic.reloadFireCooldown);
                                    Invoke("ResetCanReload", gunLogic.reloadCooldown);
                                }
                            }
                            //there is no magazine loaded
                            else
                            {
                                //Debug.Log("There is no valid magazine loaded");
                                // no bullet in the chamber, gun completely empty
                                if (ammomanager.roundInChamber.Count == 0)
                                {
                                    burstBlock = false;
                                    CancelInvoke();
                                    Debug.Log("Doing No mag No round in chamber Reload!");
                                    canShoot = false;
                                    canReload = false;
                                    reloading = true;
                                    firing = false;
                                    animator.Play("NoRoundNoMagReload");
                                    gunAnim.PlayNoRoundNoMagReload();
                                    Invoke("ResetCanShoot", gunLogic.noAmmoReloadFireCooldown);
                                    Invoke("ResetCanReload", gunLogic.noAmmoReloadCooldown);
                                    gunAnim.anim.SetBool("OutofAmmo", false);
                                }
                                // there is a round in the chamber, so replace magazine
                                else
                                {
                                    burstBlock = false;
                                    CancelInvoke();
                                    Debug.Log("Doing No Mag round in chamber Reload!");
                                    canShoot = false;
                                    canReload = false;
                                    reloading = true;
                                    firing = false;
                                    animator.Play("NoMagReload");
                                    gunAnim.PlayNoMagReload();
                                    Invoke("ResetCanShoot", gunLogic.reloadFireCooldown);
                                    Invoke("ResetCanReload", gunLogic.reloadCooldown);
                                }
                            }


                        }
                        else
                        {
                            Debug.Log("Failed ammomanagers canReload test");
                        }
                    }
                    else
                    {
                        Debug.Log("GunAnim is Null");
                    }
                }
                else
                {
                    Debug.Log("Failed holdingWeapon ");
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
