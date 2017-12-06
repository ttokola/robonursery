using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;

public class ClapHands : MonoBehaviour {
    public ArmControls armControls;
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
	void Update () {
        switch (state)
        {
            case 0:
                break;
            case 1:
                armControls.SetStaticPosition("idle");
                if (movementControls.TurnTo(clapTarget) == 0)
                {
                    state = 2;
                    //helloVoiceSource.PlayOneShot(helloSound);
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
    {
        clapTarget = ct;
        claps = c;
        clapcounter = 0;
        state = 1;
        return 0;
    }
}
