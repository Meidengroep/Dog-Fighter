using UnityEngine;
using System.Collections;

public class DroneBehaviour : MonoBehaviour
{
    public Transform target;
    private Transform prevTarget;
    private Transform spawnPosition;

    public ObjectTransformer transformer;

    public float speed;
    public float followRadius;
    public float shootRadius;
    public DroneTriggerBehaviour TriggerBehaviour;

    public Transform[] gun;
    Shooter gunL;
    private Shooter[] gunScripts;

    public enum Behaviours { Patrol, Chase, Defend, GoTo }
    public Behaviours currentState;
    Behaviours prevState;


    PreventCollision pc;
    double desiredDistance;

    // Ugly bool to see if we already have set the Shooter array
    bool first;

    void Start()
    {
        // Radius in which drone follows the player for attacking
        desiredDistance = 100;
        currentState = Behaviours.GoTo;

        // initialize gunScripts array
        gunScripts = new Shooter[gun.Length];
        first = true;

        // Add the prevent collision code 
        pc = this.gameObject.AddComponent<PreventCollision>();
        pc.setActor(this.transform);

        // Spawn positon is transform at creation
        spawnPosition = this.transform;

        ///
        /// Network Code
        ///

        this.objectSync = this.GetComponent<ObjectSync>();

        ///
        /// End Network Code
        ///
    }

    void Update()
    {
        if (Network.peerType == NetworkPeerType.Server || GlobalSettings.SinglePlayer)
        {
            // Retrieve gun scripts for shooting purposes
            // TODO: replace to main when possible
            MoveDrone();
            Shoot();
        }
        else
            this.clientBehaviourUpdate();

    }

    void GetGunScript()
    {
        for (int i = 0; i < gun.Length; i++)
        {
            foreach (Transform child in gun[i])
            {
                gunScripts[i] = child.GetComponent<Shooter>();
            }
        }

    }

    // Makes the drone shoot
    void Shoot()
    {
        ///////////////
        // ROADBLOCK //
        ///////////////
        if (target == null || target.transform == null)
        {
            TriggerLeave(null);
        }
        if (target == null || target.transform == null)
        {
            return;
        }
        bool startedShooting = false;
        // In case we are in shoot radius, shoot shoot shoot.
        if ((transform.position - target.transform.position).magnitude < shootRadius)
        {
            startedShooting = true;

            if (first)
            {
                GetGunScript();
                first = false;
            }

            foreach (Shooter s in gunScripts)
            {
                // Do not yet shoot
                s.Shoot();
            }
        }
        ///
        /// Network Code
        ///
        if (Network.peerType == NetworkPeerType.Server)
        {
            if (startedShooting && !this.KeepShooting)
            {
                this.KeepShooting = true;
                ObjectRPC.DroneShoot(this.objectSync.Owner, this.objectSync.GlobalID, true);
            }
            else if (!startedShooting && this.KeepShooting)
            {
                this.KeepShooting = false;
                ObjectRPC.DroneShoot(this.objectSync.Owner, this.objectSync.GlobalID, false);
            }
        }
        ///
        /// End Network Code
        ///

    }

    Transform temp;
    // Does the actual moving of the drone
    void MoveDrone()
    {
        Quaternion rot = Quaternion.identity;
        Vector3 direction;

        // In case we are in shooting range keep looking at the opponent

        if (currentState == Behaviours.Chase && InShootingRange())
        {
            rot = Quaternion.LookRotation(target.position - transform.position);
            // Apply rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8);

            if ((target.position - transform.position).magnitude > desiredDistance)
            {
                // Move the drone to the viewed direction
                //transform.position += transform.forward * speed * Time.deltaTime;
                transformer.TranslationSpeed = speed;
                transformer.TranslationDirection = Vector3.forward;
            }
            // Do we want the bot to rotate or just hang in the air
            else
            {
                transformer.TranslationSpeed = 0;
                //transform.RotateAround(target.position, target.up, 30 * Time.deltaTime);
            }
        }
        else
        {

            ///////////////
            // ROADBLOCK //
            ///////////////
            if (target == null || target.transform == null)
            {
                TriggerLeave(null);
            }
            if (target == null || target.transform == null)
            {
                return;
            }
            ///////////////////
            // END ROADBLOCK //
            ///////////////////

            // Recalculates the path to get to the target
            direction = pc.RecalculatePath(target);
            // Rotation needed to look at direction
            if (direction != Vector3.zero)
                rot = Quaternion.LookRotation(direction);
            // Apply rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

            // Move the drone to the viewed direction
            //transform.position += transform.forward * speed * Time.deltaTime;
            transformer.TranslationSpeed = speed;
            transformer.TranslationDirection = Vector3.forward;
        }

    }

