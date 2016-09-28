using UnityEngine;
using System.Collections;

public class SUTAutoController : MonoBehaviour 
{
	private float torqueMod = 15;
	public SUTFunctions controls;
	
	private Battery bat;
	private SUTManualController manControl;
	
	void Start ()
	{
		manControl = GetComponent<SUTManualController> ();
	}
	
	void FixedUpdate ()
	{
		if (controls.battery.normLevel < 0.1) {
			manControl.enabled = false;
			controls.DriveTo(GameObject.Find("LoadingDockElevatorR").transform);
		}
		else
		{
			manControl.enabled = true;
		}
	}
}