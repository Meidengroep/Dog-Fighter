using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Allows for switching between a list of guns using specified keyboard keys.
/// </summary>
public class GunSwitcher : MonoBehaviour 
{
	public GameObject[] Guns;
	public Transform[] Hardpoints;

	public bool HumanControlledGuns;
	public bool ShowCrosshair;
	public KeyCode NextGun;
	public KeyCode PreviousGun;

    public ThirdPersonCrosshair Crosshair;

	public GameObject[] CurrentGuns;
	private int currentGunIndex;
    //private IList<int> previousLayers;

    //private void Awake()
    //{
    //    this.previousLayers = new List<int>(2);
    //}

	void Start()
	{
		NetworkControl networkControl = null;

		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			networkControl = GameObject.Find("NetworkControl").GetComponent<NetworkControl>();
			if (networkControl.ThisPlayer == this.GetComponent<ObjectSync>().Owner)
				this.HumanControlledGuns = true;
			else 
				this.HumanControlledGuns = false;
		}

		currentGunIndex = 0;
		CurrentGuns = new GameObject[Hardpoints.Length];
		assignNewGuns(currentGunIndex);

        //foreach (Transform hardpoint in this.Hardpoints)
        //    this.previousLayers.Add(-1);
	}

    public void AssignCrossHairPosition(Vector3 crossHairPosition)
    {
        foreach (GameObject gun in this.CurrentGuns)
        {
            AimAtTarget aimAtTarget = gun.GetComponent<AimAtTarget>();
            if (aimAtTarget != null)
            {
                aimAtTarget.UpdateFiringDirection(crossHairPosition);
            }
        }
    }

    public void LayerChanged()
    {
        for (int i = 0; i < CurrentGuns.Length; i++)
            CurrentGuns[i].layer = gameObject.layer;
    }

    void Update() 
	{
        for (int i = 0; i < this.Hardpoints.Length; i++)
        {
            if (this.CurrentGuns[i].layer != this.Hardpoints[i].gameObject.layer)
            {
                this.CurrentGuns[i].layer = this.Hardpoints[i].gameObject.layer;
                Debug.Log("Found!");
            }
        }

		// Check for input and swap accordingly.
		if(Input.GetKeyDown(NextGun))
		{
			currentGunIndex = (currentGunIndex + 1) % Guns.Length;
			assignNewGuns(currentGunIndex);
		}

		if(Input.GetKeyDown(PreviousGun))
		{
			currentGunIndex = Mathf.Max(0, currentGunIndex - 1);
			assignNewGuns(currentGunIndex);
		}
	}

	private void assignNewGuns(int index)
	{
		// Assign all gun slots to the newly selected gun.
		for (int i = 0; i < CurrentGuns.Length; i++)
			assignNewGun(Hardpoints[i], index, i);
	}

    private void assignNewGun(Transform hardpoint, int newGunIndex, int gunToReplaceIndex)
    {
        // Store some properties of the current gun.
        GameObject gunToReplace = CurrentGuns[gunToReplaceIndex];

        Destroy(gunToReplace);

        // Create the new gun, and assign the stored properties to it.
        GameObject newGun = (GameObject)Instantiate(Guns[newGunIndex], hardpoint.position, hardpoint.rotation);

        // Make sure the new gun IS scaled with the hardpoint
        Vector3 initialScale = newGun.transform.localScale;
        newGun.transform.parent = hardpoint;
        newGun.transform.localScale = initialScale;

        newGun.layer = hardpoint.gameObject.layer;

        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            Player owner = this.GetComponent<ObjectSync>().Owner;
            Shooter shooter = newGun.GetComponent<Shooter>();
            shooter.Owner = owner;
        }

        AimAtTarget aimAtTarget = newGun.GetComponent<AimAtTarget>();
        aimAtTarget.GunSwitcher = this;

        newGun.GetComponent<Shooter>().HumanControlled = HumanControlledGuns;
        aimAtTarget.HumanControlled = HumanControlledGuns;

        //if (HumanControlledGuns)
        aimAtTarget.Crosshair = Crosshair;

        CurrentGuns[gunToReplaceIndex] = newGun;
    }
}