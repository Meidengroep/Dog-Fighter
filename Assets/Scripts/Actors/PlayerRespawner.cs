using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerRespawner : MonoBehaviour 
{
	public GameObject SpawnAtStart;
	public GameObject RespawnCameraGO;	
	public float respawnCameraLookatDistance;
	public GUIText DeathText;
	
	private GameObject attachedPlayer;
	private bool waitingForRespawn;
	private float respawnTimer;
	private string[] deathTexts;

    private ObjectSync objSync;

    private void Awake()
    {
        this.objSync = this.GetComponent<ObjectSync>();
    }

	void Start () 
	{
		deathTexts = new string[5];
		deathTexts[0] = "It seems that you have died. Pity.";
		deathTexts[1] = "You went down like the Titanic.";
		deathTexts[2] = "So, how was the dying?";
		deathTexts[3] = "#YOLO AMIRITE?";
		deathTexts[4] = "DDDDDDEEEEEEEAAAAAAAADDDDDD.";

		if (GlobalSettings.SinglePlayer)
			Spawn (SpawnAtStart);
		SpawnAtStart = null;
	}

    public void AttachPlayer(GameObject obj)
    {
        this.attachedPlayer = obj;
    }

    public void Respawn()
    {
        this.waitingForRespawn = false;

        this.resetTransform();

        DisableRespawnCamera();

        attachedPlayer.SetActive(true);
    }

	public void NetworkSpawn(GameObject obj)
	{
		this.attachedPlayer = obj;
		this.attachedPlayer.transform.position = gameObject.transform.position;

        this.resetTransform();
		
		DisableRespawnCamera();
		attachedPlayer.SetActive(true);		
	}



	public void DisableAndWaitForSpawn(float timeBeforeRespawn)
	{
		attachedPlayer.SetActive(false);

        if (Network.peerType != NetworkPeerType.Server || GlobalSettings.SinglePlayer)
        {
           if (this.attachedPlayer.GetComponent<ObjectSync>().IsOwner)
                ActivateRespawnCamera(attachedPlayer.transform);
        }

		waitingForRespawn = true;
		respawnTimer = timeBeforeRespawn;
	}

	private void ActivateRespawnCamera(Transform lookat)
	{
		DeathText.text = deathTexts[UnityEngine.Random.Range(0, deathTexts.Length)];

		RespawnCameraGO.transform.position = lookat.position;
		RespawnCameraGO.transform.Translate(respawnCameraLookatDistance, 0, 0);
		RespawnCameraGO.transform.LookAt(lookat);

		RespawnCameraGO.SetActive(true);
		DeathText.gameObject.SetActive(true);
	}

	private void DisableRespawnCamera()
	{
		RespawnCameraGO.SetActive(false);
		DeathText.gameObject.SetActive(false);
	}

    private void Spawn(GameObject obj)
    {
        attachedPlayer = (GameObject)Instantiate(obj, gameObject.transform.position, gameObject.transform.rotation);

        this.resetTransform();

        DisableRespawnCamera();
    }

    private void resetTransform()
    {
        this.attachedPlayer.transform.position = this.gameObject.transform.position;

        if (!GlobalSettings.SinglePlayer)
        {
            GameObject mothership = null;
            if (this.attachedPlayer.gameObject.layer == (int)Layers.Team1Actor)
                mothership = GameObject.Find(GlobalSettings.Team2MothershipName);
            else if (this.attachedPlayer.gameObject.layer == (int)Layers.Team2Actor)
                mothership = GameObject.Find(GlobalSettings.Team1MothershipName);
            else
                throw new Exception("Attached player ship of spawn point has invalid layer!");

            this.attachedPlayer.transform.LookAt(mothership.transform);
        }

        ObjectTransformer transformer = this.attachedPlayer.GetComponent<ObjectTransformer>();
        transformer.Translation = Vector3.zero;

        ShipControl shipControl = this.attachedPlayer.GetComponent<ShipControl>();
        shipControl.CurrentSpeed = 0;
    }
	
	void Update () 
	{
        if (Network.peerType == NetworkPeerType.Server || GlobalSettings.SinglePlayer)
        {
            if (this.waitingForRespawn)
            {
                this.respawnTimer -= Time.deltaTime;

                if (this.respawnTimer <= 0)
                {
                    if (!GlobalSettings.SinglePlayer)
                        ObjectRPC.RespawnObject(this.objSync.Owner, this.objSync.GlobalID);
                    this.Respawn();
                }
            }
        }
	}
}
