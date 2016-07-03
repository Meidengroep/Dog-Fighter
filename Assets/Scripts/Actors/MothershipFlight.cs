using UnityEngine;
using System.Collections;

/*
 * This script is used to make the motherships fly in circles
 */
public class MothershipFlight : MonoBehaviour {
	public float speed = 50;
	public float radius = 2000;
	//angularVelocity is the speed with which the object
	// should change its rotation (around the y axis)
	private float angularVelocity;

	// Use this for initialization
	void Start () {
		float circumference = radius * Mathf.PI * 2;
		angularVelocity = (speed / circumference) * 360;

        this.objTransformer = this.GetComponent<ObjectTransformer>();
        this.objTransformer.TranslationDirection = Vector3.forward;
        this.objTransformer.TranslationSpeed = this.speed;

        if (this.gameObject.layer == (int)Layers.Team1Actor)
            this.objTransformer.Rotation = new Vector3(0, -angularVelocity, 0);
        else if (this.gameObject.layer == (int)Layers.Team2Actor)
            this.objTransformer.Rotation = new Vector3(0, angularVelocity, 0);
	}
	
	// Update is called once per frame
	void Update () {

       

        //transform.Translate(/*transform.forward*/Vector3.forward * speed * Time.deltaTime);
        //transform.Rotate(new Vector3(0, angularVelocity * Time.deltaTime, 0));
	}

    private ObjectTransformer objTransformer;
}
//""