/*
 * Grab object and carry it to the defined position and throw it to defined target
 * Set throwind task for robot by using setTask function
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class ThrowObject : MonoBehaviour {

    [Tooltip("Drag the Transform of left hand of robot here")]
    public Transform leftHand;
    [Tooltip("Drag the Transform of right hand of robot here")]
    public Transform rightHand;

    [Tooltip("Drag the ArmControls script attached to the robot here")]
    public ArmControls armControls;

    [Tooltip("Drag the MovementControls script attached to the robot here")]
    public MovementControls movementControls;

    private float movementRange;
    private Vector3 grabOffset;
    private GameObject carryObject;
    private Transform carryTarget;
    private Transform throwTarget;

    public int state;

    private float stopCounter;

    void Start()
    {

    }

    void Update()
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
                carryObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                if (armControls.SetStaticPosition("forwardL") == 0)
                {
                    state = 3;
                    setKinematic();
                    attachTargetToHand();
                }
                break;
            case 3:
                
                if (movementControls.DriveTo(carryTarget, true) == 0)
                {
                    state = 4;
                }
                armControls.SetStaticPosition("forwardL");
                attachTargetToHand();

                break;
            case 4:
                
                if (movementControls.TurnTo(throwTarget) == 0)
                {
                    state = 5;
                }
                armControls.SetStaticPosition("forwardL");
                attachTargetToHand();
                break;
            case 5:
                setNormal();

                int throwForce = 5;

                Vector3 up_v = new Vector3(0, throwForce, 0);
                Vector3 direction_v = throwTarget.position - carryObject.transform.position;
                direction_v.Normalize();

                carryObject.GetComponent<Rigidbody>().velocity = up_v + (direction_v * throwForce);
                state = 6;
                break;

            case 6:
                stopCounter -= Time.deltaTime;

                if (stopCounter < 0)
                {
                    state = 7;
                }
                if (armControls.SetStaticPosition("up") == 0)
                {
                    state = 7;
                }
                break;

            case 7:

                state = 0;
                break;
        }

    }


    public int setTask(GameObject co, Transform ct, Transform tt, float mr, Vector3 go)
    /*
    * Set throwing task for robot 
    * 
    * Set target object, target location where object is to be carried, and target location to which direction object is to be thrown and offset vector for carrying object
    */
    {
        carryObject = co;
        carryTarget = ct;
        throwTarget = tt;
        grabOffset = go;
        state = 1;
        stopCounter = 0.3f;
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
    // Attach object to hand by adding offset vector to robots hand transform
    {
        carryObject.transform.position = leftHand.TransformPoint(grabOffset);
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
