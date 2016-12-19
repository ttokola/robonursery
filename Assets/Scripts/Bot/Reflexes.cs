/*
    Autonomous actions which can possibly override anything else,
    such as manual control or queue executing
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class Reflexes : MonoBehaviour 
{
    [Tooltip("Automatically load the battery on low charge, halts all current actions until loaded")]
    public bool autoLoad = true;
    [Tooltip("Debug bool for checking current load status")]
    public bool loading = false;
    
	private Battery bat;
	private ManualController manControl;
	private BotQueue queue;
    private LoadBattery loader;

	void Start ()
	{
		bat = GetComponent<Battery> ();
		manControl = GetComponent<ManualController> ();
		queue = GetComponent<BotQueue> ();
        loader = GetComponent<LoadBattery> ();
        //sequence = GetComponent<SequenceExample> ();
	}
	
	void FixedUpdate ()
	{
        if (autoLoad)
        {
            if (bat.level < 30 && !loading)
            {
                loading = true;
            }
            if (loading)
            {
                // Halt current actions
                manControl.enabled = false;
                //sequence.enabled = false;
                loader.Execute();
                if (bat.normLevel >= 1.0f)
                {
                    loading = false;
                }
            }
            else
            {
                //manControl.enabled = true;
                //sequence.enabled = true;
            }            
        }
	}
}

} // End namespace