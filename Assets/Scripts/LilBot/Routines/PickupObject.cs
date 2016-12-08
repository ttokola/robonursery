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
    {
        targetRb = target.GetComponent<Rigidbody> ();
        targetTr = target.GetComponent<Transform> ();
        switch(state)
        {
        // Move near the object to be picked up
        case 0:
            armControls.SetStaticPosition("idle");
            if (movementControls.DriveTo(target.transform, true) == 1)
            {
                state = 1;
                leftWheel.constraints = rightWheel.constraints = RigidbodyConstraints.FreezeRotation;
            }
            return 1;
        case 1:
            targetRb.velocity = Vector3.zero;
            if (armControls.SetStaticPosition("sides") == 0)
            {
                state = 2;
            }
            return 1;
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
            return 1;
        }
        return 0;
    }
    
    public void Drop ()
    {
        state = 0;
        Destroy(joint);
    }
	
	void FixedUpdate ()
    {
        if (state == 3)
        {
            armControls.SetStaticPosition("forwardH");
            // Move relative to hands
            //var diff = handStartPos - leftHand.position;
            //targetTr.position = leftHand.position - posDiff;
        }
	}
}

} // End namespace