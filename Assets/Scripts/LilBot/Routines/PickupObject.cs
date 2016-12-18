/*
    Move to an object, pick it up, and hold it until Drop() is called
    
    Works reliably only with objects with rigidbody enabled.
    
    Routine is implemented with a state-machine, do not attempt to move
    the bot while routine is in progress
*/

using UnityEngine;
using System.Collections;

namespace LilBotNamespace
{

public class PickupObject : MonoBehaviour {

    public GameObject tar;
    public Rigidbody leftWheel, rightWheel;
    public Transform leftHand, rightHand;
    
	public ArmControls armControls;
    public MovementControls movementControls;
    
    private int state = 0;
    
    private FixedJoint joint;
    private Rigidbody targetRb;
    private Transform targetTr;
    private Vector3 posDiff;
    private Vector3 handStartPos, targetStartPos;
    private Quaternion targetStartRot;
    
    public int Execute (GameObject target)
    /*
        Call this continuously to execute the routine
        Return codes:
        0: Routine complete
        2: Routine in progress
    */
    {
        targetRb = target.GetComponent<Rigidbody> ();
        targetTr = target.GetComponent<Transform> ();
        switch(state)
        {
        // Move near the object to be picked up
        case 0:
            armControls.SetStaticPosition("idle");
            if (movementControls.DriveTo(target.transform, true) == 0)
            {
                state = 1;
                leftWheel.constraints = rightWheel.constraints = RigidbodyConstraints.FreezeRotation;
            }
            return 2;
        case 1:
            targetRb.velocity = Vector3.zero;
            if (armControls.SetStaticPosition("sides") == 0)
            {
                state = 2;
            }
            return 2;
        case 2:
            if (armControls.SetStaticPosition("forwardL") == 0)
            {
                posDiff = leftHand.position - targetTr.position;
                /*
                handStartPos = leftHand.position;
                targetStartPos = targetTr.position;
                targetStartRot = targetTr.rotation;*/
                joint = leftHand.gameObject.AddComponent<FixedJoint> ();
                joint.connectedBody = targetRb;
                leftWheel.constraints = rightWheel.constraints = RigidbodyConstraints.None;
                state = 3; // Move on to holding state, keep holding object in FixedUpdate
            }
            return 2;
        }
        return 0;
    }
    
    public void Drop ()
    // Drop the currently held object
    {
        state = 0;
        Destroy(joint);
    }
	
	void FixedUpdate ()
    {
        if (state == 3)  // Hold-object state, keep holding the object in hands until state changes
        {
            armControls.SetStaticPosition("forwardH");
            // Move relative to hands
            //var diff = handStartPos - leftHand.position;
            //targetTr.position = leftHand.position - posDiff;
        }
	}
}

} // End namespace