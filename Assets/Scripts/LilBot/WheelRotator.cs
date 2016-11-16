using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class WheelRotator : MonoBehaviour
{
	private Rigidbody rb;
	private Transform tf;
	
	public bool debug = true;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		tf = GetComponent<Transform> ();
		rb.maxAngularVelocity = 100;  // Enables faster spinning of wheels
	}
	
	public void Rotate (float torque)
	{
		rb.AddTorque((tf.right * torque * 20f));
		if (debug && torque != 0) {
			Debug.Log(string.Format("{0}: Adding {1} torque to {2}", Time.time, torque, gameObject.name));
		}
	}
}

} // End namespace