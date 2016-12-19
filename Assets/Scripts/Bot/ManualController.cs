/*
    Drive the bot manually
    Do not attempt to use other movement functions while doing this
    
    Manual arm controls not implemented
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class ManualController : MonoBehaviour 
{
	public float torqueMod = 1;
	public bool enable;
	public MovementControls controls;
    
    public Rigidbody body;
	
	void FixedUpdate ()
	{
		if (! enable) {
			return;
		}
		controls.Drive(Input.GetAxis("Vertical") * torqueMod);
		controls.Turn(Input.GetAxis("Horizontal") * torqueMod);
	}
}

} // End namespace