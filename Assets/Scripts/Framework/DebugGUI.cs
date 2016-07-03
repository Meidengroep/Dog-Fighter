using UnityEngine;
using System.Collections;

public class DebugGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (GlobalSettings.SinglePlayer)
			return; 

        //GUI.contentColor = Color.white;
        //if (Network.peerType == NetworkPeerType.Disconnected)
        //    GUI.Label(new Rect(10, 10, 1000, 1000), "Not connected. F1 = Server. F2 = Client");
        //else if (Network.peerType == NetworkPeerType.Server)
        //    GUI.Label(new Rect(10, 10, 100, 100), "Is server.");
        //else if (Network.peerType == NetworkPeerType.Client)
        //    GUI.Label(new Rect(10, 10, 100, 100), "Is client.");
		
	}
}
