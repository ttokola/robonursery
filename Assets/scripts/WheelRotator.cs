using UnityEngine;
using System.Collections;

public class WheelRotator : MonoBehaviour
{
	private Rigidbody rb;
	private Transform tf;
    private AudioSource engineSound;
    private float engineTime;
	
	public bool debug = true;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		tf = GetComponent<Transform> ();
        engineSound = GetComponent<AudioSource> ();
		rb.maxAngularVelocity = 100;  // Enables faster spinning of wheels
	}
	
	public void Rotate (float torque)
	{   
        if (torque != 0)
        {
            engineTime = Time.time;
        }
        
		rb.AddTorque(-(tf.up * torque));
		if (debug && torque != 0) {
			Debug.Log(string.Format("{0}: Adding {1} torque to {2}", Time.time, torque, gameObject.name));
		}
	}
    
    public void FixedUpdate ()
    {
        if (Time.time - engineTime > 0.03)
        {
            
            engineSound.volume = Mathf.Max(0f, engineSound.volume - 0.06f);
        }
        else
        {
            engineSound.volume = Mathf.Min(0.6f, engineSound.volume + 0.03f);
        }
    }
}

