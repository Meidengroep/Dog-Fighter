using UnityEngine;
using System.Collections;


/// <summary>
/// Destroys the GameObject when a collision ends.
/// </summary>
public class DestroyAfterCollide : MonoBehaviour 
{
	void OnCollisionExit(Collision info) 
	{
		if (info.collider.tag != "Player")
			Network.Destroy(gameObject);
	}
}
