using UnityEngine;
using System.Collections;

public class ClientControl : NetworkObject 
{
	public LevelCreator CurrentLevel { get; private set; }
	
	public void ChangeLevel(LevelCreator level)
	{
		this.CurrentLevel = level;
		this.CurrentLevel.CreateLevel();
	}

    public void Shutdown()
    {
        Debug.Log("Disconnecting from server.");
        foreach (NetworkPlayer id in Network.connections)
            Network.CloseConnection(id, false);
        Network.Disconnect();
        MasterServer.UnregisterHost();

        GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>().ReturnToMenu();
    }

	// Use this for initialization
	protected override void Start () {

		base.Start();

		this.name = "ClientControl";

        //Debug.Log("Connecting");

		Network.Connect(GlobalSettings.ServerIP, GlobalSettings.ServerPort);
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}

	private void OnConnectedToServer()
	{
		Debug.Log("Connected to server.");
	}

    private void OnDisconnectedFromServer()
    {
        //throw new UnityException("Disconnected from server!");
        Debug.Log("Disconnected from server!");

        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();
        matchControl.EndMatch(MatchResult.Disconnected);
    }

	private void OnFailedToConnect(NetworkConnectionError error) 
	{
		GameObject.Destroy(this);
	}
}
