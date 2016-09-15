using UnityEngine;
using System.Collections;

public class Ihmisohjaus : MonoBehaviour 
	{

	public float speed;
	//private Rigidbody rb;
	private Rigidbody lwr;
	private Rigidbody rwr;
	private Transform lwt;
	private Transform rwt;
	private Rigidbody kr;
	private Transform kt;
	private Quaternion krot;
	private Rigidbody nr;
	private Transform nt;

	// Use this for initialization
	void Start () 
		{
		//rb = GetComponent<Rigidbody>();	
		lwr = this.transform.Find ("VRengas").GetComponent<Rigidbody>();
		lwt = this.transform.Find ("VRengas").GetComponent<Transform>();
		rwr = this.transform.Find ("ORengas").GetComponent<Rigidbody>();
		rwt = this.transform.Find ("ORengas").GetComponent<Transform>();
		//kr = this.transform.Find ("Kaula").GetComponent<Rigidbody>();
		//kt = this.transform.Find ("Kaula").GetComponent<Transform>();
		//krot = kt.rotation;
		//nr = this.transform.Find ("Niska").GetComponent<Rigidbody>();
		//nt = this.transform.Find ("Niska").GetComponent<Transform>();
		}
	
	// Update is called once per frame
	void FixedUpdate ()
		{
		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");
		float moveLeft = Input.GetAxis ("LeftWheel");
		lwr.AddTorque(-lwt.up * speed * moveLeft);
		float moveRight = Input.GetAxis ("RightWheel");
		rwr.AddTorque(-rwt.up * speed * moveRight);
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
	}

