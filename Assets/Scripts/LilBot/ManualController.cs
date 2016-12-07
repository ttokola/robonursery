using UnityEngine;
using System.Collections;

using LilBotNamespace;

public enum ArmState
{
    waving, idle, sides, forward, back, down, up
}

namespace LilBotNamespace
{
    
public class ManualController : MonoBehaviour 
{
    public ArmState armState = ArmState.idle;
	public float torqueMod = 1;
	public bool enable;
    public ArmControls armControls;
	public MovementControls controls;
    public bool fall = false;
    
    public Rigidbody body;
	
	void FixedUpdate ()
	{
		if (! enable) {
			return;
		}
		controls.Drive(Input.GetAxis("Vertical") * torqueMod);
		controls.Turn(Input.GetAxis("Horizontal") * torqueMod);
        
        if (armState == ArmState.idle)
        {
            armControls.SetStaticPosition("idle");
        }
        else if (armState == ArmState.sides)
        {
            armControls.SetStaticPosition("sides");
        }
        else if (armState == ArmState.forward)
        {
            armControls.SetStaticPosition("forward");
        }
        else if (armState == ArmState.back)
        {
            armControls.SetStaticPosition("back");
        }
        else if (armState == ArmState.down)
        {
            armControls.SetStaticPosition("down");
        }
        else if (armState == ArmState.up)
        {
            armControls.SetStaticPosition("up");
        }
        else if (armState == ArmState.waving)
        {
            armControls.Wave();;
        }
        
        if (fall)
        {
            body.constraints = RigidbodyConstraints.None;
        }
	}
}

} // End namespace