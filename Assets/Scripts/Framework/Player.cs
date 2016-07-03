using UnityEngine;

public class Player
{
	public NetworkViewID ID { get; private set; }
	public NetworkPlayer NetworkPlayerInfo { get; private set; }

    public Layers Team { get; set; }

	public Player(NetworkViewID id, NetworkPlayer networkPlayerInfo)
	{
		this.ID = id;
		this.NetworkPlayerInfo = networkPlayerInfo;
	}
}
