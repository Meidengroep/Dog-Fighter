using UnityEngine;
using System.Collections;

/* Generates a certain amount of asteroids within a bounded area.
 * The position, rotation and size of each astroid is randomized.
 * The area is donut shaped.
 */
public class AsteroidRingGenerator : AsteroidFieldGenerator {
	//The inherited maxOffset variable determines the fatness of the donut ring
	//The outerRadius variable determines the size of the entire donut
	public float outerRadius = 100;

	//Returns a random vector3 position within the specified donut shape.
	public override Vector3 GetRandomPosition() {
		float direction = Random.value * 2 * Mathf.PI;
		//Keep the z at 0, to make a ring instead of the outer layer of a sphere
		Vector3 directionVector = new Vector3(
			Mathf.Cos(direction),
			Mathf.Sin (direction),
			0
		);
		Vector3 pointOnRing = outerRadius * directionVector;
		//Add a spherical offset to get a donut instead of a ring
		Vector3 relativePosition = pointOnRing + maxOffset * Random.insideUnitSphere;
		//Dont forget to go from local to world coordinates
		return transform.TransformPoint(relativePosition);
	}

}
//""