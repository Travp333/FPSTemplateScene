using UnityEngine;

public class Magazine : MonoBehaviour
{
    [SerializeField]
    public int animOverriderState = 1;
    //Tracks How much ammo is in the gun. Defaults to full, is updated when dropped 
    [SerializeField]
    public int ammo;
}
