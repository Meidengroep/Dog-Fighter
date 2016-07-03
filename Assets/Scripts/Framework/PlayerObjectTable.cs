using UnityEngine;
using System.Collections.Generic;

public class PlayerObjectTable : NetworkObject
{
	public IDictionary<Player, PlayerObjects> PlayerObjects { get; private set; }
	public PlayerObjects ThisPlayerObjects 
	{
		get
		{
			return this.PlayerObjects[base.NetworkControl.ThisPlayer];
		}
	}

	public PlayerObjectTable()
		: base()
	{
		this.objectTables = new Dictionary<NetworkViewID, ObjectTable>(10);
		this.PlayerObjects = new Dictionary<Player, PlayerObjects>(10);
	}

    public ObjectTable GetPlayerTable(Player player)
    {
        ObjectTable table;

        if (this.objectTables.TryGetValue(player.ID, out table))
            return table;
        else
            throw new UnityException("Table for given player does not exist.");
    }

	public void AddPlayerTable(Player player)
	{
		if (!this.objectTables.ContainsKey(player.ID))
		{
			this.objectTables.Add(player.ID, new ObjectTable());
			this.PlayerObjects.Add(player, new PlayerObjects());
		}
		else
			throw new UnityException("Table for given player already exists.");
	}
	
	public void RemovePlayerTable(Player player)
	{
		if (this.objectTables.ContainsKey(player.ID))
		{
			this.objectTables.Remove(player.ID);
			this.PlayerObjects.Remove(player);
		}
		else
			throw new UnityException("Table for given player does not exist.");
	}

	public GameObject GetPlayerObject(Player player, int objID)
	{
        return this.GetPlayerTable(player).GetObject(objID);
	}

	public void AddPlayerObject(Player player, int objID, GameObject obj)
	{
		ObjectTable table;
		
		if (this.objectTables.TryGetValue(player.ID, out table))
			table.AddObject(objID, obj);
		else
			throw new UnityException("Table for given player does not exist.");
	}

	public void RemovePlayerObject(Player player, int objID)
	{
		ObjectTable table;       

        if (this.objectTables.TryGetValue(player.ID, out table))
            table.RemoveObject(objID);
        else
        {
            throw new UnityException("Table for given player does not exist: " + player.ID);
        }
	}

    protected override void Awake()
    {
        this.name = GlobalSettings.PlayerObjectTableName;

        base.Awake();
    }

	private IDictionary<NetworkViewID, ObjectTable> objectTables;
}
