// Autonomous actions which can possibly override anything else, like manual control or queue executing

using UnityEngine;
using System.Collections;

public class SUTReflexes : MonoBehaviour 
{
	public bool debug = false;
	
	private bool loading = false;
	private float torqueMod = 15;
	
	public SUTFunctions controls;
	private Battery bat;
	private SUTManualController manControl;
	private SUTQueue queue;

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
			loading = true;
		}
		if (loading)
		{
			// Halt current actions
			manControl.enabled = false;
			queue.enabled = false;
			
			controls.DriveTo(GameObject.Find("LoadingDockElevatorR").transform);
			if (bat.normLevel >= 1.0f)
			{
				loading = false;
			}
		}
		else
		{
			manControl.enabled = true;
			queue.enabled = true;
		}
	}
}