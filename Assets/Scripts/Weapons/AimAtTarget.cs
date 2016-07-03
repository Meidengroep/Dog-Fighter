using UnityEngine;
using System.Collections;

public class AimAtTarget : MonoBehaviour 
{
    [HideInInspector]
    public ThirdPersonCrosshair Crosshair;

    public float MaxTurnAngle;

    public GunSwitcher GunSwitcher { get; set; }

    public bool HumanControlled
    {
        get { return this.humanControlled; }
        set
        {
            this.humanControlled = value;
            if (this.Crosshair != null)
                this.Crosshair.HumanControlled = value;
        }
    }

    public void UpdateFiringDirection(Vector3 direction)
    {
        this.Crosshair.ThreeDimensionalCrosshair = direction;

        this.UpdateFiringDirection();
    }

    private void UpdateFiringDirection()
    {
        transform.LookAt(Crosshair.ThreeDimensionalCrosshair);

        float angle = Quaternion.Angle(initialRotation, transform.localRotation);

        if (angle > MaxTurnAngle)
        {
            transform.localRotation = Quaternion.Slerp(initialRotation, transform.localRotation, MaxTurnAngle / angle);
        }
    }

    private Quaternion initialRotation;

	void Start () 
    {
        initialRotation = transform.localRotation;
	}
	
	void Update () 
    {
        if (!this.HumanControlled)
            return;

        if (Crosshair == null)
            return;

        this.UpdateFiringDirection();       

        if (!GlobalSettings.SinglePlayer)
        {
            this.playerShipSync.CrosshairPosition = Crosshair.ThreeDimensionalCrosshair;
        }
	}

    private bool humanControlled;
    private PlayerShipSync playerShipSync
    {
        get
        {
            if (this.playerShipSync_ == null)
                this.playerShipSync_ = this.GunSwitcher.GetComponent<PlayerShipSync>();
            return this.playerShipSync_;
        }
    }
    private PlayerShipSync playerShipSync_;
}
