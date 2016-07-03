using UnityEngine;
using System.Collections;

public class LevelCreator : NetworkObject 
{
    public const string AsteroidRingName = "AsteroidRingGenerator";

    public GameObject AsteroidRing { get; set; }

	public LevelCreator()
		: base()
	{
		base.Start();
	}

	public virtual void CreateLevel()
	{
        GameObject obj = (GameObject)GameObject.Instantiate(AsteroidRing);
        obj.name = AsteroidRingName;
	}

	public virtual void SyncNewPlayer(Player newPlayer)
	{
	}

}
