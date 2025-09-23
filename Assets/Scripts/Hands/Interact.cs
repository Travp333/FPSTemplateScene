//Author: Travis Parks
//Debugging: Travis Parks, Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
//this script handles interacting with various objects, such as a button, a lever, a pickupable object, etc. essentially just pressing e on something
public class Interact : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> TempAmmoValue = new List<GameObject>();
    [SerializeField]
    GameObject wallCollisionCheckBox;
    public bool isWallColliding;
	[SerializeField]
	public AnimatorOverrideController baseOverride;
	[SerializeField]
	public Transform MagGrabPoint;
	public InputAction interactAction;
	public InputAction dropAction;
	GameObject gun;
	[SerializeField]
	public GameObject GunGrabPoint;
    //Components
    Movement movement;
    Grab grab;
    HandAnim hand;
    [HideInInspector]
    public Transform prop;
    [HideInInspector]
    public Rigidbody propRB;
    [SerializeField]
    [Tooltip("where a grabbed prop attaches")]
	Transform holdPoint;
	[SerializeField]
	[Tooltip("where a interact raycast points to")]
	Transform castPoint;
    [SerializeField]
    [Tooltip("where the throwing force originates from")]
    public GameObject origin;
	heldItemColliderToggle colTog;
    //Variables
    [SerializeField]
    float distance;
    int ballLength;
    [SerializeField]
    [Tooltip("what objects can be picked up by the player")]
    LayerMask mask = default;
    Transform propParent;
	Transform ragdollParent;
	PauseMenu pause;

    //NEW STUFF FROM INVENTORY SYSTEM
    tempHolder tempSlot;
    [SerializeField]
    Inven inv;
    [SerializeField]
	SimpleCameraMovement camScript = null; 
    [SerializeField]
	public List<GameObject> StorageInvenUI = new List<GameObject>();
    [SerializeField]
	GameObject InventoryUI = null; //Inventory Canvas
    //bool to track inventory being open and closed
	public bool invIsOpen = false;
    bool distanceGate = false;
	Transform storageObjectPos;
    [SerializeField]
	[Tooltip("The distance that the Inventory closes when walking away from an open storage object")]
    float invRange;
    //tracks if a storage device's inventory is open
	public bool storageInvOpen = false;
    public Transform cam;
    [SerializeField]
	[Tooltip("How wide the sphere is that is cast from the player when hitting e")]
	float sphereCastRadius = .5f;
    //raycast hit holder
	RaycastHit hit;
    // holder for inventory Items
	Item item;
    //NEW STUFF FROM INVENTORY SYSTEM
    Magazine mag;
    // Start is called before the first frame update
    void Start()
	{
        //NEW STUFF FROM INVENTORY SYSTEM
        //plugging references
		tempSlot = this.gameObject.GetComponent<tempHolder>();
		//disable all Inventory UI
		HideAllInventories();
        //NEW STUFF FROM INVENTORY SYSTEM

		//Assign components
		pause = FindFirstObjectByType<PauseMenu>();
		movement = transform.root.GetComponent<Movement>();
		dropAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Drop");
		interactAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Interact");
        grab = GetComponent<Grab>();
        hand = GetComponent<HandAnim>();
	    colTog = GetComponentInChildren<heldItemColliderToggle>();
    }
    //NEW STUFF FROM INVENTORY SYSTEM
    public void HideAllInventories(){
		//Loop through all the UIPlugger objects in the scene and add them to a list while also disabling them.
		foreach(UiPlugger g in GameObject.FindObjectsByType<UiPlugger>(FindObjectsSortMode.None)){
			//avoids adding duplicates
			if(g.gameObject.transform.GetChild(0).gameObject.activeInHierarchy == true){
				if(StorageInvenUI.Contains(g.gameObject.transform.GetChild(0).gameObject)){
					//Debug.Log("Just hiding UI");
					g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
				}
				else{
					//Debug.Log("Hiding Ui and Adding to list");
					StorageInvenUI.Add(g.gameObject.transform.GetChild(0).gameObject);
					g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		}
	}

    public void HideAllNonPlayerInventories(){
		//Loop through all the UIPlugger objects in the scene and add them to a list while also disabling them.
		foreach(UiPlugger g in GameObject.FindObjectsByType<UiPlugger>(FindObjectsSortMode.None)){
			//avoids adding duplicates
			if(g.gameObject.tag != "Player"){
				if(g.gameObject.transform.GetChild(0).gameObject.activeInHierarchy == true){
					if(StorageInvenUI.Contains(g.gameObject.transform.GetChild(0).gameObject)){
						//Debug.Log("Just hiding UI");
						g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
					else{
						//Debug.Log("Hiding Ui and Adding to list");
						StorageInvenUI.Add(g.gameObject.transform.GetChild(0).gameObject);
						g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
				}
			}
		}
	}
	void OpenInventory(){
		//makes sure the temp slot is empty
		tempSlot.ClearSlot();
		//puts cursor on screen
		Cursor.lockState = CursorLockMode.Confined; 
		//unhides cursor
		Cursor.visible = true;
		//enables UI object
		InventoryUI.SetActive(true);
		//disable camera movement script
        movement.blockMovement();
		camScript.enabled = false;
		invIsOpen = true;
	}
	void DistanceCheck(){
		if(Vector3.Distance(this.transform.position, storageObjectPos.position) > invRange){
			Debug.Log("TOO FAR!!!!!");
			distanceGate = false;
			storageObjectPos = null;
			HideAllNonPlayerInventories();
			tempSlot.ClearSlot();
			storageInvOpen = false;
		}
	}
	
	//closes out the inventory and all open storage inventories, mostly just inverse of above
	void CloseInventory(){
		tempSlot.ClearSlot();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		camScript.enabled = true;//enable camera movement script
        movement.unblockMovement();
		invIsOpen = false;
		HideAllInventories();
		storageInvOpen = false;
	}

    //NEW STUFF FROM INVENTORY SYSTEM

    List<Transform> GetAllChildren(Transform _t)
    {
        List<Transform> ts = new List<Transform>();
 
        foreach (Transform t in _t)
        {
            ts.Add(t);
            if (t.childCount > 0)
                ts.AddRange(GetAllChildren(t));
        }
 
        return ts;
    }

    //If not already holding an object, get object components, pass to Grab to do the work
    public void pickUp(GameObject obj){
	    if (!grab.isHolding) {
	        castPoint.gameObject.GetComponent<heldObjectReferenceHolder>().HeldObjectReference = obj;
            prop = obj.GetComponent<Transform>();
            propParent = prop.transform.root;
	        propRB = obj.GetComponent<Rigidbody>();
            
	        //Picking Up object, checking if its a ragdoll
            
            if(prop.gameObject.tag == "ragdoll"){
                ragdollParent = prop.parent;
                foreach(Transform G in obj.transform.root.GetComponentsInChildren<Transform>()){
                    G.gameObject.layer = 16;
                }
                foreach (GameObject G in GameObject.FindGameObjectsWithTag("ragdoll")){
                    if(G.GetComponent<objectSize>()!= null && G.GetComponent<objectSize>().isHeld){
                        G.layer = 16;
                        foreach (Rigidbody R in G.GetComponentsInChildren<Rigidbody>()){
                            R.gameObject.layer = 16;
                        }
                    }
                }
            }
            grab.pickUp(holdPoint, prop, propRB, obj);
 
        }

    }
    // this just makes you drop whatever you are holding
    public void Detach(){
	    grab.sizes = Grab.objectSizes.none;
	    colTog.clear();
        hand.setisHoldingFalse();
        if(prop.gameObject.tag == "ragdoll"){
	        castPoint.gameObject.GetComponent<heldObjectReferenceHolder>().HeldObjectReference.transform.SetParent(ragdollParent);
        }
        else{
	        castPoint.gameObject.GetComponent<heldObjectReferenceHolder>().HeldObjectReference.transform.SetParent(null);
        }
	    castPoint.gameObject.GetComponent<heldObjectReferenceHolder>().HeldObjectReference = null;
        //opposite of the pick up section, just undoing all of that back to its default state
        grab.isgrabCharging = false;
        propRB.isKinematic=(false);
        grab.isHolding = false;
        foreach (GameObject G in GameObject.FindGameObjectsWithTag("ragdoll")){
            G.layer = 13;
            foreach (Rigidbody R in G.GetComponentsInChildren<Rigidbody>()){
                R.gameObject.layer = 13;
            }
        }
        prop.transform.gameObject.layer = 13;
        foreach ( Transform child in prop.transform){
            child.transform.gameObject.layer = 13;
            foreach ( Transform child2 in child.transform){
                child2.transform.gameObject.layer = 13;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //NEW STUFF FROM INVENTORY SYSTEM 
        //Debug.Log(inv.isPickedUp);
        //Check Inventory
        if (invIsOpen)
        {
        	//pressing tab with the inventory open 
            if (Input.GetKeyDown("tab")) 
            {
	            CloseInventory();
            }
        }
        else if (!invIsOpen) 
        {
            //NEW STUFF FROM INVENTORY SYSTEM 
            //IF not paused
            if (!pause.isPaused) {
                //inventory is not open
                if (Input.GetKeyDown("tab"))
                {
                    OpenInventory();
                }
                //IF e pressed
                if (interactAction.WasPressedThisFrame())
                {
                    // if you are not holding anything
                    if (!grab.isHolding)
                    {
                        RaycastHit hit;
                        //IF a raycast hits something
                        if (Physics.SphereCast(origin.transform.position, 1f, (castPoint.position - origin.transform.position), out hit, distance, mask))
                        {
                            Debug.DrawRay(origin.transform.position, (castPoint.position - origin.transform.position), Color.green, 5f);
                            Debug.DrawLine(origin.transform.position, hit.point, Color.red, 5f);
                            //Get the properties of the something you hit
                            propRB = hit.rigidbody;
                            prop = hit.transform;
                            // If the thing is an NPC start dialogue
                            if(hit.transform.gameObject.tag == "NPC" && hit.transform.gameObject.GetComponent<Rigidbody>() == null){
                                if(FindFirstObjectByType<DialogueManager>().dialogueBox.activeInHierarchy == false){
                                    hit.transform.parent.gameObject.GetComponent<NpcDialogue>().Begin();
                                }
                                else{
                                    FindFirstObjectByType<DialogueManager>().DisplayNextSentence();
                                }
                            }
                            //-----------------------------------------------------------------------
                            //hit a pickupable item?
                            else if (hit.transform.gameObject.GetComponent<pickUpableItem>() != null)
                            {
                                Debug.Log("HIT PICK UP ABLE ITEM!");
                                //store reference to the hit object
                                item = hit.transform.gameObject.GetComponent<pickUpableItem>().item;
                                //update UI
                                int count;
                                count = hit.transform.gameObject.GetComponent<pickUpableItem>().count;
                                if (count > 1)
                                {
                                    //does this stack contain multiple objects?
                                    // if so, pick up x amount of times!

                                    for (int i = 0; i < count; i++)
                                    {
                                        //magazines will never be in a stack, they always have a stack size of 1, so null it here
                                        inv.SmartPickUp(item, null);
                                    }
                                }
                                //magazines will never be in a stack, they always have a stack size of 1 
                                else
                                {
                                    if (hit.transform.gameObject.GetComponent<AmmoList>() != null)
                                    {
                                        inv.SmartPickUp(item, hit.transform.gameObject.GetComponent<AmmoList>().Ammo);
                                    }
                                    else
                                    {
                                        // not a magazine, send null
                                        inv.SmartPickUp(item, null);
                                    }
                                }
                                //Checks if the object was successfully picked up
                                if (inv.isPickedUp)
                                {
                                    //despawn object in the world
                                    Destroy(hit.transform.gameObject);
                                    inv.isPickedUp = false;
                                    //Debug.Log("ispickedup set to "+ inv.isPickedUp);
                                }
                                //else, pickup failed, respawn the prefab at players feet
                                else
                                {
                                    if(count > 1){
                                        //Does this stack contain multiple objects?
                                        //if so update prefab to have accurate count
                                        GameObject it;
                                        // we know it is not a magazine since it is a stack, send null
                                        it = inv.SpawnItem(item.prefab, null);
                                        it.GetComponent<pickUpableItem>().count = count;
                                        Destroy(hit.transform.gameObject);
                                        Debug.Log("Inventory full!");
                                    }
                                    else{
                                        // is this a magazine? if so, also pass ammo list
                                        if (hit.transform.gameObject.GetComponent<AmmoList>() != null)
                                        {
                                            inv.SpawnItem(item.prefab, hit.transform.gameObject.GetComponent<AmmoList>().Ammo);
                                        }
                                        else
                                        {
                                            // not a magazine, send null
                                            inv.SpawnItem(item.prefab, null);
                                        }
                                        // normal drop
                                        Destroy(hit.transform.gameObject);
                                        Debug.Log("Inventory full!");
                                    }

                                }
                            }
                            //if you did not hit a pickupable object, check if you hit a storage device
                            else if(hit.transform.gameObject.GetComponent<Inven>() != null){
                                Debug.Log("HIT Storage Device !!!");
                                if(hit.transform.gameObject.tag != "Player"){
                                    Inven inv = hit.transform.gameObject.GetComponent<Inven>();
                                    //enable the relevant UI element
                                    inv.UIPlugger.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                                    storageInvOpen = true;
                                    //force open the player's inventory
                                    OpenInventory();
                                    //Add distance check here
                                    storageObjectPos = inv.gameObject.transform;
                                    distanceGate = true;
                                }
                            }
                            //--------------------------------------------------------------------------------
                            //IF the thing you hit has a rigidbody that is light enough for the player to hold
                            else if (hit.transform.gameObject.GetComponent<WeaponType>() == null && hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic == false && hit.transform.gameObject.GetComponent<Rigidbody>().mass <= grab.strength && !grab.justThrew && !hand.holdingWeapon)
                            {
                                //Debug.Log("HIT!!");
                                //Pick it up
                                pickUp(prop.gameObject);
                            }
                            
                            //Maybe revise this to just be a generic "Interactable"?
                            
                            else if (hit.transform.gameObject.GetComponent<buttonPush>() != null)
                            {
                                //Get the button object
                                buttonPush button = hit.transform.gameObject.GetComponent<buttonPush>();
                                if (!button.blocker)
                                {
                                    button.press();
                                }
                            }
                            
                            //is the thing you interacted with a weapon? can you currently hold a weapon?

                            else if (hit.transform.gameObject.GetComponent<WeaponType>() != null && !hand.holdingWeapon)
                            {
                                WeaponType wep = hit.transform.gameObject.GetComponent<WeaponType>();
                                gun = Instantiate(wep.playerModel, GunGrabPoint.transform.position, Quaternion.identity);
                                gun.transform.parent = GunGrabPoint.transform;
                                hand.gunAnim = gun.GetComponent<GunAnim>();
                                hand.gunLogic = gun.GetComponent<GunLogic>();
                                wallCollisionCheckBox.transform.localScale = new Vector3(wallCollisionCheckBox.transform.localScale.x, wallCollisionCheckBox.transform.localScale.y, wallCollisionCheckBox.transform.localScale.z * hand.gunAnim.wallCollisionCheckSizeAdjust);
                                wallCollisionCheckBox.transform.localPosition = new Vector3(wallCollisionCheckBox.transform.localPosition.x, wallCollisionCheckBox.transform.localPosition.y, wallCollisionCheckBox.transform.localPosition.z * hand.gunAnim.wallCollisionCheckPosAdjust);
                                hand.ammomanager = gun.GetComponent<AmmoManager>();
                                hand.canShoot = true;
                                hand.canReload = true;
                                mag = hit.transform.gameObject.GetComponent<Magazine>();
                                gun.GetComponent<GunAnim>().mag = mag;
                                //overwrite ammo in magazine value with value stored in the gun itself. This allows you to have persistent ammo when you drop and pick back up a weapon
                                //Debug.Log("Gun in hand has " + gun.GetComponent<AmmoManager>().ammoInMag+ " in magazine, " + "Gun picked up has " + mag.ammo + " In magazine, rewriting...");

                                //

                                gun.GetComponent<AmmoManager>().Ammo = mag.Ammo;
                                //Debug.Log("Gun in hand now has "+ gun.GetComponent<AmmoManager>().ammoInMag + " In Magazine");
                                //now the overrides, this basically just overwrites all the animations of the player to match whatever gun they just picked up. Anim overriders should be attached to each weapon
                                //check that an override was included with this weapon
                                if (wep.animOverride.Length > 0)
                                {
                                    //store it in the handanim script. It is a list so that you could potentially have multiple overrides on one weapon, in case a weapons animations goes through many different animation phases
                                    hand.handInHandAnimOverride = wep.animOverride;
                                    //just grab the first entry for now as that will be the default. we will iterate through the others ( if they exist) later
                                    hand.animator.runtimeAnimatorController = hand.handInHandAnimOverride[0];
                                }
                                gun.transform.rotation = origin.transform.rotation;
                                hand.PickUpWeapon();
                                //same concept applies here, just for the gun now. This is in case the gun itself needs to have unique animations, ie the slide moving or levers being pulled etc
                                if (wep.gunAnimOverride.Length > 0)
                                {
                                    //store it in the gunanim script via the conneciton in the hand anim script
                                    hand.gunAnim.gunInHandAnimOverride = wep.gunAnimOverride;
                                    //apply the first in the stack as the default
                                    Debug.Log("Here is the current animOverriderState " + hand.gunAnim.animOverriderState + " it is being changed to " + mag.animOverriderState);
                                    if (mag.Ammo.Count <= 0)
                                    {
                                        hand.gunAnim.anim.runtimeAnimatorController = hand.gunAnim.gunInHandAnimOverride[1];
                                        hand.gunAnim.GetComponent<Animator>().SetBool("OutofAmmo", true);
                                    }
                                    else
                                    {
                                        hand.gunAnim.animOverriderState = mag.animOverriderState;
                                        hand.gunAnim.anim.runtimeAnimatorController = hand.gunAnim.gunInHandAnimOverride[mag.animOverriderState];
                                        //I am a bit confused on this ppart honestly, I dont think the hand animation states iterating is properly implemented yet. I will just leave it as it for now
                                        //hand.animator.runtimeAnimatorController = hand.handInHandAnimOverride[mag.animOverriderState];
                                    }

                                }
                                else if (mag.Ammo.Count <= 0)
                                {
                                    hand.gunAnim.GetComponent<Animator>().SetBool("OutofAmmo", true);
                                }
                                Destroy(hit.transform.gameObject);
                                //hand
                            }
                        }
                            
                    }
                    // if you are already holding something, drop it. 
                    else if (grab.isHolding)
                    {
                        Detach();
                        //clear the temps for next loop
                        prop = null;
                        propRB = null;
                        grab.throwingforce = grab.throwingTemp;
                    }
                }
                if(dropAction.WasPressedThisFrame()){
                    if(hand.holdingWeapon && !hand.firing && !hand.reloading){
                        hand.DropWeapon();
                        if(gun.GetComponent<AmmoManager>() != null){
                            TempAmmoValue = gun.GetComponent<AmmoManager>().Ammo;
                        }
                        Destroy(gun);
                        gun = Instantiate(hand.gunAnim.WorldModel, holdPoint.transform.position, origin.transform.rotation);
                        mag = gun.GetComponent<Magazine>();
                        mag.Ammo = TempAmmoValue;
                        
                        //does this gun have a hand animation overide?
                        if (gun.GetComponent<WeaponType>().animOverride.Length > 0)
                        {
                            //get hand animation override state and write it to the dropped weapon
                        }
                        //does this gun have a hand animation override?
                        if (gun.GetComponent<WeaponType>().gunAnimOverride.Length > 0)
                        {
                            Debug.Log("Writing overwriterState " + hand.gunAnim.animOverriderState + " To dropped weapon");
                            mag.animOverriderState = hand.gunAnim.animOverriderState;
                        }
                        
                        TempAmmoValue = null;
                        gun.GetComponent<Rigidbody>().AddForce(this.transform.forward, ForceMode.Impulse);
                        wallCollisionCheckBox.transform.localScale = new Vector3 (wallCollisionCheckBox.transform.localScale.x, wallCollisionCheckBox.transform.localScale.y, wallCollisionCheckBox.transform.localScale.z / hand.gunAnim.wallCollisionCheckSizeAdjust);
                        wallCollisionCheckBox.transform.localPosition = new Vector3 (wallCollisionCheckBox.transform.localPosition.x, wallCollisionCheckBox.transform.localPosition.y, wallCollisionCheckBox.transform.localPosition.z / hand.gunAnim.wallCollisionCheckPosAdjust);
                        hand.gunAnim = null;
                        hand.ammomanager = null;
                        hand.canShoot = false;
                        hand.canReload = false;
                        hand.animator.runtimeAnimatorController = baseOverride;
                    }
                }
                if(distanceGate){
                    DistanceCheck();
                }
            }
        }
    }
}