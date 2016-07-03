using UnityEngine;
using System.Collections;

/// <summary>
/// Makes slight adjustments to the velocity direction every x amount of time.
/// </summary>
public class VelocityJitter : MonoBehaviour 
{
	public Rigidbody JitteringObject;
	public FlyControl FlyControl;
	public float Magnitude;
	public float ChangesPerSecond;

	private float accumulatedTime;
	private float timePerChange;

	void Start () 
	{
		accumulatedTime = 0;

		// Store the time between each change.
		timePerChange = 1f / ChangesPerSecond;
	}
	
	void Update () 
	{
		// Update the total time left to distribute changes in.
		accumulatedTime += Time.deltaTime;

		// While we can still fit a change in the total time left,
		// change the velocity direction.
		while (accumulatedTime >= timePerChange)
		{
			MakeChange();
			accumulatedTime -= timePerChange;
		}
	}

	void MakeChange()
	{
		// Calculate random angles around the x, y and z axis.
		float angle1 =  Random.Range(-Magnitude, Magnitude);
		float angle2 = Random.Range(-Magnitude, Magnitude);
		float angle3 = Random.Range(-Magnitude, Magnitude);

		// First rotate the velocity with the inverse of the objects current rotation.
		// This is done so we can the rotate the entire object, and then use the objects rotation on the velocity again
		// to obtain the new direction.
		// Hey, it works, dont question it :P
		FlyControl.VelocityDirection = Quaternion.Inverse(JitteringObject.transform.rotation) * FlyControl.VelocityDirection;
		JitteringObject.transform.Rotate(angle1, angle2, angle3, Space.Self);
		FlyControl.VelocityDirection = JitteringObject.transform.rotation * FlyControl.VelocityDirection;
	}
}
