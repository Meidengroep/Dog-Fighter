using UnityEngine;
using System.Collections;

public class MothershipHealthControl : HealthControl 
{
    public GameObject ExplosionGraphic;
    public AudioClip ExplosionSound;

    public float ExplosionScale;

    private bool dead;
    private int team;

    protected override void Initialize()
    {
        team = TeamHelper.GetTeamNumber(gameObject.layer);
        dead = false;
    }

    public override void Die()
    {
        if (dead)
            return;

        this.health = 0;

        if (Network.peerType != NetworkPeerType.Server)
        {
            GameObject explinst = Instantiate(ExplosionGraphic, gameObject.transform.position, Quaternion.identity) as GameObject;
            explinst.transform.localScale *= ExplosionScale;
            AudioSource.PlayClipAtPoint(ExplosionSound, gameObject.transform.position);
        }

        if (!GlobalSettings.SinglePlayer)
        {
            MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();
            matchControl.ObjectDestroyed(this.gameObject);
        }

        ObjectSync objSync = this.GetComponent<ObjectSync>();

        if (objSync != null && !GlobalSettings.SinglePlayer)
            objSync.Dispose();

        dead = true;
        base.IsDead = true;

        Destroy(gameObject, 5);
    }

    public void OnDestroy()
    {
        if (GlobalSettings.SinglePlayer)
            Application.LoadLevel("MainMenu");
    }

    void OnGUI()
    {
        if (!this.DrawHealthInfo)
            return;

        if (team == 1)
        {
            GUI.Label(new Rect(Screen.width - Screen.width * 0.1f - 200,
                               Screen.height * 0.1f,
                               200,
                               40),
			          new GUIContent("Mothership team " + team + " health: " + Mathf.Round(health) + "\n"
                                   + "Mothership team " + team + " shields: " + Mathf.Round(shieldStrength)));
        }
        else
        {
            GUI.Label(new Rect(Screen.width - Screen.width * 0.1f - 200,
                               Screen.height * 0.1f + 50,
                               200,
                               40),
			          new GUIContent("Mothership team " + team + " health: " + Mathf.Round(health) + "\n"
			               + "Mothership team " + team + " shields: " + Mathf.Round(shieldStrength)));
        }
    }
}
