using UnityEngine;
using System.Collections;

/*****************************************
See http://stackoverflow.com/questions/4749951/shoot-projectile-straight-trajectory-at-moving-target-in-3-dimensions 
for explanation by a smart person.
*****************************************/

public static class PredictPosition 
{
	public static Vector3 Predict(Vector3 targetPosition, Vector3 targetVelocity, Vector3 gunPosition, float projectileSpeed)
	{
		float a = (targetVelocity.x * targetVelocity.x) 
				+ (targetVelocity.y * targetVelocity.y) 
				+ (targetVelocity.z * targetVelocity.z)
				- projectileSpeed * projectileSpeed;

		float b = 2 
				* ((targetVelocity.x * (targetPosition.x - gunPosition.x))
				+ (targetVelocity.y * (targetPosition.y - gunPosition.y))
				+ (targetVelocity.z * (targetPosition.z - gunPosition.z)));

		float c = (targetPosition.x - gunPosition.x) * (targetPosition.x - gunPosition.x)
				+ (targetPosition.y - gunPosition.y) * (targetPosition.y - gunPosition.y)
				+ (targetPosition.z - gunPosition.z) * (targetPosition.z - gunPosition.z);

		if (b*b < 4*a*c)
			throw new UnityException("Prediction error, no real solution");

		float p = -b / (2*a);
		float q = Mathf.Sqrt(b*b - 4*a*c) / (2*a);

		float solution1 = p - q;
		float solution2 = p + q;

		if (solution1 < 0 && solution2 < 0)
			throw new UnityException("Prediction error, no positive solution");

		float finalTime = PickSmallestPositive(solution1, solution2);

		return targetPosition + finalTime * targetVelocity;
	}

	/// <summary>
	/// This method assumes that at least one of the two parameters is positive.	
	/// </summary>
	private static float PickSmallestPositive(float f1, float f2)
	{
		if (f1 < f2)
		{
			if(f1 >= 0)
				return f1;
			else return f2;
		}

		if (f2 < f1)
		{
			if (f2 >= 0)
				return f2;
			else return f1;
		}

		// f1 == f2, so return any
		return f1;
	}
}
