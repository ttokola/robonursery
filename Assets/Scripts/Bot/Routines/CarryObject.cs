using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;



public class CarryObject : MonoBehaviour {

    public Transform leftHand;
    public Transform rightHand;

    public ArmControls armControls;
    public MovementControls movementControls;

    public float movementRange;
    public Vector3 grabOffset;
    public GameObject carryObject;
    public Transform carryTarget;

    public int state;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
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


    public int setTask(GameObject co, Transform ct, float mr, Vector3 go){
        carryObject = co;
        carryTarget = ct;
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
        carryObject.transform.position = leftHand.position + grabOffset;
        return 0;
    }
}
