using UnityEngine;
using System.Collections;
using System;

public class PlayerShipRPC : RPCHolder {

	public GameObject PlayerPrefab;

	#region Call Functions
	public static void CreatePlayerShip(Player player, int objectID)
	{
        if (StopSend())
            return;

        //Debug.Log("Sending create player ship.");

		Channel.GetComponent<NetworkView>().RPC("CreatePlayerShipRPC", RPCMode.All, player.ID, objectID);
	}

	public static void CreatePlayerShip(NetworkPlayer target, Player player, int objectID)
	{
        if (StopSend())
            return;

		Channel.GetComponent<NetworkView>().RPC("CreatePlayerShipRPC", target, player.ID, objectID);
	}

	public static void SpawnPlayerShip(Player player, int spawnPointID, int playerShipID)
	{
        if (StopSend())
            return;

        //Debug.Log("Sending spawn player RPC");

		Channel.GetComponent<NetworkView>().RPC("SpawnPlayerShipRPC", RPCMode.All, player.ID, spawnPointID, playerShipID);
		
	}
	public static void SpawnPlayerShip(NetworkPlayer target, Player player, int spawnPointID, int playerShipID)
	{
        if (StopSend())
            return;

        //Debug.Log("Sending spawn player RPC");

		Channel.GetComponent<NetworkView>().RPC("SpawnPlayerShipRPC", target, player.ID, spawnPointID, playerShipID);			
	}

	public static void FireWeapon(Player player, bool fire) 
	{
        if (StopSend())
            return;

		RPCMode mode;

		if (Network.isClient)
			mode = RPCMode.Server;
		else
			mode = RPCMode.Others;

		Channel.GetComponent<NetworkView>().RPC ("FireWeaponRPC", mode, player.ID, fire);
		
	}

    public static void FiringDirection(Player player, Vector3 direction)
    {
        if (StopSend())
            return;

        RPCMode mode;

        if (Network.isClient)
            mode = RPCMode.Server;
        else
            mode = RPCMode.Others;

        Channel.GetComponent<NetworkView>().RPC("FiringDirectionRPC", mode, player.ID, direction);
    }
	#endregion

	#region RPC Definitions
	[RPC]
	private void CreatePlayerShipRPC(NetworkViewID playerID, int objectID, NetworkMessageInfo info)
	{
		Debug.Log("Create player ship RPC received!");

		Player owner = base.NetworkControl.Players[playerID];
		
        // Create the player ship.
		GameObject playerShip = (GameObject)GameObject.Instantiate(this.PlayerPrefab);
        TeamHelper.PropagateLayer(playerShip, (int)owner.Team);
		
		if (base.NetworkControl.LocalViewID == playerID)
		{
            // Set variables for when this ship is controlled by the client receiving the RPC.
			base.ObjectTables.ThisPlayerObjects.PlayerShipID = objectID;
			playerShip.GetComponentInChildren<Camera>().enabled = true;
		}
		else
		{
            // Disable components that mostly has to do with interface and controls for when the
            // ship is controlled by another player.
            playerShip.GetComponentInChildren<Camera>().enabled = false;
            playerShip.GetComponentInChildren<AudioListener>().enabled = false;		
            playerShip.GetComponent<ShipControl>().enabled = false;
            playerShip.GetComponent<SoftwareMouse>().enabled = false;
            playerShip.GetComponent<HUD>().enabled = false;
            playerShip.GetComponent<PlayerHealthControl>().DrawHealthInfo = false;
            playerShip.GetComponentInChildren<ThirdPersonCrosshair>().enabled = false;
        }

        // Set object type.
        ObjectSync objSync = playerShip.GetComponent<ObjectSync>();
        objSync.Type = ObjectSyncType.PlayerShip;

        // Set spawn point reference.
        Player player = this.NetworkControl.Players[playerID];
        int spawnPointID = this.ObjectTables.PlayerObjects[player].PlayerSpawnPointID;
        GameObject spawnPoint = this.ObjectTables.GetPlayerObject(player, spawnPointID);
        playerShip.GetComponent<PlayerHealthControl>().RespawnPoint = spawnPoint.GetComponent<PlayerRespawner>();
        spawnPoint.GetComponent<PlayerRespawner>().AttachPlayer(playerShip);

        // Add to global object tables.
		base.ObjectTables.PlayerObjects[owner].PlayerShipID = objectID;
		base.AddToObjectTables(playerShip, playerID, objectID);
	}

	[RPC]
	private void SpawnPlayerShipRPC(NetworkViewID owner, int spawnPointID, int playerShipID)
	{
//		Debug.Log("Spawn player RPC received.");

		Player player = base.Players[owner];
		GameObject spawnPoint = base.ObjectTables.GetPlayerObject(player, spawnPointID);
		GameObject playerShip = base.ObjectTables.GetPlayerObject(player, playerShipID);

		spawnPoint.GetComponent<PlayerRespawner>().NetworkSpawn(playerShip);
	}

	[RPC]
	private void FireWeaponRPC(NetworkViewID playerID, bool fire)
	{
		GameObject playerShip = base.GetPlayerShip(playerID);

		GunSwitcher gunSwitcher = playerShip.GetComponent<GunSwitcher>();

		foreach (GameObject gun in gunSwitcher.CurrentGuns)
		{
			Shooter shooter = gun.GetComponent<Shooter>();
			shooter.KeepFiring = fire;
		}

		if (Network.isServer)
			FireWeapon(base.Players[playerID], fire);
	}

    [RPC]
    private void FiringDirectionRPC(NetworkViewID playerID, Vector3 direction)
    {
        // Ignore own shooting direction RPCs echoed by server to all players.
        if (base.NetworkControl.ThisPlayer.ID == playerID)
            return;

        GameObject playerShip = base.GetPlayerShip(playerID);

        GunSwitcher gunSwitcher = playerShip.GetComponent<GunSwitcher>();
        gunSwitcher.AssignCrossHairPosition(direction);

        if (Network.isServer)
            FiringDirection(base.Players[playerID], direction);
    }
	#endregion
}
