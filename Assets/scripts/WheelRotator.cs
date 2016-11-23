using UnityEngine;
using System.Collections;

public class WheelRotator : MonoBehaviour
{
	private Rigidbody rb;
	private Transform tf;
    private AudioSource sound;
    private float engineTime;
	
	public bool debug = true;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		tf = GetComponent<Transform> ();
        sound = GetComponent<AudioSource> ();
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
            sound.volume = Mathf.Max(0f, sound.volume - 0.02f);
        }
        else
        {
            sound.volume = Mathf.Min(0.2f, sound.volume + 0.01f);
        }
    }
}

