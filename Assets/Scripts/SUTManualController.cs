using UnityEngine;
using System.Collections;

public class SUTManualController : MonoBehaviour 
{
	public float torqueMod = 10;
	public bool enable;
	public SUTFunctions controls;
	
	void FixedUpdate ()
	{
		if (! enable) {
			return;
		}
		controls.Drive(Input.GetAxis("Vertical") * torqueMod);
		controls.Turn(Input.GetAxis("Horizontal") * torqueMod);
	}
}