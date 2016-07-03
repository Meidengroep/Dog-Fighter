using UnityEngine;
using System.Collections;

public class DroneTriggerBehaviour : MonoBehaviour 
{
    public DroneBehaviour DroneBehaviour;

	void Start () 
    {
        SphereCollider c = this.gameObject.AddComponent<SphereCollider>();
        //followRadius;
        c.radius = 500; 
        c.isTrigger = true;

	}

    void OnTriggerEnter(Collider obj)
    {
        DroneBehaviour.TriggerEnter(obj);
    }

    void OnTriggerStay(Collider obj)
    {
        DroneBehaviour.TriggerStay(obj);
    }

    void OnTriggerLeave(Collider obj)
    {
        DroneBehaviour.TriggerLeave(obj);
    }
}
