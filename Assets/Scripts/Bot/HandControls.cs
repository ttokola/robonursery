/*
 * Control hand grab and release
 * Attach this script to robots hand
 * NOTE: If needed, you have to disable IgnoreCollision script to enable collision detection
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControls : MonoBehaviour {

    [Tooltip("Drag the transform of the robots hand here")]
    public Transform hand;

    private FixedJoint handAttachJoint;
    private Rigidbody rbOnContact;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    // Check if some object with rigidbody touches robots hand
    {
        if (collision.collider.GetComponent<Rigidbody>() != null)
        {
            rbOnContact = collision.collider.GetComponent<Rigidbody>();
        }
    }

    void OnCollisionExit(Collision collision)
    // Register when robot is not contact with some object anymore
    {
        rbOnContact = null;
    }

    public int Grab()
    /*
     * Grabbing function intended to be used by SUT AI
     * 
    Return codes:
     *  0: No item with rigidbody to be grabbed touching this hand
     *  1: Grab done successfully
     *  2: Hand is already holding item
     */
    {
        if (handAttachJoint != null)
        {
            return 2;
        }

        if (rbOnContact == null)
        {
            return 0;
        }

        handAttachJoint = hand.gameObject.AddComponent<FixedJoint>();
        handAttachJoint.connectedBody = rbOnContact;
        return 1;
    }

    public int Grab(Rigidbody targetRb)
   /* 
    * Grabbing function intended to be used by actor robots
    * Does not require actually touching object
    * 
    Return codes:
    *  1: Grab done successfully
    *  2: Hand is already holding item
    */
    {
        if (handAttachJoint != null)
        {
            return 2;
        }

        handAttachJoint = hand.gameObject.AddComponent<FixedJoint>();
        handAttachJoint.connectedBody = targetRb;
        return 1;
    }

    public int Release()
    /*
     * Release grabbed item
     * 
     * Return codes:
     * 0: Not holding any item currently
     * 1: Item released successfully
     */
    {
        if (handAttachJoint != null)
        {
            Destroy(handAttachJoint);
            handAttachJoint = null;
            return 1;
        }
        return 0;       
    }
}
