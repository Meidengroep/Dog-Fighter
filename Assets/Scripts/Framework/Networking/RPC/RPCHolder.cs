using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class RPCHolder : NetworkObject 
{
	public RPCMode RPCMode { get; set; }

	// Use this for initialization
	protected override void Start () {

		base.Start();

		this.RPCMode = RPCMode.All;
	}

	protected void AddToObjectTables(GameObject obj, NetworkViewID playerID, int objectID)
	{
		ObjectSync objSync = obj.GetComponent<ObjectSync>();

		Player player = this.NetworkControl.Players[playerID];

		objSync.AssignID(player, objectID);

		this.ObjectTables.AddPlayerObject(player, objectID, obj);
	}

    [MethodImpl(MethodImplOptions.NoInlining)]
	public void CheckServer()
	{
        if (Network.peerType != NetworkPeerType.Server)
        {
            string callerName = new StackFrame(1, true).GetMethod().Name;

            throw new UnityException("Only the server may use this function: " + callerName);
        }
	}
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void CheckClient()
    {
        if (Network.peerType != NetworkPeerType.Client)
        {
            string callerName = new StackFrame(1, true).GetMethod().Name;

            throw new UnityException("Only the server may use this function: " + callerName);
        }
    }

    public static bool StopSend()
    {
        return NetworkControlStatic.StopAll;
    }

    public static PlayerRPC Channel
    {
        get
        {
            if (channel_ == null)
                channel_ = GameObject.Find(GlobalSettings.RPCChannelName).GetComponent<PlayerRPC>();
            return channel_;
        }
    }

    public static NetworkControl NetworkControlStatic
    {
        get
        {
            if (networkControl_ == null)
                networkControl_ = GameObject.Find(GlobalSettings.NetworkControlName).GetComponent<NetworkControl>();
            return networkControl_;
        }
    }

    private static PlayerRPC channel_;
    private static NetworkControl networkControl_;

    private bool destroyed;
}
