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
    
    private int state;
    
    private Vector3 handStartPos, targetStartPos;
    private Quaternion targetStartRot;
    
	void Start ()
    {
	
	}
    
    int Execute (GameObject target)
    {
        var targetRb = target.GetComponent<Rigidbody> ();
        var targetTr = target.GetComponent<Transform> ();
        switch(state)
        {
        // Move near the object to be picked up
        case 0:
            armControls.SetStaticPosition("idle");
            if (movementControls.DriveTo(target.transform) == 1)
            {
                state = 1;
                leftWheel.constraints = rightWheel.constraints = RigidbodyConstraints.FreezeRotation;
            }
            break;
        case 1:
            targetRb.velocity = Vector3.zero;
            if (armControls.SetStaticPosition("sides") == 0)
            {
                state = 2;
            }
            break;
        case 2:
            if (armControls.SetStaticPosition("forwardL") == 0)
            {
                handStartPos = leftHand.position;
                targetStartPos = targetTr.position;
                targetStartRot = targetTr.rotation;
                state = 3;
            }
            break;
        case 3:
            armControls.SetStaticPosition("forwardH");
            // Move relative to hands
            var diff = handStartPos - leftHand.position;
            targetTr.position = targetStartPos - diff;
            break;
        }
        return 0;
    }
	
	void Update ()
    {
        Execute(tar);
	}
}

} // End namespace