using UnityEngine;
using System.Collections;

[System.Serializable]
public class Battery
{
	public float level, max;
	public Material mat;
	public Transform tr;

	public void Deplete(float amount)
	{
		level -= amount*Time.deltaTime;
		// Change battery indicator color
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
	
	public float normLevel
	// Return battery level normalized to 0..1
	{
		get { return level/max; }
		set { level = max * value; }
	}
}