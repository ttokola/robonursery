// Turn to direction of target transform and then clap hands

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class ClapHands : MonoBehaviour {
    [Tooltip("Drag the ArmControls script attached to the robot here")]
    public ArmControls armControls;
    [Tooltip("Drag the MovementControls script attached to the robot here")]
    public MovementControls movementControls;

    public float movementRange;
    public Transform clapTarget;

    public int state;
    public int claps;
    private int clapcounter;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
                armControls.SetStaticPosition("idle");
                if (movementControls.TurnTo(clapTarget) == 0)
                {
                    state = 2;
                }
                break;

            case 2:
                if (armControls.SetStaticPosition("clapPosition1") == 0)
                {
                    state = 3;
                }
                movementControls.TurnTo(clapTarget);
                break;
            case 3:
                if (armControls.SetStaticPosition("clapPosition2") == 0)
                {
                    clapcounter++;
                    if (clapcounter >= claps)
                    {
                        state = 4;
                    }
                    else
                    {
                        state = 2;
                    }

                }
                movementControls.TurnTo(clapTarget);
                break;
            case 4:
                state = 0;
                clapcounter = 0;
                break;
        }
	}

    public int setTask(Transform ct, int c)
    /*
     * Set hand clap task for robot 
     * 
     * Set clapping target transform and number of claps
     */
    {
        clapTarget = ct;
        claps = c;
        clapcounter = 0;
        state = 1;
        return 0;
    }

    public int getState()
    {
        return state;
    }

    public void cancelTask()
    // Stop executing task
    {
        state = 0;
    }
}
