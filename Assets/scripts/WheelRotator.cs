using UnityEngine;
using System.Collections;

public class WheelRotator : MonoBehaviour
{
	private Rigidbody rb;
	private Transform tf;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		tf = GetComponent<Transform> ();
	}
	
	public void Rotate (float torque)
	{
		rb.AddTorque(-(tf.up * torque));
	}
}
