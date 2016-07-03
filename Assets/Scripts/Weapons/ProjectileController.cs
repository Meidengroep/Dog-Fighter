using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour 
{
	public float Damage;
	public FlyControl FlyControl;

	public void SetVelocityDirection(Vector3 velocityDirection)
	{
		FlyControl.VelocityDirection = velocityDirection;
	}
}
