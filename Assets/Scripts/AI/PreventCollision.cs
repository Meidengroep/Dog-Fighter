using UnityEngine;
using System.Collections;

public class PreventCollision : MonoBehaviour {

    Transform drone;

    public void setActor(Transform actor)
    {
        drone = actor;
    }
   
    Vector3 shootRay(Vector3 origin, Vector3 direction, float distance)
    {
        RaycastHit hitInfo;
        //Debug.DrawLine(origin, direction + origin, Color.green);


        //int layerMask = 1 << 8;
        int layerMask = 1 << 8;
        layerMask |= (1 << 9);
        layerMask |= (1 << 10);
        layerMask |= (1 << 11);
        layerMask |= (1 << 12);
        layerMask |= (1 << 13);
        layerMask |= (1 << 17);
        layerMask = ~layerMask;
        if (Physics.Raycast(origin, direction, out hitInfo, distance, layerMask))
        {
            if (hitInfo.transform != drone)
            {
                //Debug.DrawLine(origin, hitInfo.point, Color.red);
                //Debug.DrawLine(hitInfo.point, hitInfo.normal, Color.blue);
                return hitInfo.normal * 20;
            }
        }
        return Vector3.zero;
         
    }

    // Prevents collision by finding new heading direction
    public Vector3 RecalculatePath(Transform target)
    {
        // Direction to target
        Vector3 dir = (target.position - drone.position).normalized;

        // Position of drone
        Vector3 origin = drone.position;

        // direction drone is flying
        Vector3 direction = drone.forward;

        // TODO Finetune length of ray
        float distance = 15;

        // Construct a temporary vector for different orientation
        GameObject temp = new GameObject();
        Vector3 t = origin;
        temp.transform.position = t;
        temp.transform.localEulerAngles = drone.localEulerAngles;
        
        // Shoot 5 different rays
        // One from the center of the drone to the fron
        // two from the sides
        // In case the ray hits a rigid body, change the course of the drone
        dir += shootRay(origin, direction, distance);
    
        // Shoot ray to the right
        temp.transform.localEulerAngles = new Vector3(0, 45 + drone.localEulerAngles.y, 0);
        dir += shootRay(origin, temp.transform.forward, distance);

        // Shoot ray to the left
        temp.transform.localEulerAngles = new Vector3(0, -45 + drone.localEulerAngles.y, 0);
        dir += shootRay(origin, temp.transform.forward, distance);

        // Shoot ray up
        temp.transform.localEulerAngles = new Vector3(40 + drone.localEulerAngles.x, drone.localEulerAngles.y, 0);
        dir += shootRay(origin, temp.transform.forward, distance);

        // Shoot ray down
        temp.transform.localEulerAngles = new Vector3(-40 + drone.localEulerAngles.x, drone.localEulerAngles.y, 0);
        dir += shootRay(origin, temp.transform.forward, distance);

        // Destroy the temporary created object
        Destroy(temp);

        // Return final new direction
        return dir;
    }


}
