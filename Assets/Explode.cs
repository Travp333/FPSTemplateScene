using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Travis
public class Explode : MonoBehaviour
{
    Rigidbody body; 
    [SerializeField]
    float radius;
    [SerializeField]
    float power; 
    [SerializeField]
    float upModifier;
    float damage;
    [SerializeField]
    Vector3 torque;
    void Start()
    {

        body = GetComponent<Rigidbody>();
        Vector3 explosionPos = transform.position;
        	Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        	foreach (Collider hit in colliders)
        	{
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                    if (hit.transform.IsChildOf(this.transform)){
                        if (rb != null)
                        rb.AddExplosionForce(power, explosionPos, radius, upModifier);
                        rb.AddTorque(torque, ForceMode.Impulse);
                    }
            }
    }
    // Update is called once per frame
}
