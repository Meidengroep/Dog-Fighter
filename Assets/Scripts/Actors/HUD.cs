using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
    public Camera Camera;

    public PlayerHealthControl HealthControl;

    public float StatusTexture_Left;
    public float StatusTexture_Top;
    public float StatusTexture_Width;
    public float StatusTexture_Height;
    public Texture2D HullTexture;
    public Texture2D ShieldTexture;

    public Texture2D HostileNpcTexture;
    public Texture2D PredictedPositionTexture;

    public SoftwareMouse Mouse;

    public GunSwitcher GunSwitcher;
    private ThirdPersonCrosshair Crosshair;

    private int team;

    void Start()
    {
        team = TeamHelper.GetTeamNumber(gameObject.layer);
        this.Crosshair = GunSwitcher.Crosshair;
    }

    void OnGUI()
    {
        if (HealthControl.DrawHealthInfo)
            DrawHealthInfo();

        DrawNpcMarkers();
    }

    private void DrawHealthInfo()
    {
        Color hullColor = Color.Lerp(Color.red, Color.green, HealthControl.CurrentHealth / HealthControl.MaxHealth);

        GUI.color = hullColor;
        GUI.Label(
            new Rect(StatusTexture_Left, StatusTexture_Top, StatusTexture_Width, StatusTexture_Height),
            new GUIContent(HullTexture));

        Color shieldColor = Color.white;
        shieldColor.a = HealthControl.CurrentShields / HealthControl.MaxShields;

        GUI.color = shieldColor;
        GUI.Label(
             new Rect(StatusTexture_Left, StatusTexture_Top, StatusTexture_Width, StatusTexture_Height),
             new GUIContent(ShieldTexture));

        GUI.color = Color.white;
        GUI.Label(
                new Rect(StatusTexture_Left + StatusTexture_Width + 10, StatusTexture_Top, StatusTexture_Width, StatusTexture_Height),
			new GUIContent("Hull: " + Mathf.Round(HealthControl.CurrentHealth) + "/" + HealthControl.MaxHealth));
        GUI.Label(
                new Rect(StatusTexture_Left + StatusTexture_Width + 10, StatusTexture_Top + 20, StatusTexture_Width, StatusTexture_Height),
			new GUIContent("Shields: " + Mathf.Round(HealthControl.CurrentShields) + "/" + HealthControl.MaxShields));
    }

    private void DrawNpcMarkers()
    {
        // Draw npc markers
        IList<GameObject> npcs;

        // Find the hostile npcs
        if (team == 1)
            npcs = GlobalSettings.Team2Npcs;
        else npcs = GlobalSettings.Team1Npcs;

        if (npcs.Count == 0)
        {
            Crosshair.ResetDistance();
            return;
        }

        GameObject closestToCursor = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < npcs.Count; i++)
        {
            Vector3 screenPosition3D = Camera.WorldToScreenPoint(npcs[i].transform.position);
            screenPosition3D = ClampToScreen(screenPosition3D);

            DrawNpcMarker(screenPosition3D);

            Vector2 screenPosition2D = new Vector2(screenPosition3D.x, screenPosition3D.y);
            float distanceToMouse = (screenPosition2D - Mouse.ScreenPosition).magnitude;

            if (distanceToMouse < closestDistance)
            {
                closestDistance = distanceToMouse;
                closestToCursor = npcs[i];
            }
        }

        CreatePredictionMarker(closestToCursor);
    }

    private void DrawNpcMarker(Vector3 screenPosition3D)
    {
        GUI.Label(new Rect(screenPosition3D.x - HostileNpcTexture.width / 2f,
                           screenPosition3D.y - HostileNpcTexture.height / 2f,
                           HostileNpcTexture.width,
                           HostileNpcTexture.height),
                  new GUIContent(HostileNpcTexture));
    }

    private void CreatePredictionMarker(GameObject npc)
    {
        ObjectTransformer transformer = npc.GetComponent<ObjectTransformer>();

        Shooter shooter = GunSwitcher.CurrentGuns[0].GetComponent<Shooter>();
        ProjectileController controller = shooter.Projectile.gameObject.GetComponent<ProjectileController>();

        Vector3 predictedPosition = PredictPosition.Predict(npc.transform.position, 
                                                            transformer.WorldTranslationDirection * transformer.TranslationSpeed, 
                                                            GunSwitcher.CurrentGuns[0].transform.position, 
                                                            controller.FlyControl.DesiredSpeed);

        Vector3 predictedScreenPosition = Camera.WorldToScreenPoint(predictedPosition);
        predictedScreenPosition = ClampToScreen(predictedScreenPosition);

        //if (predictedScreenPosition.z < 0)
          //  Crosshair.ResetDistance();
        //else Crosshair.CurrentDistance = predictedScreenPosition.z;

        GUI.Label(new Rect(predictedScreenPosition.x - PredictedPositionTexture.width / 2f,
                   predictedScreenPosition.y - PredictedPositionTexture.height / 2f,
                   PredictedPositionTexture.width,
                   PredictedPositionTexture.height),
          new GUIContent(PredictedPositionTexture));
    }

    private Vector3 ClampToScreen(Vector3 point)
    {
        point.x = Mathf.Clamp(point.x, 0, Screen.width);
        point.y = Mathf.Clamp(point.y, 0, Screen.height);

        if (point.z < 0)
        {
            if (point.x > Screen.width / 2)
                point.x = 0;
            else point.x = Screen.width;
        }
        else
            point.y = Screen.height - point.y;

        return point;
    }
}
