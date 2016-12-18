/*
    Example of sequencing movement, object pickup and dropping using a state machine
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class SequenceTemplate : MonoBehaviour {
    
    private int state;
    
    /* Declare external objects required for this sequence here */
    
    //public GameObject obj;
    
    /* Declare controls and routines required for this sequence */
    
    //public ArmControls armControls;
    //public MovementControls movementControls;
    //public PickupObject someRoutine;

    /* Additional sequence-specific variables */
    
    //public bool loop;  // Loop-back
    
    void Start ()
    {
        state = 0;
    }
	
	void FixedUpdate ()
    {
        /*switch (state)
        {
        case 0:
            if (doSomething() == 0)
            {
                state = 1;
            }
            break;
        case 1:
            if (doSomethingElse() == 0)
            {
                state = 2;
            }
            break;
        case 2:
            if (doEvenMore() == 0 && loop)
            {
                state = 3;
            }
            break;
        }*/
    }
}

} // End namespace