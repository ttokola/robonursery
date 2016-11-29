// Autonomous actions which can possibly override anything else, like manual control or queue executing

using UnityEngine;
using System.Collections;

public class SUTReflexes : MonoBehaviour 
{
	public bool debug = false;	
    public bool loading = false;
    public Vector3 destination;
    
	private Battery bat;
	private SUTManualController manControl;
	private SUTQueue queue;
    public AudioSource fullCharge;
    public AudioSource lowCharge;

	void Start ()
	{
		bat = GetComponent<Battery> ();
		manControl = GetComponent<SUTManualController> ();
		queue = GetComponent<SUTQueue> ();
	}
	
	void FixedUpdate ()
	{
		if (bat.level < 30 && ! loading)
		{
			if (debug) { Debug.Log("Critical battery level, autoloading"); }
            destination = GameObject.Find("LoadingDockElevatorR").transform.position;
			loading = true;
            queue.hasDestination = false;
            lowCharge.Play();
		}
		if (loading)
		{
			// Halt current actions
			manControl.enabled = false;
			if (bat.normLevel >= 1.0f)
			{
				loading = false;
                queue.hasDestination = false;
                fullCharge.Play();
			}
		}
		else
		{
			manControl.enabled = true;
		}
	}
}