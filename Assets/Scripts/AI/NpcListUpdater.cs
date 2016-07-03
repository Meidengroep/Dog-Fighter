using UnityEngine;
using System.Collections;

public class NpcListUpdater : MonoBehaviour 
{
	void Start () 
    {
        GlobalSettings.AddNpc(gameObject);
	}

    void OnDestroy()
    {
        GlobalSettings.RemoveNpc(gameObject);
    }
}
