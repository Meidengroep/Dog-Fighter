using UnityEngine;
using System.Collections;

public class LookAtPredictedPos : MonoBehaviour 
{
	public ShipControl ShipControl;
	public Transform ShipPosition;
	public Transform GunPosition;
	public Shooter Shooter;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		Vector3 targetPos = PredictPosition.Predict(ShipPosition.position
		                                            , ShipControl.CurrentSpeed * ShipPosition.forward
		                                            , GunPosition.position
		                                            , Shooter.ProjectileSpeed);
		GunPosition.LookAt(targetPos);
		Shooter.Shoot();
	}
}
