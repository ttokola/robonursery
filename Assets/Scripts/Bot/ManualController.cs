/*
    Drive the bot manually. Manual controls are always enabled
    if this script is enabled.
    Do not attempt to use other movement functions while doing this.
    
    Manual arm controls not yet implemented.
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class ManualController : MonoBehaviour 
{
    [Tooltip("Modifier for the rotation of the wheels with manual controls, note that this will be further modified by the modifier in the movement controls")]
	public float torqueMod = 1;
    
    public MovementControls controls;
	
	void FixedUpdate ()
	{
		controls.Drive(Input.GetAxis("Vertical") * torqueMod);
		controls.Turn(Input.GetAxis("Horizontal") * torqueMod);
	}
}

} // End namespace