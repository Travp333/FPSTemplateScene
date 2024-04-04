//Author: Travis Parks
//Debugging: Travis Parks, Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
//this script handles interacting with various objects, such as a button, a lever, a pickupable object, etc. essentially just pressing e on something
public class Interact : MonoBehaviour
{
	public InputAction interactAction;
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

    // Start is called before the first frame update
    void Start()
	{
        //Assign components
		movement = transform.root.GetComponent<Movement>();
		interactAction = movement.GetComponent<PlayerInput>().currentActionMap.FindAction("Interact");
        grab = GetComponent<Grab>();
        hand = GetComponent<HandAnim>();
	    colTog = GetComponentInChildren<heldItemColliderToggle>();
    }

    List<Transform> GetAllChilds(Transform _t)
    {
        List<Transform> ts = new List<Transform>();
 
        foreach (Transform t in _t)
        {
            ts.Add(t);
            if (t.childCount > 0)
                ts.AddRange(GetAllChilds(t));
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
    public void detach(){
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
        //IF not paused
        if (!FindObjectOfType<PauseMenu>().isPaused) {
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
                            if(FindObjectOfType<DialogueManager>().dialogueBox.activeInHierarchy == false){
                                hit.transform.parent.gameObject.GetComponent<NpcDialogue>().Begin();
                            }
                            else{
                                FindObjectOfType<DialogueManager>().DisplayNextSentence();
                            }
                        }
	                    //IF the thing you hit has a rigidbody that is light enough for the player to hold
                        else if (hit.transform.gameObject.GetComponent<WeaponType>() == null && hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic == false && hit.transform.gameObject.GetComponent<Rigidbody>().mass <= grab.strength && !grab.justThrew)
                        {
                        	//Debug.Log("HIT!!");
                            //Pick it up
                            pickUp(prop.gameObject);
                        }
                        
	                    //Maybe revise this to just be a generic "Interactable"?
                        
	                    //IF the the thing you hit is a button
	                    
		                else if (hit.transform.gameObject.GetComponent<buttonPush>() != null)
                        {
                            //Get the button object
                            buttonPush button = hit.transform.gameObject.GetComponent<buttonPush>();
	                        if (!button.blocker)
                            {
                                button.press();
                            }
                        }
		                else if(hit.transform.gameObject.GetComponent<WeaponType>() != null){
		                	WeaponType wep = hit.transform.gameObject.GetComponent<WeaponType>();
		                	hand.PickUpWeapon();
		                	gun = Instantiate(wep.worldModel, GunGrabPoint.transform.position, origin.transform.rotation);
		                	gun.transform.parent = GunGrabPoint.transform;
		                	hand.gunAnim = gun.GetComponent<GunAnim>();
		                	Destroy(hit.transform.gameObject);
		                	//hand
		                }
	                    
                   
                    }
                }
                // if you are already holding something, drop it. 
                else if (grab.isHolding)
                {
                    detach();
                    //clear the temps for next loop
                    prop = null;
                    propRB = null;
                    grab.throwingforce = grab.throwingTemp;
                }
            }
        }
    }
}