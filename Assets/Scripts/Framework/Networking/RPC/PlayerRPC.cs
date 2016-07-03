using UnityEngine;
using System.Collections;

public class PlayerRPC : RPCHolder {

	public static void NewPlayerJoined(NetworkPlayer networkPlayer, NetworkViewID id)
	{
        if (StopSend())
            return;

		Debug.Log("Sending new player joined.");

		Channel.GetComponent<NetworkView>().RPC("NewPlayerJoinedRPC", RPCMode.All, networkPlayer, id);
	}

	public static void NewPlayerJoined(NetworkPlayer target, NetworkPlayer networkPlayer, NetworkViewID id)
	{
        if (StopSend())
            return;

		Debug.Log("Sending new player joined.");
		
		Channel.GetComponent<NetworkView>().RPC("NewPlayerJoinedRPC", target, networkPlayer, id);
	}

    public static void SetPlayerTeam(NetworkViewID playerID, int layer)
    {
        if (StopSend())
            return;        

        Channel.GetComponent<NetworkView>().RPC("SetPlayerTeamRPC", RPCMode.Others, playerID, layer);
    }

    public static void SetPlayerTeam(NetworkPlayer target, NetworkViewID playerID, int layer)
    {
        Channel.GetComponent<NetworkView>().RPC("SetPlayerTeamRPC", target, playerID, layer);

    }

	[RPC]
	private void NewPlayerJoinedRPC(NetworkPlayer networkPlayer, NetworkViewID id, NetworkMessageInfo info)
	{
        if (StopSend())
            return;

		Debug.Log("New player joined RPC received!");
		
		Player player = new Player(id, networkPlayer);
		base.NetworkControl.Players.Add(id, player);
		base.ObjectTables.AddPlayerTable(player);

        if (Network.isClient)
		{
			if (networkPlayer.ipAddress == base.NetworkControl.LocalIP)
			{
				// Set own ID assigned by server.
				base.NetworkControl.LocalViewID = id;
			}

		}
	}

    [RPC]
    private void SetPlayerTeamRPC(NetworkViewID playerID, int layer, NetworkMessageInfo info)
    {
        ///
        /// This RPC is only used to set the player team before all the player objects are created.
        /// This function does not propagate the team change to all existing player objects.
        ///

        base.Players[playerID].Team = (Layers)layer;
    }
}
