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
	
	private float tLastMove;
	
	public Transform body;
	
	public WheelRotator left_wheel, right_wheel;
	public float rot_speed_mod;	
	
	public Battery battery;
	public XZ goTo;

	void Start () 
	{
		//rb = GetComponent<Rigidbody>();
		//kr = this.transform.Find ("Kaula").GetComponent<Rigidbody>();
		//kt = this.transform.Find ("Kaula").GetComponent<Transform>();
		//krot = kt.rotation;
		//nr = this.transform.Find ("Niska").GetComponent<Rigidbody>();
		//nt = this.transform.Find ("Niska").GetComponent<Transform>();
		battery.level = Mathf.Clamp(0.0f, battery.max, battery.level);
		tLastMove = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");
		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
			tLastMove = Time.time;
			Forward(-Input.GetAxis("Vertical"));
			Turn(-Input.GetAxis("Horizontal"));
		}
		/*if ((Time.time - tLastMove) > 1) {
			Vector3 dir = (new Vector3 (goTo.x, 0, goTo.z) - body.position).normalized;
			Quaternion lr = Quaternion.LookRotation(dir);
			Debug.Log(lr.eulerAngles);
		}*/
		/*if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
			RotateWheel("left", -Input.GetAxis ("LeftWheel"));
			RotateWheel("right", -Input.GetAxis ("RightWheel"));
		} else {
		}*/
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
	
	void Turn(float force)
	{
		RotateWheel("left", force);
		RotateWheel("right", -force);
	}
	
	void Forward(float force)
	{
		RotateWheel("left", force);
		RotateWheel("right", force);
	}
	
	void Update ()
	{
		//Debug.Log(body.right);
		//Debug.Log(Vector3.Distance(body.position, new Vector3(xz.x, 0, xz.z)));
		//Debug.Log(Vector3.Angle(
		//	new Vector3(goTo.x, 0, goTo.z),
		//	new Vector3(body.right.x, 0, body.right.z)
		//));
		//Debug.Log(body.right);
		//Debug.Log(body.transform.rotation.eulerAngles);
		//Debug.Log(new Vector3(body.position.x, 0, body.position.z));
		//Debug.Log(new Vector3(goTo.x, 0, goTo.z));
	}
}

