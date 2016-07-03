using UnityEngine;
using System.Collections;

/// <summary>
/// Deletes the GameObject after it has lived for a certain amount of time.
/// </summary>
public class Tidy : MonoBehaviour 
{
	public float TimeToLive;

	void Start () 
	{
		Destroy(gameObject, TimeToLive);
	}
}
