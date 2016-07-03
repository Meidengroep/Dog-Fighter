using UnityEngine;
using System.Collections;

public class PlayerHealthControl : HealthControl 
{
	public PlayerRespawner RespawnPoint;
	public float RespawnDelay;

	public GameObject ExplosionGraphic;
	public AudioClip ExplosionSound;

    protected override void Initialize()
    {
        if (!GlobalSettings.SinglePlayer)
        {
            PlayerRespawner[] respawnpoints = FindObjectsOfType<PlayerRespawner>();
            foreach (PlayerRespawner spawner in respawnpoints)
                if (spawner.gameObject.layer == gameObject.layer)
                {
                    RespawnPoint = spawner;
                    break;
                }
        }
        else
            this.RespawnPoint = GameObject.Find("Spawnpoint_Player1").GetComponent<PlayerRespawner>();
    }

	public override void Die()
	{
        if (Network.peerType != NetworkPeerType.Server)
        {
            Instantiate(ExplosionGraphic, gameObject.transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(ExplosionSound, gameObject.transform.position);
        }

		RespawnPoint.DisableAndWaitForSpawn(RespawnDelay);
	}
}
