﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBallistics : MonoBehaviour 
{
	public Transform barrelTrans;
	//Drags
	public Transform targetObj;
	public Transform gunObj;

	//The bullet's initial speed in m/s
	//Sniper rifle
	//public static float bulletSpeed = 850f;
	//Test
	public static float bulletSpeed = 20f;

	//The step size
	static float h;

	//For debugging
	private LineRenderer lineRenderer;

	void Awake()
	{
		//Can use a less precise h to speed up calculations
		//Or a more precise to get a more accurate result
		//But lower is not always better because of rounding errors
		h = Time.fixedDeltaTime * 1f;
        
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{
		RotateGun();

		//DrawTrajectoryPath();
	}
	//Rotate the gun and the turret
	void RotateGun()
	{
		//If we want to calculate the barrel should have to hit the target, we will get 0, 1, or 2 angles
		//Angle used if we want to simulate artillery, where the bullet hits the target from above
		float? highAngle = 0f;
		//Angle used if we want to simulate a rifle, where the bullet hits the target from the front
		float? lowAngle = 0f;

		CalculateAngleToHitTarget(out highAngle, out lowAngle);

		//We will here simulate artillery cannon where we fire in a trajetory so the shell lands from above the target
		//If you want to simulate a rifle, you just use lowAngle instead
        
		//If we are within range
		if (highAngle != null)
		{
			float angle = (float)highAngle;

			//Rotate the barrel
			//The equation we use assumes that if we are rotating the gun up from the
			//pointing "forward" position, the angle increase from 0, but our gun's angles
			//decreases from 360 degress when we are rotating up
			barrelTrans.localEulerAngles = new Vector3(360f - angle, 0f, 0f);

			//Rotate the gun turret towards the target
			transform.LookAt(targetObj.transform.position);
			transform.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		//Get the 2 angles
		//float? highAngle = 0f;
		//float? lowAngle = 0f;
	
		//CalculateAngleToHitTarget(highAngle, lowAngle);

		//Artillery
		//float angle = (float)lowAngle.GetValueOrDefault();
		//Debug.Log(angle);
		//Regular gun
		//float angle = (float)lowAngle;

		//If we are within range
		//	if (CalculateAngleToHitTarget(highAngle, lowAngle))
			//	{
			//Rotate the gun
			//The equation we use assumes that if we are rotating the gun up from the
			//pointing "forward" position, the angle increase from 0, but our gun's angles
			//decreases from 360 degress when we are rotating up
			//	gunObj.localEulerAngles = new Vector3(360f - angle, 0f, 0f);

			//Rotate the turret towards the target
			//	transform.LookAt(targetObj);
			//	transform.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
			//}
		}
	}
	//Which angle do we need to hit the target?
	//Returns 0, 1, or 2 angles depending on if we are within range
		void CalculateAngleToHitTarget(out float? theta1, out float? theta2)
	{
		//Initial speed
		float v = bulletSpeed;

		Vector3 targetVec = targetObj.position - gunObj.position;

		//Vertical distance
		float y = targetVec.y;

		//Reset y so we can get the horizontal distance x
		targetVec.y = 0f;

		//Horizontal distance
		float x = targetVec.magnitude;

		//Gravity
		float g = 9.81f;


		//Calculate the angles
	
		float vSqr = v * v;

		float underTheRoot = (vSqr * vSqr) - g * (g * x * x + 2 * y * vSqr);

		//Check if we are within range
		if (underTheRoot >= 0f)
		{
			float rightSide = Mathf.Sqrt(underTheRoot);

			float top1 = vSqr + rightSide;
			float top2 = vSqr - rightSide;

			float bottom = g * x;

			theta1 = Mathf.Atan2(top1, bottom) * Mathf.Rad2Deg;
			theta2 = Mathf.Atan2(top2, bottom) * Mathf.Rad2Deg;
		}
		else
		{
			theta1 = null;
			theta2 = null;
		}
	}
	//Display the trajectory path with a line renderer
	void DrawTrajectoryPath()
	{
		//How long did it take to hit the target?
		float timeToHitTarget = CalculateTimeToHitTarget();

		//How many segments we will have
		int maxIndex = Mathf.RoundToInt(timeToHitTarget / h);
		//lineRenderer.SetVertexCount(maxIndex);

		//Start values
		Vector3 currentVelocity = gunObj.transform.forward * bulletSpeed;
		Vector3 currentPosition = gunObj.transform.position;

		Vector3 newPosition = Vector3.zero;
		Vector3 newVelocity = Vector3.zero;

		//Build the trajectory line
		for (int index = 0; index < maxIndex; index++)
		{
			lineRenderer.SetPosition(index, currentPosition);

			//Calculate the new position of the bullet
			TutorialBallistics.CurrentIntegrationMethod(h, currentPosition, currentVelocity, out newPosition, out newVelocity);

			currentPosition = newPosition;
			currentVelocity = newVelocity;
		}
	}
	//How long did it take to reach the target (splash in artillery terms)?
	public float CalculateTimeToHitTarget()
	{
		//Init values
		Vector3 currentVelocity = gunObj.transform.forward * bulletSpeed;
		Vector3 currentPosition = gunObj.transform.position;

		Vector3 newPosition = Vector3.zero;
		Vector3 newVelocity = Vector3.zero;

		//The total time it will take before we hit the target
		float time = 0f;

		//Limit to 30 seconds to avoid infinite loop if we never reach the target
		for (time = 0f; time < 30f; time += h)
		{
			TutorialBallistics.CurrentIntegrationMethod(h, currentPosition, currentVelocity, out newPosition, out newVelocity);

			//If we are moving downwards and are below the target, then we have hit
			if (newPosition.y < currentPosition.y && newPosition.y < targetObj.position.y)
			{
				//Add 2 times to make sure we end up below the target when we display the path
				time += h * 2f;

				break;
			}

			currentPosition = newPosition;
			currentVelocity = newVelocity;
		}

		return time;
	}
	//Easier to change integration method once in this method
	public static void CurrentIntegrationMethod(
		float h,
		Vector3 currentPosition,
		Vector3 currentVelocity,
		out Vector3 newPosition,
		out Vector3 newVelocity)
	{
		//IntegrationMethods.EulerForward(h, currentPosition, currentVelocity, out newPosition, out newVelocity);
		//IntegrationMethods.Heuns(h, currentPosition, currentVelocity, out newPosition, out newVelocity);
		//IntegrationMethods.RungeKutta(h, currentPosition, currentVelocity, out newPosition, out newVelocity);
		IntegrationMethods.BackwardEuler(h, currentPosition, currentVelocity, out newPosition, out newVelocity);
	}
}
