/*
 * Grab object and carry it to the defined position
 * Set carrying task for robot by using setTask function
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;



public class CarryObject : MonoBehaviour {

    [Tooltip("Drag the Transform of left hand of robot here")]
    public Transform leftHand;
    [Tooltip("Drag the Transform of right hand of robot here")]
    public Transform rightHand;

    [Tooltip("Drag the ArmControls script attached to the robot here")]
    public ArmControls armControls;
    [Tooltip("Drag the MovementControls script attached to the robot here")]
    public MovementControls movementControls;

    public float movementRange;
    public Vector3 grabOffset;
    public GameObject carryObject;
    public Transform carryTarget;

    public int state;

	// Use this for initialization
	void Start () {

	}
	
	void Update ()
    /* Task execution working as state machine
     * If state is 0 then execution does not happen
     */
    {
        switch (state)
        {
            case 0:
                break;
            case 1:
                if (movementControls.DriveTo(carryObject.transform, true) == 0)
                {
                    state = 2;
                    
                }
                break;
            case 2:
                if (armControls.SetStaticPosition("forwardL") == 0)
                {
                    state = 3;
                    setKinematic();
                }
                break;
            case 3:
                attachTargetToHand();
                if (armControls.SetStaticPosition("forwardCarry") == 0)
                {
                    state = 4;
                }

                
                break;
            case 4:
                if (movementControls.DriveTo(carryTarget, true) == 0)
                {
                    state = 5;
                }
                armControls.SetStaticPosition("forwardCarry");
                attachTargetToHand();
                break;
            case 5:
                setNormal();
                state = 0;
                break;
        }
		
	}


    public int setTask(GameObject co, Transform ct, float mr, Vector3 go)
    /*
     * Set carrying task for robot 
     * 
     * Set target object, target location where object is to be carried and offset vector for carrying object
     */
    {
        carryObject = co;
        carryTarget = ct;
        grabOffset = go;
        movementRange = mr;
        state = 1;
        return 0;
    }

    private int setKinematic()
    //Sets carried object (including child objects) to be kinematic
    {
        Component[] rbs;
        rbs = carryObject.GetComponentsInChildren(typeof(Rigidbody));

        if (rbs != null)
        {
            foreach (Rigidbody r in rbs)
            {
                r.isKinematic = true;
                Collider c = r.GetComponent<Collider>();
                if (c != null)
                {
                    r.GetComponent<Collider>().enabled = false;
                }
            }
        }
        return 0;
    }

    private int setNormal()
    //Sets carried object (including child objects) to be normal
    {
        Component[] rbs;
        rbs = carryObject.GetComponentsInChildren(typeof(Rigidbody));

        if (rbs != null)
        {
            foreach (Rigidbody r in rbs)
            {
                r.isKinematic = false;
                Collider c = r.GetComponent<Collider>();
                if (c != null)
                {
                    r.GetComponent<Collider>().enabled = true;
                }
            }
        }
        return 0;
    }

    private int attachTargetToHand()
    // When carrying object, this function is called to attach kinematic object to robot hands
    {
        carryObject.transform.position = leftHand.position + grabOffset;
        return 0;
    }

    public int getState()
    {
        return state;
    }

    public void cancelTask()
    // Stop executing task
    {
        if (state != 0)
        {
            setNormal();
        }
        state = 0;

    }
}
