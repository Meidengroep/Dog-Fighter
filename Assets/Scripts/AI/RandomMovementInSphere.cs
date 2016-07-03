using UnityEngine;
using System.Collections;

/// <summary>
/// Lets an object move randomly in a sphere.
/// </summary>
public class RandomMovementInSphere : MonoBehaviour 
{
	public Transform Character;
	public Transform SphereCenter;
	public int SphereRadius;
	public float MovementSpeed;

	private Vector3 CurrentTarget;

	void Start () 
	{
		// Pick the first movement target.
		PickNewTarget();
	}
	
	void Update () 
	{
		// Check in which direction we are moving.
		Vector3 direction = CurrentTarget - Character.position;

		// Calculate the speed at which to move.
		float speed = MovementSpeed * Time.deltaTime;

		// Check if we are close enough so that we would arrive at the target in this timestep.
		float distanceToTarget = direction.magnitude;
		if (distanceToTarget <= MovementSpeed || distanceToTarget == 0)
		{
			// Find a new target to move to.
			PickNewTarget();
		}

		// Move the object.
		Character.Translate(direction.normalized * speed);
	}

	private void PickNewTarget()
	{
		// Pick a random direction from the center of the sphere.
		Vector3 directionFromCenter = Random.insideUnitSphere;

		// Pick a random distance from the center of the sphere.
		float randomRadius = Random.Range(0, SphereRadius);

		// Create a new target.
		CurrentTarget = SphereCenter.position + randomRadius * directionFromCenter.normalized;
	}
}
