using UnityEngine;
using System.Collections;

[System.Serializable]
public class XZ
{
	public int x;
	public int z;
}

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
		} else {
			r = 1;
		}
		if (normLevel < 0.5)
		{
			g = Mathf.InverseLerp (0.0f, 0.5f, normLevel);
		} else
		{
			g = 1;
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

public class Ihmisohjaus : MonoBehaviour 
{
	
	private float tLastMove;
	
	public Transform body;
	
	public WheelRotator left_wheel, right_wheel;
	public float rot_speed_mod;	
	
	public Battery battery;
	public XZ goTo;

	void Start () 
	{
		battery.level = Mathf.Clamp(0.0f, battery.max, battery.level);
		tLastMove = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float tAutomaticAfter = 5;
		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
		{
			tLastMove = Time.time;
			Drive(-Input.GetAxis("Vertical"));
			Turn(-Input.GetAxis("Horizontal"));
		}
		if (Time.time > tLastMove + tAutomaticAfter)
		{
			TurnTo(new Vector3(goTo.x, 0, goTo.z));
		}
	}
	
	void RotateWheel(string wheel, float torque)
	{
		if (battery.normLevel <= 0)
		{
			return;
		}
		torque = rot_speed_mod * torque;
		if (torque != 0)
		{
			battery.Deplete(1);
		}
		if (wheel == "left")
		{
			left_wheel.Rotate(torque);
		} else
		{
			right_wheel.Rotate(torque);
		}

	}
	
	void Turn(float force)
	// Turn left (negative force) or right (positive force)
	{
		RotateWheel("left", force);
		RotateWheel("right", -force);
	}
	
	void Drive(float force)
	// Drive forwards (positive force) or backwards (negative force)
	{
		RotateWheel("left", force);
		RotateWheel("right", force);
	}
	
	void TurnTo(Vector3 to)
	// Turn towards a coordinate
	{
		float threshold = 5;
		float angle = Utils.Vec3FullAngle(body.right, new Vector3(to.x, 0, to.z));
		if (Mathf.Abs(angle) > threshold)
		{
			Turn(-Mathf.Sign(angle));
		}
	}
	
	void Update ()
	{
		//Debug.Log(Utils.Vec3FullAngle(body.right, new Vector3(goTo.x, 0, goTo.z)));
	}
}

