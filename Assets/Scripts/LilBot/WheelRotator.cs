using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class WheelRotator : MonoBehaviour
{
    private float lastEngineRunTime;
    private float mod = 20f;
    
    private AudioSource engineSound;
	private Rigidbody rb;
	private Transform tf;
	
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
        torque = torque * mod;
		rb.AddTorque(tf.right * torque);
        if (torque != 0)
        {
            lastEngineRunTime = Time.time;
        }
		if (debug && torque != 0)
        {
			Debug.Log(string.Format("{0}: Adding {1} torque to {2}", Time.time, torque, gameObject.name));
		}
	}
    
    public void FixedUpdate ()
    {
        if (Time.time - lastEngineRunTime > 0.03)  // Wheels not driven recently
        {
            engineSound.volume = Mathf.Max(0f, engineSound.volume - 0.06f);
        }
        else  // Wheels driven recently
        {
            engineSound.volume = Mathf.Min(0.6f, engineSound.volume + 0.03f);
        }
    }
}

} // End namespace