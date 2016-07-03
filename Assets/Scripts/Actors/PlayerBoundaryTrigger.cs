using UnityEngine;
using System.Collections;

public class PlayerBoundaryTrigger : MonoBehaviour {
	//The time in sec. for which a player can be outside the boundary, before he explodes
	public float maxOutsideTime = 5;
	//The current time in sec. that the player has been outside
	//equal to 0 if he is currently inside
	private float curOutsideTime;
	private bool outside;
	
	//Keep a healthcontrol object to kill the player
	public HealthControl healthControl;
	
	// Use this for initialization
	void Start () {
		curOutsideTime = 0;
		//If the player starts outside,
		//  this script will not start counting.
		outside = false;
	}
	
	void Update () {
		//If outside, count the time
		if (outside) {
			curOutsideTime += Time.deltaTime;
			//Check for death
			if (curOutsideTime > maxOutsideTime)
            {
                ObjectSync objSync = this.GetComponent<ObjectSync>();
                ObjectRPC.KillObject(objSync.Owner, objSync.GlobalID);
                Debug.Log("Out of boundary call.");
                    //healthControl.Die();
                outside = false;
            }
		}
	}
	
	// Called when the player leaves a trigger collider
	void OnTriggerEnter (Collider other) {
		if (LayerMask.LayerToName(other.gameObject.layer) == "Boundary") {
			outside = false;
			curOutsideTime = 0;
		}
	}
	
	// Called when the player leaves a trigger collider
	void OnTriggerExit (Collider other) {
		//Start counting, if it was the boundary
		if (LayerMask.LayerToName(other.gameObject.layer) == "Boundary") {
			outside = true;
		}
	}
	
	public void OnGUI() {
		if (outside) {
			GUI.Label(new Rect(Screen.width / 2 - 75, 100, Screen.width / 2 + 75, 200),
			          "You have left the battlefield." + "\n" +
			          "Return within " + Mathf.Round(maxOutsideTime - curOutsideTime) + " seconds, or you will die!");
		}
	}
}
//""