using UnityEngine;
using System.Collections;

public class HealthControl : MonoBehaviour
{
    public bool DrawHealthInfo { get; set; }
    public bool IsDead { get; set; }

    public float MaxHealth;
    public float MaxShields;
    public float HealthPerSecond;
    public float ShieldsPerSecond;
    public float ShieldRechargeDelay;

    public GameObject ShieldImpactPrefab;
    public float ShieldImpactScale = 1;

    protected float health;
    protected float shieldStrength;
    protected float currentShieldDelay;

    protected virtual void Awake()
    {
        this.DrawHealthInfo = true;
    }

    protected virtual void Start()
    {
        Initialize();

        health = MaxHealth;
        shieldStrength = MaxShields;
        this.objSync = this.GetComponent<DestroyableObjectSync>();
    }

    /// <summary>
    /// Called at the first line of the Start() method.
    /// </summary>
    protected virtual void Initialize()
    {

    }

    protected virtual void OnEnable()
    {
        health = MaxHealth;
        shieldStrength = MaxShields;
    }

    protected virtual void Update()
    {
        currentShieldDelay = Mathf.Max(currentShieldDelay - Time.deltaTime, 0);

        if (!this.IsDead)
            Heal(HealthPerSecond * Time.deltaTime, ShieldsPerSecond * Time.deltaTime);
    }

    public float CurrentHealth
    {
        get { return health; }
        set
        {
            this.health = value;
            //if (!this.CheckIfAlive())
            //    this.Die();
        }
    }
    public float CurrentShields
    {
        get { return shieldStrength; }
        set { this.shieldStrength = value; }
    }

    #region TakeDamageMethods

    public virtual void TakeDamage(float damage)
    {
        TakeDamage(Mathf.Max(0, damage - shieldStrength), damage);
    }

    public virtual void TakeDamage(float damage, Vector3 impactPoint)
    {
        TakeDamage(Mathf.Max(0, damage - shieldStrength), damage, impactPoint);
    }

    public virtual void TakeDamage(float hullDamage, float shieldDamage)
    {
        if (this.IsDead)
            return;

        if (Network.peerType != NetworkPeerType.Server && !GlobalSettings.SinglePlayer)
            return;

        currentShieldDelay = ShieldRechargeDelay;
        shieldStrength = Mathf.Max(0, shieldStrength - shieldDamage);

        health = Mathf.Max(0, health - hullDamage);

        //Debug.Log("Taking damage! " + health + " " + shieldStrength);

        this.objSync.RequestHealthSync();

        if (!this.CheckIfAlive())
        {
            // Notify others that the object should start 'dying'.
            if (!GlobalSettings.SinglePlayer)
            {
                if (!this.IsDead)
                    ObjectRPC.KillObject(this.objSync.Owner, this.objSync.GlobalID);
                // Die() will be called using RPCs on both the client and server.
                //if (this.objSync.IsDisposed)
                //    throw new UnityException("ObjectSync has already been disposed.");
            }
            else
                Die();
        }
    }

    public virtual void TakeDamage(float hullDamage, float shieldDamage, Vector3 impactPoint)
    {
        if (shieldStrength > 0)
        {
            GameObject shieldPrefabInstance = (GameObject)Instantiate(ShieldImpactPrefab, transform.position, transform.rotation);
            shieldPrefabInstance.transform.LookAt(impactPoint);
            shieldPrefabInstance.transform.parent = transform;
            shieldPrefabInstance.transform.localScale *= ShieldImpactScale;
        }

        TakeDamage(hullDamage, shieldDamage);
    }

    #endregion

    public bool CheckIfAlive()
    {
        return this.health > 0;
    }

    public virtual void Die()
    {
        if (this.IsDead)
            return;

        this.health = 0;
        this.shieldStrength = 0;

        ObjectSync objSync = this.GetComponent<ObjectSync>();

        if (objSync != null && !GlobalSettings.SinglePlayer)
            objSync.Dispose();

        this.IsDead = true;

        Destroy(gameObject);
    }

    public void Heal(float hullHealing, float shieldHealing, bool ignoreShieldDelay = false)
    {
        health = Mathf.Min(health + hullHealing, MaxHealth);
        if (ignoreShieldDelay || currentShieldDelay <= 0)
            shieldStrength = Mathf.Min(shieldStrength + shieldHealing, MaxShields);
    }

    protected DestroyableObjectSync objSync;
}
