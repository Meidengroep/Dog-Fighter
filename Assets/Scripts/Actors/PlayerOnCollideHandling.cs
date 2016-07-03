using UnityEngine;
using System.Collections;

public class PlayerOnCollideHandling : MonoBehaviour 
{
	public PlayerHealthControl HealthControl;

	void OnCollisionEnter(Collision info)
	{
        //Debug.Log("Player collision!");

		switch(info.collider.gameObject.tag)
		{
			case "Team1Projectile":
				HandleProjectileCollision(info);
				break;
			case "Team2Projectile":
				HandleProjectileCollision(info);
				break;
			case "Mothership":
				HealthControl.TakeDamage(HealthControl.MaxHealth, HealthControl.MaxShields);
				break;
			case "Obstacle": 
				HealthControl.TakeDamage(HealthControl.MaxHealth, HealthControl.MaxShields);
				break;
		}
	}

	private void HandleProjectileCollision(Collision info)
	{
		//if (Network.peerType == NetworkPeerType.Server || GlobalSettings.SinglePlayer)
		{
            //Debug.Log("Collision detected.");
            //Debug.Log(info.gameObject.name);

			if (!TeamHelper.IsSameTeam(info.collider.gameObject.tag, gameObject.layer))
				HealthControl.TakeDamage(info.gameObject.GetComponent<ProjectileController>().Damage, info.contacts[0].point);
		}

		Destroy(info.gameObject);
	}
}
