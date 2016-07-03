using UnityEngine;
using System.Collections.Generic;

public class MultiplayerPrototype1Level : LevelCreator 
{
    public const int LevelID = 1;

    public GameObject MothershipPrefab;

	public override void CreateLevel ()
	{
		if (Network.peerType == NetworkPeerType.Server)
			this.createServerSideObjects();

        base.CreateLevel();
	}

	public override void SyncNewPlayer(Player newPlayer)
	{
		base.SyncNewPlayer(newPlayer);

        // Sync the objects that are not associated with any player.
        ObjectTable serverTable = base.ObjectTables.GetPlayerTable(base.NetworkControl.ThisPlayer);
        ICollection<GameObject> serverObjects = serverTable.GetAllObjects();
        Debug.Log("Number of server objects: " + serverObjects.Count);

        foreach (GameObject obj in serverObjects)
        {
            ObjectSync objSync = obj.GetComponent<ObjectSync>();

            switch (objSync.Type)
            {
                case ObjectSyncType.Mothership:
                    ObjectRPC.CreateMothership(newPlayer.NetworkPlayerInfo, objSync.Owner, objSync.GlobalID, obj.layer);

                    // Temporary mothership positions.
                    if (obj.layer == (int)Layers.Team1Mothership)
                    {
                        ObjectRPC.ObjectPosition(objSync.Owner, objSync.GlobalID, new Vector3(1000, 0, 0), Vector3.zero);
                    }
                    else
                    {
                        ObjectRPC.ObjectPosition(objSync.Owner, objSync.GlobalID,  new Vector3(-1000, 0, 0), Vector3.zero);
                    }

                    break;
                case ObjectSyncType.Drone:
                    ObjectRPC.CreateDrone(newPlayer.NetworkPlayerInfo, objSync.Owner, objSync.GlobalID, obj.transform.position, obj.layer);
                    break;
                case ObjectSyncType.Asteroid:
                    ObjectRPC.CreateAsteroid(newPlayer.NetworkPlayerInfo, objSync.Owner, objSync.GlobalID, obj.transform.position, obj.transform.localScale, obj.name);
                    break;

            }
        }

        // Sync the objects that belong to other players.
		foreach (Player p in base.Players.Values)
		{
            // Skip all objects for the server player (already synced) and the new player (empty).
            if (!(p.ID == base.NetworkControl.LocalViewID || p.ID == newPlayer.ID))
            {
                // Sync player ships and player spawnpoints
                PlayerObjects playerObjects = base.ObjectTables.PlayerObjects[p];

                GameObject spawnPoint = base.GetObject(p, playerObjects.PlayerSpawnPointID);

                ObjectRPC.CreatePlayerSpawnpoint(
                    newPlayer.NetworkPlayerInfo
                    , p
                    , playerObjects.PlayerSpawnPointID
                    , spawnPoint.transform.position);

                GameObject playerShip = base.GetObject(p, playerObjects.PlayerShipID);

                PlayerShipRPC.CreatePlayerShip(newPlayer.NetworkPlayerInfo, p, playerObjects.PlayerShipID);
                //ObjectRPC.SetObjectLayer(newPlayer.NetworkPlayerInfo, p, playerObjects.PlayerShipID, (Layers)playerShip.layer);
            }
		}

        // Create the objects for the new player.
		int spawnPointID = base.GUIDGenerator.GenerateID();
		int playerShipID = base.GUIDGenerator.GenerateID();       

        GameObject mothership = base.GetMothership(newPlayer.Team);
        Vector3 spawnPointPos = mothership.transform.position + new Vector3(0, 500, 0);

        // The order in which the following RPCs are sent is critical!
		ObjectRPC.CreatePlayerSpawnpoint(newPlayer, spawnPointID, spawnPointPos);
        PlayerShipRPC.CreatePlayerShip(newPlayer, playerShipID);
		PlayerShipRPC.SpawnPlayerShip(newPlayer, spawnPointID, playerShipID);
	}

    /// <summary>
    /// Initializes the server-side objects.
    /// </summary>
	private void createServerSideObjects()
	{
        Player server = new Player(base.NetworkControl.LocalViewID, Network.player);
        base.NetworkControl.Players.Add(server.ID, server);
        base.ObjectTables.AddPlayerTable(server);
        Debug.Log("Created server player: " + server.ID);

        Player serverPlayer = base.NetworkControl.ThisPlayer;

        // Initialize the starting positions of the motherships.
        Vector3 team1MothershipPos = new Vector3(2000, 0, 0);
        Vector3 team2MothershipPos = new Vector3(-2000, 0, 0);

        // Initialize te motherships.
        GameObject team1Mothership = (GameObject)GameObject.Instantiate(
            this.MothershipPrefab, team1MothershipPos, Quaternion.identity
            );
        GameObject team2Mothership = (GameObject)GameObject.Instantiate(
           this.MothershipPrefab, team2MothershipPos, Quaternion.identity
           );

        // Assign teams to the motherships.
        TeamHelper.PropagateLayer(team1Mothership, (int)Layers.Team1Mothership);
        TeamHelper.PropagateLayer(team2Mothership, (int)Layers.Team2Mothership);
        team1Mothership.name = "Team1Mothership";
        team2Mothership.name = "Team2Mothership";

        // Generate object IDs for the motherships.
        int team1MothershipID = base.GUIDGenerator.GenerateID();
        int team2MothershipID = base.GUIDGenerator.GenerateID();

        // Assign some values.
        ObjectSync team1MSObjSync = team1Mothership.GetComponent<ObjectSync>();
        team1MSObjSync.Type = ObjectSyncType.Mothership;
        team1MSObjSync.AssignID(serverPlayer, team1MothershipID);
        HealthControl team1MSHealthControl = team1Mothership.GetComponent<HealthControl>();
        team1MSHealthControl.DrawHealthInfo = false;
        
        ObjectSync team2MSObjSync = team2Mothership.GetComponent<ObjectSync>();
        team2MSObjSync.Type = ObjectSyncType.Mothership;
        team2MSObjSync.AssignID(serverPlayer, team2MothershipID);
        HealthControl team2MSHealthControl = team2Mothership.GetComponent<HealthControl>();
        team2MSHealthControl.DrawHealthInfo = false;

        base.ObjectTables.AddPlayerObject(serverPlayer, team1MothershipID, team1Mothership);
        base.ObjectTables.AddPlayerObject(serverPlayer, team2MothershipID, team2Mothership);
	}
}
