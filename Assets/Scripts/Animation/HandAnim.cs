//Author: Travis Parks
//Debugging: Travis Parks
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script controls all the animations tied to the character, such as when certain animations should be played and how they should be played.
public class HandAnim : MonoBehaviour
{
    Controls controls;
    MovementSpeedController speedController;
    [SerializeField]
    GameObject sphere;
    Movement movement;
    [HideInInspector]
    public bool barragePrep = false;
    bool flipflop = true;
    //bool flipflop2 = true;
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
    public void setisClimbing(bool plug){
        animator.SetBool("isClimbing", plug);
    }
    public bool getisClimbing(){
        return animator.GetBool("isClimbing");
    }
    public void setisThrowing(bool plug){
        animator.SetBool("grabCharge", !plug);
        animator.SetBool("isThrowing", plug);
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
        animator.SetBool("Interact", true);
        Invoke("resetInteract", .1f);
    }
    void Start()
    {
        controls = GameObject.Find("Data").GetComponentInChildren<Controls>();
        speedController = sphere.GetComponent<MovementSpeedController>();
        //animator = GetComponent<Animator>();
        movement = sphere.GetComponent<Movement>();
        grab = GetComponent<Grab>();
    }

    void startBarrage(){
        animator.SetBool("barragePrep", false);
        movement.isBarraging = true;
        //flipflop2 = true;
        //speedController.setFactor(.5f);
        //Debug.Log("SPEED CUT");
        //movement change here

    }

    void resetbarragePrep(){
        barragePrep = false;
        //speedController.setFactor(2f);
        //Debug.Log("SPEED BOOSTED");
        //movement restoration here
    }

    void openGate(){
        blocker = true;
    }
    void closeGate(){
        blocker = true;
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

    public void setIsHoldingFoodTrue(){
        animator.SetFloat("isHoldingFood", 1);
    }
    public void setIsHoldingFoodFalse(){
        animator.SetFloat("isHoldingFood", 0);
    }
    void resetHoldingChange(){
        animator.SetBool("holdingChange", false);
    }
    void resetEatFood(){
        animator.SetBool("EatFood", false);
    }
    public void setEatFood(){
        animator.SetBool("EatFood", true);
        Invoke("resetEatFood", .1f);
    }
    public void forceIdle(){
        animator.SetBool("isMoving", false);
        animator.SetBool("isClimbing", false);
        animator.SetBool("HungerDive", false);
        animator.SetBool("isSprinting", false);
        animator.SetBool("walkPressed", false);
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
            if (movement.ClimbingADJ) {
                animator.SetFloat("ClimbingX", movement.velocity.x);
                animator.SetFloat("ClimbingY", movement.velocity.y);
                animator.SetBool("isClimbing", true);
            }
            else if (!movement.ClimbingADJ) {
                animator.SetBool("isClimbing", false);
            }
            if (movement.Diving) {
                animator.SetBool("HungerDive", true);
            }
            else if (!movement.Diving) {
                animator.SetBool("HungerDive", false);
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

            //Barrage Attack
           // if (Input.GetKeyDown("mouse 1")) {
           //     if (flipflop2 && !movement.isBarraging && !grab.isHolding) {
           //         animator.SetBool("barragePrep", true);
           //         blocker = false;
           //         flipflop2 = false;
           //     }
           //     else if (!flipflop2) {
           //         movement.isBarraging = false;
           //         animator.SetBool("barragePrep", false);
           //         blocker = true;
           //         flipflop2 = true;
           //     }
           // }


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
            if (Input.GetKey(controls.keys["throw"])) {
                if (barragePrep) {
                    animator.SetBool("charging", true);

                }
                else if (!barragePrep) {
                    animator.SetBool("charging", false);
                    movement.isBarraging = false;
                }
            }
            else if (!Input.GetKey(controls.keys["throw"])) {
                animator.SetBool("charging", false);
                movement.isBarraging = false;
            }
        }
    }
}
