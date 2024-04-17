//Author: Travis
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script causes a game obejct to despawn after a set range of time
public class Die : MonoBehaviour
{
    [SerializeField]
    float lifeMin = 5;
    [SerializeField]
    float lifeMax = 10;

    void Start()
    {
        Destroy(this.gameObject, Random.Range(lifeMin, lifeMax));
    }

}
