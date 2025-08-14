using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectSize : MonoBehaviour
{
    [SerializeField]
    public string objectName;
    public enum objectSizes{tiny, small, medium, large};
    public objectSizes sizes;
    [HideInInspector]
	public bool portalWarp;
	public bool isHeld;

}
