using UnityEngine;
using System.Collections.Generic;

public class GUIDGenerator : MonoBehaviour {

	public int GenerateID()
	{
		if (this.recyledIDs.Count > this.recycleDelay)
			return this.recyledIDs.Dequeue();
		else
		{
			this.currentID++;

			return this.currentID;
		}
	}

	public void RecycleID(int id)
	{
		this.recyledIDs.Enqueue(id);
	}

	// Use this for initialization
	void Start () {
		this.recycleDelay = 5000;
		this.currentID = int.MinValue;
		this.recyledIDs = new Queue<int>(this.recycleDelay * 2);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private int recycleDelay;
	private int currentID;
	private Queue<int> recyledIDs;
}
