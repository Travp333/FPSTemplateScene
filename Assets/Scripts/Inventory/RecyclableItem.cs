using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclableItem : MonoBehaviour
{
	[SerializeField]
	[Tooltip("What does this item recycle into? Match the order with below, ie recyclesInto[0] will pull from amount[0])")]
	public Item[] recyclesInto;
	[SerializeField]
	[Tooltip("How many of that item is created (Match the order with above, ie recyclesInto[0] will pull from amount[0])")]
	public int[] amount;
	
}
