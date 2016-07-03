using UnityEngine;
using System.Collections.Generic;

public class ObjectTable 
{
	public ObjectTable()
	{
		this.objectTable = new Dictionary<int, GameObject>(100);
	}

    public ICollection<GameObject> GetAllObjects()
    {
        return this.objectTable.Values;
    }

	public GameObject GetObject(int id)
	{
		GameObject obj;

		if (this.objectTable.TryGetValue(id, out obj))
			return obj;
		else
			throw new UnityException("Object with given ID does not exist: " + id);
	}

	public void AddObject(int id, GameObject obj)
	{
		if (!this.objectTable.ContainsKey(id))
			this.objectTable.Add(id, obj);
		else
			throw new UnityException("Given object ID already exists: " + id);
	}

	public void RemoveObject(int id)
	{
		if (this.objectTable.ContainsKey(id))
			this.objectTable.Remove(id);
		else
			throw new UnityException("Given object ID does not exist: " + id);
	}

	public IDictionary<int, GameObject> objectTable;

}
