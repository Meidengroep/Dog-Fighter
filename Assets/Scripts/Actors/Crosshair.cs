using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {
	public Texture2D crosshairImage;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI()
	{
float xMin = (Input.mousePosition.x) - (crosshairImage.width / 2);
float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2);
	
    GUI.DrawTexture(new Rect(xMin, yMin + 90, crosshairImage.width, crosshairImage.height), crosshairImage);
	}
}
