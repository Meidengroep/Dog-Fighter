using UnityEngine;
using System.Collections;

public class ServerControl : NetworkObject 
{
	public Object RPCChannelPrefab;

	public LevelCreator CurrentLevel { get; private set; }

	public void ChangeLevel(LevelCreator level)
	{
		this.CurrentLevel = level;
		this.CurrentLevel.CreateLevel();		
	}

    public void Shutdown()
    {
        this.shuttingDown = true;

        foreach (NetworkPlayer p in Network.connections)
            Network.CloseConnection(p, true);

        Network.Disconnect();
        MasterServer.UnregisterHost();

        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();
        matchControl.ReturnToMenu();
    }

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();

		this.name = "ServerControl";

		Network.InitializeServer(10, GlobalSettings.ServerPort, false);
	}

    protected override void Update()
    {
        base.Update();

        //if (this.shuttingDown)
        //{
        //    Debug.Log("Connections: " + Network.connections.Length);
        //    if (Network.connections.Length == 0)
        //    {
        //        foreach (NetworkPlayer p in Network.connections)
        //            Network.CloseConnection(p, true);

        //        Network.Disconnect();
        //        MasterServer.UnregisterHost();

        //        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();
        //        matchControl.ReturnToMenu();
        //    }
        //}
    }

	private bool firstPlayerJoined;
    private bool firstTeamAssigned;
    private bool shuttingDown;

	private void OnServerInitialized()
	{
		// Network Initialize the RPCChannel to agree on a NetworkView with all clients.
		Network.Instantiate(this.RPCChannelPrefab, Vector3.zero, Quaternion.identity, 0);
	}

	private void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
        if (!this.firstPlayerJoined)
        {
            ObjectRPC.LoadLevel(0);
        }
        else
            ObjectRPC.LoadLevel(networkPlayer, 0);

		Debug.Log("A new player has joined.");
		Debug.Log("Current number of players: " + base.Players.Count);

		foreach (Player p in base.Players.Values)
		{
			PlayerRPC.NewPlayerJoined(networkPlayer, p.NetworkPlayerInfo, p.ID);
		}		

		// Generate a viewID for the new player.
		NetworkViewID viewID = Network.AllocateViewID();

		// Notice everyone that the new player has joined.
		PlayerRPC.NewPlayerJoined(networkPlayer, viewID);
        
        Player newPlayer = base.Players[viewID];

        // Tell the new player the teams of all the other players.
        foreach (Player p in base.Players.Values)
            if (!(p.ID == base.NetworkControl.LocalViewID || p.ID == newPlayer.ID))
                PlayerRPC.SetPlayerTeam(newPlayer.NetworkPlayerInfo, p.ID, (int)p.Team);

        this.assignTeam(newPlayer);

        // Sync the existing game world with the new player.
        this.CurrentLevel.SyncNewPlayer(newPlayer);

	}

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Player disconnectedPlayer = null;

        foreach (Player p in base.Players.Values)
            if (p.NetworkPlayerInfo == player)
            {
                disconnectedPlayer = p;
                break;
            }

        Debug.Log("Team " + TeamHelper.GetTeamNumber((int)disconnectedPlayer.Team) + " has been disconnected!");

        MatchResult result = disconnectedPlayer.Team == Layers.Team1Actor ? MatchResult.Team2Win : MatchResult.Team1Win;

        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();
        matchControl.EndMatch(result);

    }

    private void assignTeam(Player player)
    {
        Layers layer = Layers.Team1Actor;

        if (this.firstPlayerJoined)
            layer = Layers.Team2Actor;

        this.firstPlayerJoined = true;
        player.Team = layer;

        Debug.Log("Assigned layer: " + layer);

        PlayerRPC.SetPlayerTeam(player.ID, (int)player.Team);
    }
}
