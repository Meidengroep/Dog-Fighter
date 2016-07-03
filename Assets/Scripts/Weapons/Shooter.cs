using UnityEngine;
using System.Collections;

/// <summary>
/// Shoots a certain prefab RigidBody from one or more positions with a certain velocity.
/// </summary>
public class Shooter : MonoBehaviour 
{
	public Rigidbody Projectile;
	public Transform[] ShotPositions;
	public float ReloadDelay;
	public bool HumanControlled;

	public Player Owner { get; set; }
	public bool KeepFiring { get; set; }

	public float ProjectileSpeed { get; private set; }

	private float reloadTimer;
	private int projectileLayerMask; 
	private string projectileTag;
    private int previousObjLayer;

	void Start()
	{
		reloadTimer = 0;
		ProjectileSpeed = Projectile.GetComponent<ProjectileController>().FlyControl.DesiredSpeed;
        this.previousObjLayer = -1;
		//DetermineProjectileLayer();
	}

	private void DetermineProjectileLayer()
	{
		projectileTag = TeamHelper.LayerToProjectileTag(gameObject.layer);
		projectileLayerMask = LayerMask.NameToLayer(projectileTag);
        //Debug.Log("Determining layers.");
        //Debug.Log(gameObject.name);
        //Debug.Log("Player ship layer: " + gameObject.layer);
        //Debug.Log("Previous ship layer: " + previousObjLayer);
        //Debug.Log("Output layer: " + projectileLayerMask);
        this.previousObjLayer = gameObject.layer;
	}

	public void Shoot()
	{
		// Fire projectiles
		if (reloadTimer <= 0)
		{
			// Spawn a new projectile at each position.
			foreach (Transform t in ShotPositions)
			{
				SpawnProjectile(t);
			}
				
			reloadTimer = ReloadDelay;
		}
	}

	void Update () 
	{		
		if (reloadTimer > 0)
			reloadTimer -= Time.deltaTime;

        if (HumanControlled)
        {
            if (GlobalSettings.HasFocus && Input.GetButton("Fire1"))
			{
				if (!this.KeepFiring && Network.peerType != NetworkPeerType.Disconnected)
					PlayerShipRPC.FireWeapon(this.Owner, true);

				this.KeepFiring = true;

				Shoot();
			}
			else
			{
				if (this.KeepFiring && Network.peerType != NetworkPeerType.Disconnected)
					PlayerShipRPC.FireWeapon(this.Owner, false);

				this.KeepFiring = false;					
			}
			
        }
		else if (this.KeepFiring)
		{
			Shoot ();
		}
	}

	private void SpawnProjectile(Transform shotPosition)
	{
        if (this.previousObjLayer != this.gameObject.layer)
            this.DetermineProjectileLayer();

		// Create an instance of the prefab projectile.
		Rigidbody shot = Instantiate(Projectile) as Rigidbody;

		// Transform the new projectile to align it properly.
		shot.transform.Rotate(shotPosition.eulerAngles, Space.World);
		shot.transform.position = shotPosition.position;
		// Scale the projectile size relative to the gun size
		shot.transform.localScale = Vector3.Scale(shot.transform.localScale, transform.lossyScale);

		shot.gameObject.layer = projectileLayerMask;
		shot.gameObject.tag = projectileTag;

        //Debug.Log("Assigning stuff to projectile.");
        //Debug.Log("Layer: " + projectileLayerMask);
        //Debug.Log("Tag: " + projectileTag);

		// Fire it away by giving it a velocity.
		ProjectileController pController = shot.GetComponent<ProjectileController>();
		pController.SetVelocityDirection(shotPosition.forward);
	}
}
