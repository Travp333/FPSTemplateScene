using UnityEngine;
using System.Collections.Generic;
// a FILO stack that represents ammo loaded into a magazine. This should ideally store ammo loaded into the magazine in the order it was loaded in,
// then fire the bullets in a first in last out order, meaning the most recently loaded bullet will fire first, then count down. 
public class AmmoList : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> Ammo = new List<GameObject>();

}
