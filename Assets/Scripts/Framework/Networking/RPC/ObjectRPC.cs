using UnityEngine;
using System.Collections;

public class ObjectRPC : RPCHolder 
{
	public GameObject PlayerSpawnPointPrefab;
    public GameObject MothershipPrefab;
    public GameObject AIDronePrefab;
    public GameObject AsteroidRing;

    #region Calls

    public static void SetObjectTag(Player objectOwner, int objectID, string tag)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("SetObjectTagRPC", RPCMode.All, objectOwner.ID, objectID, tag);
    }
    public static void SetObjectLayer(Player objectOwner, int objectID, Layers layer)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("SetObjectLayerRPC", RPCMode.All, objectOwner.ID, objectID, (int)layer);
    }
    public static void SetObjectLayer(NetworkPlayer target, Player objectOwner, int objectID, Layers layer)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("SetObjectLayerRPC", target, objectOwner.ID, objectID, (int)layer);
    }

    public static void LoadLevel(int levelID)
	{
        if (StopSend())
            return;

		Channel.CheckServer();

		Channel.GetComponent<NetworkView>().RPC("LoadLevelRPC", RPCMode.All, levelID);
	}
    public static void LoadLevel(NetworkPlayer target, int levelID)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("LoadLevelRPC", target, levelID);
    }

    public static void ObjectPosition(Player player, int objectID, Vector3 position, Vector3 orientation)
    {
        if (StopSend())
            return;

        //Debug.Log ("Sending player ship position.");

        Channel.GetComponent<NetworkView>().RPC("ObjectPositionRPC", RPCMode.Others, player.ID, objectID, position, orientation);
    }
    public static void ObjectVelocityClient(Player player, int objectID, Vector3 transform, Vector3 rotation)
    {
        if (StopSend())
            return;

        Channel.CheckClient();

        Channel.GetComponent<NetworkView>().RPC("ObjectVelocityRPC", RPCMode.Server, player.ID, objectID, transform, rotation);
    }
    public static void ObjectVelocityServer(Player player, int objectID, Vector3 transform, Vector3 rotation)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("ObjectVelocityRPC", RPCMode.Others, player.ID, objectID, transform, rotation);
    }

    public static void CreateMothership(NetworkPlayer target, Player owner, int objectID, int layer)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("CreateMothershipRPC", target, owner.ID, objectID, layer);
    }

    public static void CreateDrone(Player owner, int objectID, Vector3 position, int layer)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("CreateDroneRPC", RPCMode.Others, owner.ID, objectID, position, layer);
    }
    public static void CreateDrone(NetworkPlayer target, Player owner, int objectID, Vector3 position, int layer)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("CreateDroneRPC", target, owner.ID, objectID, position, layer);
    }
    public static void DroneShoot(Player owner, int objectID, bool shoot)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        try
        {
            Channel.GetComponent<NetworkView>().RPC("DroneShootRPC", RPCMode.Others, owner.ID, objectID, shoot);
        }
        catch (UnityException)
        {
            // This catch is meant only for when players disconnect and RPCs are lost.
            // Doing a catch like this is a bit risky.
            Debug.Log("DroneShootRPC failed to send.");
        }
    }

	public static void CreatePlayerSpawnpoint(Player owner, int objectID, Vector3 position)
	{
        if (StopSend())
            return;

		Channel.CheckServer();

		Channel.GetComponent<NetworkView>().RPC("CreatePlayerSpawnpointRPC", RPCMode.All, owner.ID, objectID, position);
	}
	public static void CreatePlayerSpawnpoint(NetworkPlayer target, Player owner, int objectID, Vector3 position)
	{
        if (StopSend())
            return;

		Channel.CheckServer();		
		
		Channel.GetComponent<NetworkView>().RPC("CreatePlayerSpawnpointRPC", target, owner.ID, objectID, position);
	}

	public static void SetObjectHealth(Player objectOwner, int objectID, float health, float shields)
	{
        if (StopSend())
            return;

		Channel.CheckServer();

        //Debug.Log("Sending SetObjectHealthRPC");

		Channel.GetComponent<NetworkView>().RPC("SetObjectHealthRPC", RPCMode.Others, objectOwner.ID, objectID, health, shields);
	}
    public static void KillObject(Player objectOwner, int objectID)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        //Debug.Log("Sending KillObjectRPC.");

        Channel.GetComponent<NetworkView>().RPC("KillObjectRPC", RPCMode.All, objectOwner.ID, objectID);
    }
    /// <summary>
    /// Send an RPC to all clients to respawn a certain object. May only be used by the server.
    /// This function currently only works for player ships.
    /// </summary>
    /// <param name="spawnPointOwner">The spawnpoint that will respawn the attatched player.</param>
    /// <param name="spawnPointID">The ID of the spawnpoint.</param>
    public static void RespawnObject(Player spawnPointOwner, int spawnPointID)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("RespawnObjectRPC", RPCMode.Others, spawnPointOwner.ID, spawnPointID);
    }

    public static void CreateAsteroid(NetworkPlayer target, Player owner, int id, Vector3 position, Vector3 scale, string name)
    {
        if (StopSend())
            return;

        Channel.CheckServer();

        Channel.GetComponent<NetworkView>().RPC("CreateAsteroidRPC", target, owner.ID, id, position, scale, name);
    }

    #endregion

    #region RPCs
    [RPC]
    private void SetObjectTagRPC(NetworkViewID owner, int objectID, string tag)
    {
        GameObject obj = base.GetObject(owner, objectID);
        obj.tag = tag;
    }
    [RPC]
    private void SetObjectLayerRPC(NetworkViewID owner, int objectID, int layer)
    {
        //Debug.Log("SetObjectLayerRPC received.");

        GameObject obj = base.GetObject(owner, objectID);
        obj.name = "Player_" + layer;
        obj.layer = layer;
        TeamHelper.PropagateLayer(obj, layer);
    }

    [RPC]
	private void LoadLevelRPC(int levelID, NetworkMessageInfo info)
	{
		LevelCreator creator = null;
		
		switch (levelID)
		{
		    case 0:
                MultiplayerPrototype1Level npl = this.gameObject.AddComponent<MultiplayerPrototype1Level>();
                npl.MothershipPrefab = this.MothershipPrefab;
                npl.AsteroidRing = this.AsteroidRing;
                creator = npl;
			    break;
		}

		if (Network.peerType == NetworkPeerType.Server)
		{
			GameObject.Find("ServerControl").GetComponent<ServerControl>().ChangeLevel(creator);
		}
		else if (Network.peerType == NetworkPeerType.Client)
		{
			GameObject.Find("ClientControl").GetComponent<ClientControl>().ChangeLevel(creator);
		}
	}

    [RPC]
    private void ObjectPositionRPC(NetworkViewID objectOwner, int objectID, Vector3 position, Vector3 orientation)
    {
        //		Debug.Log("Player ship position received.");

        GameObject obj = base.GetObject(objectOwner, objectID);
        obj.transform.position = position;
        obj.transform.eulerAngles = orientation;
    }

    [RPC]
    private void ObjectVelocityRPC(NetworkViewID objectOwner, int objectID, Vector3 translation, Vector3 rotation)
    {
        //		Debug.Log("Player ship velocity received.");		

        GameObject obj = base.GetObject(objectOwner, objectID);
        ObjectTransformer objectTransform = obj.GetComponent<ObjectTransformer>();

        objectTransform.Translation = translation;
        objectTransform.Rotation = rotation;
    }

	[RPC]
	private void CreatePlayerSpawnpointRPC(NetworkViewID owner, int objectID, Vector3 position, NetworkMessageInfo info)
	{		
		Debug.Log("Create player spawn point RPC received.");

        Player player = base.NetworkControl.Players[owner];

		GameObject spawnPoint = (GameObject)GameObject.Instantiate(this.PlayerSpawnPointPrefab, position, Quaternion.identity);
        ObjectSync spawnPointSync = spawnPoint.GetComponent<ObjectSync>();
        spawnPointSync.Type = ObjectSyncType.PlayerSpawnPoint;
        spawnPoint.layer = (int)player.Team;

        GameObject mothership = base.GetMothership(player.Team);
        spawnPoint.transform.parent = mothership.transform;
        
        base.ObjectTables.PlayerObjects[base.Players[owner]].PlayerSpawnPointID = objectID;
		base.AddToObjectTables(spawnPoint, owner, objectID);
	}

    [RPC]
    private void CreateMothershipRPC(NetworkViewID owner, int objectID, int layer)
    {
        //Debug.Log("Received CreateMothershipRPC");

        GameObject motherShip = (GameObject)GameObject.Instantiate(this.MothershipPrefab);
        motherShip.GetComponent<DroneSpawn>().enabled = false;
        //motherShip.GetComponent<MovingObjectSync>().SuppressVelocitySync = true;  

        TeamHelper.PropagateLayer(motherShip, layer);

        if (motherShip.layer == (int)Layers.Team1Mothership)
            motherShip.name = "Team1Mothership";
        else if (motherShip.layer == (int)Layers.Team2Mothership)
            motherShip.name = "Team2Mothership";

        base.AddToObjectTables(motherShip, owner, objectID);
    }

    [RPC]
    private void CreateDroneRPC(NetworkViewID owner, int objectID, Vector3 position, int layer)
    {
        GameObject drone = (GameObject)GameObject.Instantiate(this.AIDronePrefab, position, Quaternion.identity);
        //ObjectSync objSync = drone.GetComponent<ObjectSync>();
        drone.GetComponent<HealthControl>().DrawHealthInfo = false;
        //drone.GetComponent<DroneBehaviour>().enabled = false;
        //drone.GetComponent<NpcListUpdater>().enabled = false;
        //drone.GetComponent<MovingObjectSync>().SuppressVelocitySync = true;

        TeamHelper.PropagateLayer(drone, layer);

        base.AddToObjectTables(drone, owner, objectID);
    }

    [RPC]
    private void DroneShootRPC(NetworkViewID owner, int objectID, bool shoot)
    {
        GameObject drone = base.GetObject(owner, objectID);
        drone.GetComponent<DroneBehaviour>().KeepShooting = shoot;
    }

	[RPC]
	private void SetObjectHealthRPC(NetworkViewID objectOwner, int objectID, float health, float shields)
	{
        //Debug.Log("SetObjectHealthRPC received.");

		GameObject obj = base.GetObject(objectOwner, objectID);
		HealthControl healthControl = obj.GetComponent<HealthControl>();

		healthControl.CurrentHealth = health;
		healthControl.CurrentShields = shields;
	}

    [RPC]
    private void KillObjectRPC(NetworkViewID objectOwner, int objectID)
    {
        //Debug.Log("KillObjectRPC received.");

        GameObject obj = base.GetObject(objectOwner, objectID);

        //if (Network.peerType == NetworkPeerType.Server)
        //{
        //    ObjectSync objSync = obj.GetComponent<ObjectSync>();

        //    if (objSync != null)
        //        objSync.Dispose();
        //}
        HealthControl healthControl = obj.GetComponent<HealthControl>();
        healthControl.Die();
    }

    /// <summary>
    /// RPC to respawn a certain object. This function currently only works for player ships.
    /// </summary>
    /// <param name="spawnPointOwner">The spawnpoint that will respawn the attatched player.</param>
    /// <param name="spawnPointID">The ID of the spawnpoint.</param>
    [RPC]
    private void RespawnObjectRPC(NetworkViewID spawnPointOwner, int spawnPointID)
    {
        //Debug.Log("RespawnObjectRPC received.");

        GameObject spawnPoint = base.GetObject(spawnPointOwner, spawnPointID);
        PlayerRespawner respawner = spawnPoint.GetComponent<PlayerRespawner>();

        respawner.Respawn();
    }

    [RPC]
    private void CreateAsteroidRPC(NetworkViewID objectOwner, int objectID, Vector3 position, Vector3 localScale, string name)
    {
        this.AsteroidRingGenerator.CreateAsteroid(position, localScale, name, base.Players[objectOwner], objectID); 
    }

    protected ClientControl ClientControl
    {
        get
        {
            if (this.clientControl == null)
                this.clientControl = GameObject.Find(GlobalSettings.ClientControlName).GetComponent<ClientControl>();
            return this.clientControl;
        }
    }

    protected AsteroidRingGenerator AsteroidRingGenerator
    {
        get
        {
            if (this.asteroidRingGenerator == null)
                this.asteroidRingGenerator = GameObject.Find(LevelCreator.AsteroidRingName).GetComponent<AsteroidRingGenerator>();
            return this.asteroidRingGenerator;
        }
    }

    private ClientControl clientControl;
    private AsteroidRingGenerator asteroidRingGenerator;
    #endregion
}
