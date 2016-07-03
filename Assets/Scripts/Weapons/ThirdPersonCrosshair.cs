using UnityEngine;
using System.Collections;

/// <summary>
/// Draws a crosshair on a camera, given a transform to fire a collision detection ray from.
/// </summary>
public class ThirdPersonCrosshair : MonoBehaviour 
{
    public Camera Camera;

	public float MaxDistance; 

    [HideInInspector]
    public float CurrentDistance;

	//public Texture2D CrosshairTexture;

    public SoftwareMouse Mouse;

    [HideInInspector]
    public Vector3 ThreeDimensionalCrosshair;

    public bool HumanControlled { get; set; }

    //private Vector2 crosshairPosition;
	private int layerMask;

    void Awake()
    {
        this.HumanControlled = true;
    }

	void Start()
	{
        CurrentDistance = MaxDistance;

        int teamNumber = TeamHelper.GetTeamNumber(gameObject.layer);

        int teamXActorMask;
        if (teamNumber == 1)
        {
            teamXActorMask = 1 << 8;
        }
        else teamXActorMask = 1 << 11;

        int projectileMask = (1 << 10) | (1 << 13) | (1 << 2) | (1 << 3);

        layerMask = ~(teamXActorMask | projectileMask);

        ThreeDimensionalCrosshair = new Vector3(0, 0, 0);
        //crosshairPosition = new Vector2(0, 0);
	}

    public void ResetDistance()
    {
        CurrentDistance = MaxDistance;
    }
	
	void Update () 
	{
        if (!this.HumanControlled)
            return;

		RaycastHit hitInfo;

		// Fire a ray, and check for collisions.
		// Use the collision point if it exists, and the max distance if it doesnt.
        Vector3 screenPosRayPreparation = new Vector3(Mouse.ScreenPosition.x, Screen.height - Mouse.ScreenPosition.y, 0);
        Ray ray = Camera.ScreenPointToRay(screenPosRayPreparation);

        //if (Physics.Raycast(RayTransform.position, RayTransform.forward, out hitInfo, MaxDistance, layerMask))
        if (Physics.Raycast(ray, out hitInfo, CurrentDistance, layerMask))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red);
            ThreeDimensionalCrosshair = hitInfo.point;
        }
        else
        {
            ThreeDimensionalCrosshair = ray.origin + ray.direction * CurrentDistance;
            //Debug.DrawRay(ray.origin, ray.direction * CurrentDistance, Color.green);
        }

		// Obtain the position of the 3D crosshair in 2D.
        /*Vector3 screenPosition = Camera.WorldToScreenPoint(ThreeDimensionalCrosshair);

        crosshairPosition.x = screenPosition.x;
        crosshairPosition.y = Screen.height - screenPosition.y;*/
	}

    /*void OnGUI()
    {
        GUI.Label(new Rect(crosshairPosition.x - CrosshairTexture.width / 2f,
                           crosshairPosition.y - CrosshairTexture.height / 2f,
                           CrosshairTexture.width, CrosshairTexture.height), 
                  new GUIContent(CrosshairTexture));
    }*/
}
