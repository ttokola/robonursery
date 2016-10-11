// Autonomous actions which can possibly override anything else, like manual control or queue executing

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class Reflexes : MonoBehaviour 
{
	public bool debug = false;	
    public bool loading = false;
    public Vector3 destination;
    
	private Battery bat;
	private ManualController manControl;
	private BotQueue queue;

	void Start ()
	{
		bat = GetComponent<Battery> ();
		manControl = GetComponent<ManualController> ();
		queue = GetComponent<BotQueue> ();
	}
	
	void FixedUpdate ()
	{
		if (bat.level < 30 && ! loading)
		{
			if (debug) { Debug.Log("Critical battery level, autoloading"); }
            destination = GameObject.Find("LoadingDockElevatorR").transform.position;
			loading = true;
            queue.hasDestination = false;
		}
		if (loading)
		{
			// Halt current actions
			manControl.enabled = false;
			if (bat.normLevel >= 1.0f)
			{
				loading = false;
                queue.hasDestination = false;
			}
		}
		else
		{
			manControl.enabled = true;
		}
	}
}

} // End namespace