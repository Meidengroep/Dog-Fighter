using UnityEngine;
using System.Collections;

public class DestroyableObjectSync : ObjectSync 
{
	public void RequestHealthSync()
	{
		if (GlobalSettings.SinglePlayer)
			return;

		if (Network.peerType != NetworkPeerType.Server)
			return;

        //Debug.Log("Health sync requested.");

		this.SyncHealth = true;

	}

	protected bool SyncHealth { get; private set; }

	protected override void SyncFunction ()
	{
		base.SyncFunction ();

		if (Network.peerType == NetworkPeerType.Server)
		{
			if (this.SyncHealth)
			{
                //Debug.Log("Syncing health: " + this.Type);
                //Debug.Log(base.Owner == null);
                //Debug.Log(base.GlobalID == null);

				float health = this.healthControl.CurrentHealth;
				float shield = this.healthControl.CurrentShields;

				ObjectRPC.SetObjectHealth(base.Owner, base.GlobalID, health, shield);

                this.SyncHealth = false;
			}
		}
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

		this.healthControl = this.GetComponent<HealthControl>();	
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
	
	}

	private HealthControl healthControl;
	
}
