using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class ThrowObject : MonoBehaviour {

    public Transform leftHand;
    public Transform rightHand;

    public ArmControls armControls;
    public MovementControls movementControls;

    public float movementRange;
    public Vector3 grabOffset;
    public GameObject carryObject;
    public Transform carryTarget;
    public Transform throwTarget;

    public int state;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                break;
            case 1:
                //jostain syystä en onnistu mitenkään laittamaan defaulttiparametrejä
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
    {
        carryObject = co;
        carryTarget = ct;
        throwTarget = tt;
        grabOffset = go;
        state = 1;
        return 0;
    }

    private int setKinematic()
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
    {
        //carryObject.transform.position = leftHand.position + grabOffset;
        carryObject.transform.position = leftHand.TransformPoint(grabOffset);
        return 0;
    }
}
