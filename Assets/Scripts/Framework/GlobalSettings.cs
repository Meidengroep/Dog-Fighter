using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalSettings : MonoBehaviour 
{
    public const string NetworkControlName = "NetworkControl";
    public const string ServerControlName = "ServerControl";
    public const string ClientControlName = "ClientControl";
    public const string RPCChannelName = "RPCChannel";
    public const string MatchControlName = "MatchControl";
    public const string PlayerObjectTableName = "PlayerObjectTable";

    public const string Team1MothershipName = "Team1Mothership";
    public const string Team2MothershipName = "Team2Mothership";

	public static bool SinglePlayer = false;
	
	public static bool HasFocus { get; private set;}

    public static bool IsServer = false;
    public static int ServerPort = 6500;
    public static string ServerIP = "127.0.0.1";

    public static IList<GameObject> Team1Npcs;
    public static IList<GameObject> Team2Npcs;
	
	// Use this for initialization
	void Start () 
    {
        Team1Npcs = new List<GameObject>();
        Team2Npcs = new List<GameObject>();

        DontDestroyOnLoad(this);

        Application.LoadLevel("MainMenu");
	}

    public static void AddNpc(GameObject npc)
    {
        int team = TeamHelper.GetTeamNumber(npc.layer);

        if (team == 1)
            Team1Npcs.Add(npc);
        else Team2Npcs.Add(npc);
    }

    public static void RemoveNpc(GameObject npc)
    {
        int team = TeamHelper.GetTeamNumber(npc.layer);

        if (team == 1)
            Team1Npcs.Remove(npc);
        else Team2Npcs.Remove(npc);
    }

	void OnApplicationFocus(bool focus) 
	{
    	HasFocus = focus;    
		//Debug.Log("Focus = " + this.HasFocus);
	}
}
