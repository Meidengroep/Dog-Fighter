using UnityEngine;
using System.Collections;

public class CheckpointHandler : MonoBehaviour {


    // The motherships 
    public GameObject[] ships;

    void OnTriggerEnter(Collider Obj)
    {
        // Check if this is a drone
        if (Obj.gameObject.tag == "Npc")
        {
            DroneBehaviour s = Obj.gameObject.GetComponent<DroneBehaviour>();
            GameObject p = ships[0];
            if (TeamHelper.IsSameTeam(int.Parse(p.layer.ToString()), int.Parse(Obj.gameObject.layer.ToString())))
            {
                Transform newTarget = ships[1].transform.FindChild("DroneDestination");
                s.target = newTarget;
            }
            else
            {

                Transform newTarget = ships[0].transform.FindChild("DroneDestination");
                s.target = newTarget;
            }
        }
    }
 
}
