using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Travis Parks
// this script toggles on and off the colliders that represent a held object depending on its size. 
public class heldItemColliderToggle : MonoBehaviour
{
    [SerializeField]
    Grab grab;
    GameObject smallCollider;
    GameObject medCollider;
    GameObject largeCollider;
    // Start is called before the first frame update
    void Start()
    {
        smallCollider = transform.GetChild(0).gameObject;
        medCollider = transform.GetChild(1).gameObject;
        largeCollider = transform.GetChild(2).gameObject;
    }

    public void clear(){
        smallCollider.gameObject.SetActive(false);
        medCollider.SetActive(false);
        largeCollider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(grab.isHolding){
            if(grab.sizes == Grab.objectSizes.large){
                largeCollider.SetActive(true);
            }
            if(grab.sizes == Grab.objectSizes.medium){
                medCollider.SetActive(true);
            }
            if(grab.sizes == Grab.objectSizes.small || grab.sizes == Grab.objectSizes.tiny){
                smallCollider.SetActive(true);
            }
        }
        else{
            smallCollider.gameObject.SetActive(false);
            medCollider.SetActive(false);
            largeCollider.SetActive(false);
        }
    }
}
