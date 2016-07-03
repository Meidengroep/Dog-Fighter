using UnityEngine;
using System.Collections;

public class PlayerShipSync : MovingObjectSync
{
    public Vector3 CrosshairPosition
    {
        get { return this.crossHairPosition; }
        set
        {
            if (this.crossHairPosition != value)
            {
                this.crossHairPosition = value;
                this.positionChanged = true;
            }
        }
    }

    protected override void SyncFunction()
    {
        base.SyncFunction();

        if (!GlobalSettings.SinglePlayer)
        {
            if (this.positionChanged)
            {
                if (Network.peerType == NetworkPeerType.Client && base.IsOwner)
                    PlayerShipRPC.FiringDirection(this.Owner, this.CrosshairPosition);

                this.positionChanged = false;
            }
        }
    }

    private Vector3 crossHairPosition;
    private bool positionChanged;

    private GunSwitcher gunSwitcher
    {
        get
        {
            if (this.gunSwitcher_ == null)
                this.gunSwitcher_ = this.GetComponent<GunSwitcher>();
            return this.gunSwitcher_;
        }
    }
    private GunSwitcher gunSwitcher_;

}
