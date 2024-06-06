using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

[System.Serializable]
public struct Heat {
    public int heat;
    public Vector3 recoilOffset;
}
 
 
public class RecoilManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("List of Heat values and offsets, to determine how much a shot is offset depending on the heat")]
    public Heat[] weaponHeatList;
    [SerializeField]
    [Tooltip("Measure of how long you've been firing a weapon, affects recoil")]
    public int weaponHeat;
    [SerializeField]
    [Tooltip("the point at which the weapons recoil stops changing and flattens out")]
    int maxHeat;
    [SerializeField]
    [Tooltip("How long the weapon will wait before beginning to decay its heat")]
    float heatCooldownWait;
    [Tooltip("keeps track of time to be compared to heatCooldownWait")]
    float heatCooldownWaitTimer;
    [SerializeField]
    [Tooltip("The rate at which the heat decays, ie one decay per .5 seconds")]
    float heatCooldown;
    [Tooltip("keeps track of time to be compared to heatCooldown")]
    float heatCooldownTimer;
    public void DetermineHeat(){
        if(weaponHeat < maxHeat){
            weaponHeat++;
            heatCooldownTimer = 0f;
            heatCooldownWaitTimer = 0f;
        }
        else{
            heatCooldownTimer = 0f;
            heatCooldownWaitTimer = 0f;
        }
    }
    public Vector3 ShiftVector(Vector3 forwardVector){
        
        forwardVector = transform.localToWorldMatrix * (new Vector3(weaponHeatList[weaponHeat].recoilOffset.x, weaponHeatList[weaponHeat].recoilOffset.y, 0));
        return forwardVector;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponHeat != 0){
            if(weaponHeat > maxHeat){
                weaponHeat = maxHeat;
            }
            if(heatCooldownWaitTimer < heatCooldownWait){
                heatCooldownWaitTimer += Time.deltaTime;
                //Debug.Log("Waiting to start decaying...");
            }
            else if (heatCooldownTimer < heatCooldown){
                heatCooldownTimer += Time.deltaTime;
            }
            else{
                //Debug.Log("Decaying!");
                weaponHeat--;
                heatCooldownTimer = 0;
            }
            if(weaponHeat == 0){
                //Debug.Log("Reset Back to Zero!");
                heatCooldownTimer = 0;
                heatCooldownWaitTimer = 0;
            }
        }
    }
}
