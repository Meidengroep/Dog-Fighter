using UnityEngine;
using System.Collections;

public class ObjectTransformer : MonoBehaviour {
	
    /// <summary>
    /// Gets or sets the translational speed per second in the translation direction.
    /// </summary>
	public float TranslationSpeed
	{
		get { return this.translationSpeed; }
		set
		{
			if (value == this.translationSpeed)
				return;
			
			this.translationSpeed = value;
			
			this.recalculateTranslation();
		}
	}
    /// <summary>
    /// Get or sets the normalized direction of the translation.
    /// </summary>
	public Vector3 TranslationDirection
	{
		get { return this.translationDirection; }
		set
		{
			this.translationDirection = value.normalized;
			
			this.recalculateTranslation();
		}
	}
    /// <summary>
    /// Gets or sets the translation per second. This value implicitly contains
    /// translation speed and direction.
    /// </summary>
	public Vector3 Translation
	{
		get { return this.translation; }
		set
		{
			this.translation = value;			
			this.translationSpeed = value.magnitude;
			this.translationDirection = value.normalized;
		}
	}
	
    /// <summary>
    /// Gets or sets the rotation per second in euler-angles.
    /// </summary>
	public Vector3 Rotation
	{
		get { return this.rotation; }
		set 
		{
			this.rotation = value;
		}
	}

    /// <summary>
    /// Gets the translation direction in world coordinates.
    /// </summary>
    public Vector3 WorldTranslationDirection
    {
        get { return transform.localToWorldMatrix * translationDirection; }
    }
	
	// Use this for initialization
	protected virtual void Start () {
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		this.transform.Translate(this.Translation * Time.deltaTime);
		this.transform.Rotate(this.Rotation.x * Time.deltaTime, 0, 0, Space.Self);
		this.transform.Rotate(0, this.Rotation.y * Time.deltaTime, 0, Space.Self);
        this.transform.Rotate(0, 0, this.Rotation.z * Time.deltaTime, Space.Self);
	}
	
	private void recalculateTranslation()
	{
		this.translation = this.translationDirection * this.translationSpeed;		
	}
	
	private float translationSpeed;
	private Vector3 translationDirection;
	private Vector3 translation;
	
	private Vector3 rotation;
}
