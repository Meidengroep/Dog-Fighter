using UnityEngine;
using System.Collections;
using System;

public class ObjectSync : NetworkObject 
{
    public ObjectSyncType Type { get; set; }

	public Player Owner { get; private set; }
	public int GlobalID { get; private set; }
	public bool IsIDAssigned { get; private set; }
    public bool IsDisposed { get; private set; }

    public bool IsOwner
    {
        get
        {
            if (GlobalSettings.SinglePlayer)
                return true;

            return base.NetworkControl.ThisPlayer.ID == this.Owner.ID;
        }
    }

	public void AssignID(Player owner, int globalID)
	{
		if (!this.IsIDAssigned)
		{
            if (owner != null)
                this.Owner = owner;
            else
                throw new UnityException("Given owner may not be null.");

            if (globalID != null)
			    this.GlobalID = globalID;
            else
                throw new UnityException("Given globalID may not be null.");

			this.IsIDAssigned = true;
		}
		else
			throw new UnityException("IDs have already been assigned.");
	}

    //public virtual void NetworkDestroy()
    //{
    //    base.ObjectTables.RemovePlayerObject(this.Owner, this.GlobalID);
    //}

	public override void Dispose()
	{
        if (this.IsDisposed)
            throw new UnityException("Object has already been disposed.");

        //Debug.Log(base.ObjectTables == null);
        base.ObjectTables.RemovePlayerObject(this.Owner, this.GlobalID);

		base.NetworkControl.SyncTimeEvent -= this.SyncFunction;

        //Debug.Log("ObjectSync disposed: ID = " + this.GlobalID);

		if (this.IsIDAssigned)
		{
			//this.Owner = null;

			if (Network.isServer)
				base.GUIDGenerator.RecycleID(this.GlobalID);
		}

        //Debug.Log("Diposed");

        this.IsDisposed = true;

		base.Dispose();
	}

	protected virtual void SyncFunction()
	{
	}

	// Use this for initialization
	protected override void Start () {

		base.Start();

        if (!GlobalSettings.SinglePlayer)
        {
            base.NetworkControl.SyncTimeEvent += this.SyncFunction;
            //Debug.Log("Syncfunction registered.");
        }
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}
}
