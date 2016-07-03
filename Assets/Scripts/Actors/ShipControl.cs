using UnityEngine;
using System.Collections;

public class ShipControl : MonoBehaviour 
{
    public SoftwareMouse Mouse;
    public Camera Camera;

    public float MaxSpeed;
    public float MaxSpeedZOffset;
    public float Acceleration;

    public float MouseRotationSpeed;
    public float KeyboardRollSpeed;

    public float XCameraOffsetTweak;
    public float YCameraOffsetTweak;

    public bool SmoothRotation;
    public float SmoothingFactor;

    public bool UseCameraOffset;

    [HideInInspector]
    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { this.currentSpeed = value; }
    }   


    private Vector2 screenCenter;
    private float currentSpeed;
    private Vector3 baseCameraPosition;
    private Quaternion baseCameraRotation;

    private Vector2 smoothingGhostMouse;

    private ObjectTransformer transformer;

	void Start () 
    {
        transformer = gameObject.GetComponent<ObjectTransformer>();

        // Remember the camera position in the editor.
        baseCameraPosition = Camera.transform.localPosition;
        baseCameraRotation = Camera.transform.localRotation;

        currentSpeed = 0;
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);

        smoothingGhostMouse = Mouse.ScreenPosition;
	}
	
	void Update () 
    {
        HandleToggleInput();
        HandleMouseOrientation();
        HandleKeyboardOrientation();
        HandleKeyboardTranslation();
	}

    void OnGUI()
    {
        return;
        // Print settings info
        string smoothingOnText;
        if (SmoothRotation) 
            smoothingOnText = "on";
        else 
            smoothingOnText = "off";

        float currentLine = 0;

        if (SmoothRotation)
            GUI.color = Color.green;
        else
            GUI.color = Color.red;

        GUI.Label(new Rect(5, 5 + currentLine++ * 20, 400, 200), new GUIContent("Mouse smoothing is " + smoothingOnText + "\n"
                                                                              + "Press 'T' to toggle"));
        currentLine++;

        GUI.Label(new Rect(5, 5 + currentLine++ * 20, 400, 200), new GUIContent("Press M to toggle mouse." + "\n"
                                                                              + "(Don't use ESC!)" + "\n" 
                                                                              + "Then use the slider to change smoothing." + "\n"
                                                                              + "Higher values mean less smoothing."));
        currentLine += 3;

        SmoothingFactor = GUI.HorizontalSlider(new Rect(5, 5 + currentLine * 20, 250, 20), SmoothingFactor, 0, 15);
        GUI.Label(new Rect(270, 5 + currentLine++ * 20, 400, 200), new GUIContent(SmoothingFactor.ToString()));

        GUI.color = Color.white;

        string cameraOffsetText;
        if (UseCameraOffset)
            cameraOffsetText = "on";
        else
            cameraOffsetText = "off";

        if (UseCameraOffset)
            GUI.color = Color.green;
        else
            GUI.color = Color.red;

        GUI.Label(new Rect(5, 5 + currentLine++ * 20, 400, 200), new GUIContent("Camera offset is " + cameraOffsetText + "\n"
                                                                              + "Press 'O' to toggle"));

        // Print control info
        GUI.color = Color.green;
        GUI.Label(new Rect(Screen.width - 200, 5, 200, 200), new GUIContent("Use mouse to steer." + "\n"
                                                                          + "Use A/D to roll." + "\n"
                                                                          + "Use W/S to accelerate/decelerate."));

        GUI.color = Color.white;
    }

    void HandleToggleInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
            SmoothRotation = !SmoothRotation;

        if (Input.GetKeyDown(KeyCode.O))
            UseCameraOffset = !UseCameraOffset;

        if (Input.GetKeyDown(KeyCode.M))
        {
            Screen.lockCursor = !Screen.lockCursor;
        }
    }

    private void HandleMouseOrientation()
    {
        // Get screen position of the mouse
        Vector2 mousePos = Mouse.ScreenPosition;

        // Use the current mouse position to rotate if we don't want smoothing.
        if (!SmoothRotation)
            HandleMouseOrientation_Aux(mousePos);
        // Use a ghost mouse that follows the actual mouse otherwise.
        else
        {
            Vector2 desiredGhostDirection = mousePos - smoothingGhostMouse;
            Vector2 step = desiredGhostDirection * SmoothingFactor * Time.deltaTime;

            if (step.magnitude > desiredGhostDirection.magnitude)
                smoothingGhostMouse = mousePos;
            else
                smoothingGhostMouse += step;

            HandleMouseOrientation_Aux(smoothingGhostMouse);
        }
    }

    private void HandleMouseOrientation_Aux(Vector2 mousePos)
    {
        // Calculate the difference of the mouse with the screen center
        Vector2 difference = mousePos - screenCenter;

        // Calculate how far the mouse is from the center in percentages.
        // This is to maintain invariance with different resolutions.
        float xDistanceScreenPercentage = difference.x / screenCenter.x;
        float yDistanceScreenPercentage = difference.y / screenCenter.y;

        // Rotate proportional to the distance with the screen center.
        //transform.Rotate(Vector3.up, xDistanceScreenPercentage * MouseRotationSpeed * Time.deltaTime);
        //transform.Rotate(Vector3.right, yDistanceScreenPercentage * MouseRotationSpeed * Time.deltaTime);
        transformer.Rotation = new Vector3(yDistanceScreenPercentage * MouseRotationSpeed,
                                           xDistanceScreenPercentage * MouseRotationSpeed,
                                           transformer.Rotation.z);

        // Reset the camera to its original position.
        Camera.transform.localPosition = baseCameraPosition;
        Camera.transform.localRotation = baseCameraRotation;

        // Check if we want the camera to offset when rotating.
        if (UseCameraOffset)
        {   
            // Calculate how far the camera should offset.
            float xCameraOffset = xDistanceScreenPercentage * XCameraOffsetTweak;
            float yCameraOffset = yDistanceScreenPercentage * YCameraOffsetTweak;
            float zCameraOffset = MaxSpeedZOffset * currentSpeed / MaxSpeed;

            Vector3 translation = new Vector3(
                                              xCameraOffset,
                                              -yCameraOffset,
                                              -zCameraOffset);

            // Apply the offset.
            Camera.transform.Translate(translation);
        }
    }

    private void HandleKeyboardOrientation()
    {
        float rollRotation = 0;

        // Add the roll from both left and right to apply them at the same time.
        if (Input.GetKey(KeyCode.D))
            rollRotation -= KeyboardRollSpeed;

        if (Input.GetKey(KeyCode.A))
            rollRotation += KeyboardRollSpeed;

        // Apply the roll.
        //transform.Rotate(Vector3.forward, rollRotation);

        transformer.Rotation = new Vector3(transformer.Rotation.x,
                                           transformer.Rotation.y,
                                           rollRotation);
    }
    
    private void HandleKeyboardTranslation()
    {
        // Change the speed according to input.
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += Acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, MaxSpeed);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            currentSpeed -= Acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, MaxSpeed);
        }

        // Translate forwards with the current speed.
        //transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        transformer.Translation = Vector3.forward * currentSpeed;
    }
}
