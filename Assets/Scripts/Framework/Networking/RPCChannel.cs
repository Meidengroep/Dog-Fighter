using UnityEngine;
using System.Collections;
using System;

public class RPCChannel : NetworkObject 
{
    protected override void Awake()
    {
        this.name = GlobalSettings.RPCChannelName;

        base.Awake();
    }

    protected void OnDestroy()
    {
        UnityEngine.Debug.Log("RPCChannel destroyed.");

        try
        {
            GameObject.Find(GlobalSettings.NetworkControlName).GetComponent<NetworkControl>().Shutdown();
        }
        catch (NullReferenceException)
        {
            Debug.Log("NullReferenceException processing OnDestroy() of RPCChannel. Has the game been terminated by user?");
        }
    }

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

		if (Network.isServer)
		{
			base.NetworkControl.LocalViewID = this.GetComponent<NetworkView>().viewID;
		}
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}
}
