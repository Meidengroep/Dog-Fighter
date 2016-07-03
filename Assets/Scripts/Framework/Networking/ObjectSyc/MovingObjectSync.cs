using UnityEngine;
using System.Collections;

public class MovingObjectSync : DestroyableObjectSync 
{
    //public bool SuppressVelocitySync { get; set; }

	protected override void Start ()
	{
		base.Start ();

		this.objectTransformer = this.GetComponent<ObjectTransformer>();
	}

    protected override void SyncFunction()
    {
        base.SyncFunction();

        if (!base.IsIDAssigned)
            throw new UnityException("ID not assigned.");

        if (Network.isServer)
        {
            Vector3 pos = this.transform.position;
            Vector3 orientation = this.transform.eulerAngles;

            if (pos != this.previousPos || orientation != this.previousOrientation)
            {
                ObjectRPC.ObjectPosition(base.Owner, base.GlobalID, pos, orientation);
                this.previousPos = pos;
                this.previousOrientation = orientation;
            }
        }

        if (base.IsOwner)
        {
            Vector3 translation = this.objectTransformer.Translation;
            Vector3 rotation = this.objectTransformer.Rotation;

            if (translation != this.previousTranslation || rotation != this.previousRotation)
            {
                if (Network.peerType == NetworkPeerType.Server)
                    ObjectRPC.ObjectVelocityServer(base.Owner, base.GlobalID, translation, rotation);
                else if (Network.peerType == NetworkPeerType.Client)    
                    ObjectRPC.ObjectVelocityClient(base.Owner, base.GlobalID, translation, rotation);

                this.previousTranslation = translation;
                this.previousRotation = rotation;
            }
        }
    }

	private Vector3 previousPos;
	private Vector3 previousOrientation;
	private Vector3 previousTranslation;
	private Vector3 previousRotation;

	private ObjectTransformer objectTransformer;

}
