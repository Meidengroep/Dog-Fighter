using UnityEngine;
using System.Collections;

/* Generates a certain amount of asteroids within a bounded area.
 * The position, rotation and size of each asteroid is randomized.
 */
public class AsteroidFieldGenerator : NetworkObject {
	//Amount of asteroids generated
	public int amount = 20;
	//Determine the max distance from the center that an astoid can be
	public float maxOffset = 30;
	//The range of possible sizes, relative to the template asteroid
	public float minAsteroidSize = 2;
	public float maxAsteroidSize = 8;
	//The template asteroid that is copied each time
	public GameObject templateAsteroid;

	//Keep a count of the asteroid, used to name them
	private int asteroidCount = 0;

	// Use this for initialization
	void Awake () 
    {
        if (!GlobalSettings.SinglePlayer)
        {
            if (Network.peerType == NetworkPeerType.Client)
                return;
        }

		for (int numAsteroids = 0; numAsteroids < amount; numAsteroids ++) 
        {
			CreateRandomAsteroid();
		}
	}

    public void CreateAsteroid(Vector3 position, Vector3 localScale, string name, Player owner, int id)
    {
        GameObject asteroid = (GameObject)GameObject.Instantiate(templateAsteroid, position, Random.rotation);
        asteroid.SetActive(true);
        asteroid.transform.localScale = localScale;
        asteroid.transform.name = name;
        asteroid.transform.parent = this.transform;

        if (!GlobalSettings.SinglePlayer)
        {
            if (Network.peerType == NetworkPeerType.Server)
            {
                ObjectSync objSync = asteroid.GetComponent<ObjectSync>();
                objSync.Type = ObjectSyncType.Asteroid;
                objSync.AssignID(owner, id);
            }

            base.ObjectTables.AddPlayerObject(owner, id, asteroid);
        }

        asteroidCount++;
    }

	//Creates one randomized asteroid within the bounds
	void CreateRandomAsteroid () {
		Vector3 position = GetRandomPosition();
		//Make sure that everything scales well
		position.Scale(transform.localScale);

        Vector3 localScale = new Vector3(1, 1, 1) * Random.Range(minAsteroidSize, maxAsteroidSize);

        string name = "asteroid" + asteroidCount;

        if (!GlobalSettings.SinglePlayer)
        {
            Player serverPlayer = base.NetworkControl.ThisPlayer;
            int id = base.GUIDGenerator.GenerateID();

            this.CreateAsteroid(position, localScale, name, serverPlayer, id);
        }
        else
            this.CreateAsteroid(position, localScale, name, null, 0);
	}

	//Returns a random vector3 position within the bounds
	public virtual Vector3 GetRandomPosition() {
		return new Vector3(Random.Range(transform.position.x - maxOffset, transform.position.x + maxOffset),
		                                     Random.Range(transform.position.y - maxOffset, transform.position.y + maxOffset),
		                                     Random.Range(transform.position.z - maxOffset, transform.position.z + maxOffset));
	}

}