using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adpoted from https://www.habrador.com/tutorials/unity-realistic-bullets/

public class BulletData : MonoBehaviour
{
    //Data belonging to this bullet type
    //The initial speed [m/s]
    public float muzzleVelocity = 10f;
    //Mass [kg]
	public float mass = 0.2f;
    //Radius [m]
	public float radius = 0.05f;
    //Coefficients, which is a value you can't calculate - you have to simulate it in a wind tunnel
    //and they also depends on the speed, so we pick some average value
    //Drag coefficient (Tesla Model S has the drag coefficient 0.24)
	public float dragCoefficient = 0.5f;
    //Lift coefficient
	public float liftCoefficient = 0.5f;


    //External data (shouldn't maybe be here but is easier in this tutorial)
    //Wind speed [m/s]
    public Vector3 windSpeedVector = new Vector3(0f, 0f, 0f);
    //The density of the medium the bullet is travelling in, which in this case is air at 15 degrees [kg/m^3]
	public float densityOfMedium = 1.225f;
}
