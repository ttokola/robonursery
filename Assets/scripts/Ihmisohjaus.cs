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
		}
		else
		{
			r = 1;
		}
		if (normLevel < 0.5)
		{
			g = Mathf.InverseLerp (0.0f, 0.5f, normLevel);
		}
		else
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
	public Vector3 goTo; // For testing movement, replace with something more intelligent

	void Start () 
	{
		battery.level = Mathf.Clamp(0.0f, battery.max, battery.level);
		tLastMove = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float tAutomaticAfter = 1;
		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
		{
			tLastMove = Time.time;
			Drive(Input.GetAxis("Vertical"));
			Turn(Input.GetAxis("Horizontal"));
		}
		if (Time.time > tLastMove + tAutomaticAfter)
		{
			DriveTo(new Vector3(goTo.x, 0, goTo.z));
		}
	}
	
	void RotateWheel(string wheel, float torque)
	// Ultimately, probably all moving components should consume battery straight in the baseclass
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
		}
		else
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
	// Drive backwards (negative force) or forwards (positive force)
	{
		RotateWheel("left", force);
		RotateWheel("right", force);
	}
	
	int TurnTo(Vector3 target)
	// Turn towards a coordinate
	// Return 1 if angle to target is below threshold, -1 otherwise
	{
		float angleThreshold = 5;
		float angle = Utils.AngleTo(body.position, body.right, target);
		if (Mathf.Abs(angle) > angleThreshold)
		{
			float direction = Mathf.Sign(angle);
			Turn(1 * direction);
			return -1;
		}
		else
		{
			return 1;
		}
	}
	
	int DriveTo(Vector3 target)
	// Drive near a position
	// Return 1 if distance to target is below threshold, -1 otherwise
	{
		float distanceThreshold = 0.5f;
		if (TurnTo(target) == -1) {
			return -1;
		}
		float dist = Vector3.Distance(
			new Vector3(body.position.x, 0, body.position.z),
			new Vector3(target.x, 0, target.z)
		);
		if (dist > distanceThreshold)
		{
			//Debug.Log("autof");
			//Debug.Log(dist);
			Drive(1);
			return -1;
		}
		else if (dist < -distanceThreshold)
		{
			Debug.Log("autob");
			Debug.Log(dist);
			Drive(-1);
			return -1;
		}
		else
		{
			//Debug.Log("suc");
			//Debug.Log(dist);
			return 1;
		}
	}
	
	void Update ()
	{
		//Debug.Log(Utils.Vec3FullAngle(body.right, new Vector3(goTo.x, 0, goTo.z)));
		Debug.Log(Utils.AngleTo(body.position, body.right, goTo));
	}
}

