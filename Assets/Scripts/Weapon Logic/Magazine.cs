using UnityEngine;

public class Magazine : MonoBehaviour
{
    [SerializeField]
    public int gunAnimOverriderState = 1;
    [SerializeField]
    public int handAnimOverriderState = 1;
    //Tracks How much ammo is in the gun. Defaults to full, is updated when dropped 
    [SerializeField]
    public int ammo;
}
