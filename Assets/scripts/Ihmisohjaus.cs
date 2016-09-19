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
		if (normLevel > 0.5) {
			r = 1 - Mathf.InverseLerp (0.5f, 1.0f, normLevel);
		} else {
			r = 1;
		}
		if (normLevel < 0.5) {
			g = Mathf.InverseLerp (0.0f, 0.5f, normLevel);
		} else {
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
	//private Rigidbody rb;
	//private Rigidbody lwr;
	//private Rigidbody rwr;
	//private Transform lwt;
	//private Transform rwt;
	//private Rigidbody kr;
	//private Transform kt;
	//private Quaternion krot;
	//private Rigidbody nr;
	//private Transform nt;
	
	public WheelRotator left_wheel, right_wheel;
	public float rot_speed_mod;	
	
	public Battery battery;

	void Start () 
	{
		//rb = GetComponent<Rigidbody>();
		//kr = this.transform.Find ("Kaula").GetComponent<Rigidbody>();
		//kt = this.transform.Find ("Kaula").GetComponent<Transform>();
		//krot = kt.rotation;
		//nr = this.transform.Find ("Niska").GetComponent<Rigidbody>();
		//nt = this.transform.Find ("Niska").GetComponent<Transform>();
		battery.level = Mathf.Clamp(0.0f, battery.max, battery.level);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");
		RotateWheel("left", -Input.GetAxis ("LeftWheel"));
		RotateWheel("right", -Input.GetAxis ("RightWheel"));
		//float RotNeck = Input.GetAxis ("Mouse X");
		//kr.AddTorque(kt.up * speed * RotNeck);
		//float LookUp = Input.GetAxis ("Mouse Y");
		//Quaternion deltaRot = Quaternion.Euler(LookUp * nt.right * Time.deltaTime);
		//nr.MoveRotation(nr.rotation * deltaRot);
		//nr.AddTorque(nt.up * speed * LookUp);
		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
		//rb.AddForce (movement * speed);

		//Calculate rotation
		//Quaternion rotateThroat = Quaternion.AngleAxis(RotNeck*100, kt.up);
		//Quaternion rotateNeck = Quaternion.AngleAxis(LookUp*speed, nt.up);
		//Finally rotate the object accordingly
		//kr.MoveRotation(rotateThroat * krot);
		//nr.MoveRotation(nr.rotation * rotateNeck);
	}
	
	void RotateWheel(string wheel, float torque)
	{
		if (battery.normLevel <= 0) {
			return;
		}
		torque = rot_speed_mod * torque;
		if (torque != 0) {
			battery.Deplete(1);
		}
		if (wheel == "left") {
			left_wheel.Rotate(torque);
		} else {
			right_wheel.Rotate(torque);
		}

	}
	
	void Update ()
	{
		//Debug.Log(battery.level);
		//Debug.Log(battery.normLevel);
	}
}