    // Go back to original target and state
    public void TriggerLeave(Collider Object)
    {
        if (Network.peerType == NetworkPeerType.Client && !GlobalSettings.SinglePlayer)
            return;

        // In case object no longer exists or target leaves
        // go back to main target
        if (Object == null || Object.transform == target)
        {
            // If our previous target is no longer available return ro spawn position
            if (prevTarget == null)
            {
                target = spawnPosition;
                currentState = prevState;
            }
            else
            {   
                target = prevTarget;
                currentState = prevState;
            }
        }

    }

    public void TriggerStay(Collider Object)
    {
        if (Network.peerType == NetworkPeerType.Client && !GlobalSettings.SinglePlayer)
            return;

        ///////////////
        // ROADBLOCK //
        ///////////////
        if (Object == null || Object.transform == null || target == null || target.transform == null)
        {
            TriggerLeave(null);
            return;
        }
        ///////////////////
        // End ROADBLOCK //
        ///////////////////
       
        // Check to make sure if the object getting this close is the object we 
        // are targeting
        if (Object.transform == target.transform)
        {
            // In case our target is not the mothership and it is no longer in the chase range
            if ((Object.transform.position - transform.position).magnitude > 700 && Object.transform.tag != "Mothership")
            {
                TriggerLeave(null);
            }
          
        }
        else
        {
            // If we are not in chasing mode check if 
            // we want to target opponent (if it is closer than current target)
            if (currentState != Behaviours.Chase)
            {
                TriggerEnter(Object);
            }
        }


    }

    public void TriggerEnter(Collider Object)
    {
        if (Network.peerType == NetworkPeerType.Client && !GlobalSettings.SinglePlayer)
            return;

        //  TODO change in some important condition like:
        //  - enough health
        //  - not state defending
        //  - some other?

        if (isOpponent(Object))
        {
            // TODO Only chase if this object is closer than current target

            // In csse the mothership is the new target
            // set the desired distance to mothership and
            // the radius in which to begin shooting
            if (Object.transform.tag == "Mothership")
            {

                if (currentState != Behaviours.Chase)
                {
                    prevTarget = target;
                    prevState = currentState;
                }
                target = Object.transform;

                currentState = Behaviours.Chase;
                desiredDistance = 700;
                shootRadius = 900;
            }
            // If the new target is not the mothership, make sure it
            // is close enough to start responding to it

            else if ((Object.transform.position - transform.position).magnitude < 700)
            {

                if (currentState != Behaviours.Chase)
                {
                    prevTarget = target;
                    prevState = currentState;
                }
                target = Object.transform;

                currentState = Behaviours.Chase;
                // check if the distance to the opponent is close enoug
                desiredDistance = 100;
                shootRadius = 200;
            }
        }

    }


    bool isOpponent(Collider Object)
    {

        // Drone attacks players, Npcs, and mothership of the opposing team
        if ((Object.gameObject.tag == "Player" || Object.gameObject.tag == "Npc" || Object.gameObject.tag == "Mothership") &&
            (checkTeam(Object.gameObject) != checkTeam(this.gameObject)))
        {

            return true;
        }

        return false;
    }

    // Returns the team of a game object
    int checkTeam(GameObject o)
    {
        string l = o.layer.ToString();
        switch (l)
        {
            case "8":
            case "9":
            case "10":
                return 1;
            case "11":
            case "12":
            case "13":
                return 2;
        }
        return 0;
    }

    bool InShootingRange()
    {
        ///////////////
        // ROADBLOCK //
        ///////////////
        if (target == null || target.transform == null)
        {
            TriggerLeave(null);
            return false;
        }
        ///////////////////
        // END ROADBLOCK //
        ///////////////////

        if ((transform.position - target.transform.position).magnitude < shootRadius)
        {
            return true;
        }
        return false;
    }

    // Sets a new target
    public void SetTarget(Transform newTarget)
    {
        prevTarget = target;
        target = newTarget;
    }

    #region Network Code
    public bool ClientBehaviour { get; set; }
    public bool KeepShooting { get; set; }

    private ObjectSync objectSync;

    private void clientBehaviourUpdate()
    {
        if (this.KeepShooting)
        {
            if (this.first)
            {
                this.GetGunScript();
                this.first = false;
            }

            foreach (Shooter s in this.gunScripts)
            {
                // Do not yet shoot
                s.Shoot();
            }
        }
    }
    #endregion

}
