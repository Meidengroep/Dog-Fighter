using UnityEngine;
using System.Collections;

public class SoftwareMouse : MonoBehaviour 
{
    public float Sensitivity;

    public Vector2 ScreenPosition { get { return screenPosition; } }

    public Texture2D Cursor;
    public bool ShowCursor;

    public bool UseCustomBoundary = false;
    public float CustomBoundaryRadius = 0;

    private Vector2 screenPosition;
    private Vector2 screenCenter;

    void Start () 
    {        
        Screen.lockCursor = true;

        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        screenPosition = screenCenter;

        CalculateNewPosition();
	}

    void Update()
    {
        CalculateNewPosition();
    }

    void OnGUI()
    {
        if (ShowCursor)
            GUI.Label(
                new Rect(screenPosition.x - Cursor.width / 2,
                         screenPosition.y - Cursor.height / 2,
                         Cursor.width, 
                         Cursor.height), 
                new GUIContent(Cursor));
    }

    private void CalculateNewPosition()
    {
        screenPosition.x += Input.GetAxis("Mouse X") * Time.deltaTime * Sensitivity;
        screenPosition.y -= Input.GetAxis("Mouse Y") * Time.deltaTime * Sensitivity;

        screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

        if (UseCustomBoundary)
        {
            Vector2 differenceWithCenter = screenPosition - screenCenter;
            float maxDifference = CustomBoundaryRadius * Screen.height/  2;

            if (differenceWithCenter.magnitude > maxDifference)
                screenPosition = screenCenter + differenceWithCenter.normalized * maxDifference;
        }
    }
}
