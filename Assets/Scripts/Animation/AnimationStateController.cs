using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I need to properly use hashes, im kinda half assing it here
// is on wall stays true after bumping into a ridigbody in water

public class AnimationStateController : MonoBehaviour
{
    public GameObject player = default;
    Movement sphere = default; 
    Animator animator;
	int isWalkingHash;
	int isRunningHash;
	int isSprintingHash;
    int isOnSteepHash;
    int isJumpingHash;
    int onGroundHash;
    int isOnWallHash;
    int isFallingHash;
    bool isOnGround;
    bool isOnWall;

    [HideInInspector]
    public bool isOnGroundADJ;
    bool isOnSteep;
    //bool isOnSteepADJ;

    bool JumpPressed;
    [SerializeField]
    [Tooltip("how long you need to be in the air before the 'onGround' bool triggers")]
    float OnGroundBuffer = .5f;
    [SerializeField]
    [Tooltip("how long isJumping stays true after pressing it ( maybe should be in movingsphere?)")]
    float JumpBuffer = .5f;
    bool JumpSwitch = true;
    float Groundstopwatch = 0;
    float Jumpstopwatch = 0;



    void JumpAnimEvent(){
		sphere.JumpTrigger();
	}

 
    void Start() { 
        sphere = player.GetComponent<Movement>();
        animator = GetComponent<Animator>();

		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		isSprintingHash = Animator.StringToHash("isSprinting");
        isJumpingHash = Animator.StringToHash("isJumping");
        onGroundHash = Animator.StringToHash("OnGround");
        isOnWallHash = Animator.StringToHash("isOnWall");
        isFallingHash = Animator.StringToHash("isFalling");

    }

    //this is meant to allow a sort of buffer, so bools stay true for a set amount of time
    void BoolAdjuster(){
        isOnGround = sphere.OnGround;
        isOnSteep = sphere.OnSteep;
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
    float jumpCount;
    float jumpCap = .2f;
    void Update() {
        //Debug.Log(sphere.velocity.magnitude);
        BoolAdjuster();
        bool JumpPressed = Input.GetKey(sphere.controls.keys["jump"]);
        isOnGround = isOnGroundADJ;
        bool isFalling = animator.GetBool(isFallingHash);
        bool isOnWall = animator.GetBool(isOnWallHash);
		bool isRunning = animator.GetBool(isRunningHash);
		bool isWalking = animator.GetBool(isWalkingHash);
		bool isSprinting = animator.GetBool(isSprintingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
		bool SprintPressed = Input.GetKey(sphere.controls.keys["sprint"]);
        bool WalkPressed = Input.GetKey(sphere.controls.keys["duck"]);
        bool forwardPressed = Input.GetKey(sphere.controls.keys["walkUp"]);
        bool leftPressed = Input.GetKey(sphere.controls.keys["walkLeft"]);
        bool rightPressed = Input.GetKey(sphere.controls.keys["walkRight"]);
        bool backPressed = Input.GetKey(sphere.controls.keys["walkDown"]);
        bool movementPressed = forwardPressed || leftPressed || rightPressed || backPressed;

        if (isOnGround){
            animator.SetBool(onGroundHash, true);
        }
        else if (!isOnGround){
            animator.SetBool(onGroundHash, false);
        }
        //This makes jump stay true a little longer after you press it, dependent on "JumpBuffer"
        if (JumpPressed){
            if(JumpSwitch){
                Jumpstopwatch = 0;
                animator.SetBool(isJumpingHash, true);
                JumpSwitch = false;
            }
            else{
                Jumpstopwatch += Time.deltaTime;
                    if(Jumpstopwatch >= JumpBuffer){
                        animator.SetBool(isJumpingHash, false);
                    }
            }   
        }
        //this activates when jump is not pressed, counts until jumpbuffer, then disables jump
        if(!JumpPressed){
            JumpSwitch = true;
            Jumpstopwatch += Time.deltaTime;
            if(Jumpstopwatch >= JumpBuffer){
                animator.SetBool(isJumpingHash, false);
            }
        }
        
        // if you are in the air, adding timer to give a little time before the falling animation plays
        if (!isOnGroundADJ && !isOnSteep){
            jumpCount += Time.deltaTime;
            if(jumpCount > jumpCap){
                animator.SetBool(isFallingHash, true);
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isSprintingHash, false);
                animator.SetBool(isRunningHash, false);
                jumpCount = 0f;
            }

        }
        else if(isOnGroundADJ || isOnSteep ){
            jumpCount = 0f;
        }

        else if (!isOnGroundADJ && isOnSteep ){
            animator.SetBool(isOnWallHash, true);
        }

        if (isOnGroundADJ){
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isOnWallHash, false);
        }

        if (isOnSteep){
            animator.SetBool("isOnSteep", true);
        }

        if (!isOnSteep){
            animator.SetBool("isOnSteep", false);
            animator.SetBool(isOnWallHash, false);
        }


        if (!isWalking && (movementPressed && WalkPressed)){
            animator.SetBool(isWalkingHash, true);
            
        }
        if (isWalking && (!movementPressed || !WalkPressed)){
            animator.SetBool(isWalkingHash, false);
        }


        if (!isSprinting && (movementPressed && SprintPressed && !WalkPressed )){
            animator.SetBool(isSprintingHash, true);
            
        }
        if (isSprinting && (!movementPressed || !SprintPressed || WalkPressed)){
            animator.SetBool(isSprintingHash, false);
        }

        if (!isRunning && movementPressed && !WalkPressed && !SprintPressed && sphere.velocity.magnitude > 0 ){
            animator.SetBool(isRunningHash, true);
        }
        if ((isRunning && !movementPressed || WalkPressed || SprintPressed ) || sphere.velocity.magnitude <= 0.08f){
            animator.SetBool(isRunningHash, false);
        }
    }

}
