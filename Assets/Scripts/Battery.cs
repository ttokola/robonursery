using UnityEngine;
using System.Collections;

public class Battery : MonoBehaviour
{
	public float max;
	public Material mat;

	[SerializeField]
	private float _level;	
	public float level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = Mathf.Max(0, Mathf.Min(max, value));
		}
	}
	
	public float normLevel
	// Return battery level normalized to 0..1
	{
		get
		{
			return level/max;
		}
		set
		{
			level = max * value;
		}
	}

	void Update ()
	{	
		float r, g;
		if (normLevel > 0.5)
		{
			r = 1 - Mathf.InverseLerp (0.5f, 1.0f, normLevel);
			g = 1;
		}
		else
		{
			g = Mathf.InverseLerp (0.0f, 0.5f, normLevel);
			r = 1;
		}
		mat.SetColor ("_EmissionColor", new Color (r, g, 0.0f, 1.0f));
	}
		
	public void Deplete(float amount)
	{
		level -= amount * Time.deltaTime;
	}
	
	public void Charge(float amount)
	{
		level += amount * Time.deltaTime;
	}
	

}