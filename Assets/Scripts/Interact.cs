//Author: Travis Parks
//Debugging: Travis Parks, Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script handles interacting with various objects, such as a button, a lever, a pickupable object, etc. essentially just pressing e on something
public class Interact : MonoBehaviour
{
    //Components
    Controls controls;
    Movement movement;
    Grab grab;
    HandAnim hand;
    [HideInInspector]
    public Transform prop;
    [HideInInspector]
    public Rigidbody propRB;
    [SerializeField]
    [Tooltip("where a grabbed prop attaches")]
    Transform dummy;
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
        controls = GameObject.Find("Data").GetComponentInChildren<Controls>();
        movement = transform.root.GetComponent<Movement>();
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
            grab.pickUp(dummy, prop, propRB, obj);
 
        }

    }
    // this just makes you drop whatever you are holding
    public void detach(){
	    grab.sizes = Grab.objectSizes.none;
	    colTog.clear();
        hand.setisHolding(false);
        if(prop.gameObject.tag == "ragdoll"){
            dummy.GetChild(5).SetParent(ragdollParent);
        }
        else{
            dummy.GetChild(5).SetParent(null);
        }
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
            if (Input.GetKeyDown(controls.keys["interact"]))
            {
	            // if you are not holding anything
                if (!grab.isHolding)
                {
                    RaycastHit hit;
                    //IF a raycast hits something
                    if (Physics.SphereCast(origin.transform.position, .2f, (dummy.position - origin.transform.position), out hit, distance, mask))
                    {
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
                        if (hit.transform.gameObject.GetComponent<Rigidbody>() != null && hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic == false && hit.transform.gameObject.GetComponent<Rigidbody>().mass <= grab.strength && !grab.justThrew)
                        {
                            //Pick it up
                            pickUp(prop.gameObject);
                        }
                        
	                    //Maybe revise this to just be a generic "Interactable"?
                        
	                    //IF the the thing you hit is a button
	                    /*
                        if (hit.transform.gameObject.GetComponent<buttonPush>() != null)
                        {
                            //Get the button object
                            buttonPush button = hit.transform.gameObject.GetComponent<buttonPush>();
                            if ((button.oneTime && button.anim.GetBool("onePush") == false && button.door.subGate == false) || (button.door != null && button.door.subGate == false) || !button.blocker)
                            {
                                button.press();
                            }
                            if ((button.oneTime && button.anim.GetBool("onePush") == false && button.belt.subGate == false) || (button.belt != null && button.belt.subGate == false) || !button.blocker)
                            {
                                button.press();
                            }
	                    }
	                    */
                   
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