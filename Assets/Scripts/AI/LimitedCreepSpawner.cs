using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LimitedCreepSpawner : MonoBehaviour 
{
	public GameObject[] WavePrefabs;
	public int MaxActiveWaves;
	public float TimeBeforeFirstWave;
	public float TimeBetweenWaves;

	private float spawnTimer;
	private List<GameObject[]> activeWaves;

	// TODO get this transform from code or something
	public Transform DroneTarget;

	void Start () 
	{
		activeWaves = new List<GameObject[]>(MaxActiveWaves);
	}

	void Update () 
	{
		spawnTimer = Mathf.Max(0, spawnTimer - Time.deltaTime);

		for (int i = 0; i < activeWaves.Count; i++)
		{
			bool allGone = true;
			for (int j = 0; j < activeWaves[i].Length; j++)
				if (activeWaves[i][j] != null)
					allGone = false;

			if (allGone)
			{
				activeWaves.RemoveAt(i);
				i--;
			}
		}

		if (spawnTimer <= 0)
			if (activeWaves.Count < MaxActiveWaves)
			{
				activeWaves.Add(SpawnWave());
				spawnTimer = TimeBetweenWaves;
			}
	}

	private GameObject[] SpawnWave()
	{
		GameObject[] newWave = new GameObject[WavePrefabs.Length];

		for (int i = 0; i < WavePrefabs.Length; i++)
		{
			newWave[i] = (GameObject)Instantiate(WavePrefabs[i], gameObject.transform.position, gameObject.transform.rotation);
			newWave[i].tag = "Npc";
			newWave[i].layer = gameObject.layer;
			DroneBehaviour behav = newWave[i].GetComponent<DroneBehaviour>();
			behav.target = DroneTarget;
		}

		return newWave;
	}
}
