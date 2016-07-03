using UnityEngine;
using System.Collections;

public class MothershipOnCollideHandling : MonoBehaviour
{
    public MothershipHealthControl HealthControl;

    void OnCollisionEnter(Collision info)
    {
        //Debug.Log("Collision detected: " + info.collider.gameObject.tag);

        switch (info.collider.gameObject.tag)
        {
            case "Team1Projectile":
                HandleProjectileCollision(info);
                break;
            case "Team2Projectile":
                HandleProjectileCollision(info);
                break;
            case "Mothership":
                break;
            case "Obstacle":
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
