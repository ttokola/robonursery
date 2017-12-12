/*
    Example of sequencing movement, object pickup and dropping using a state machine
*/

using UnityEngine;
using System.Collections;

using LilBotNamespace;

namespace LilBotNamespace
{

public class MoveAndPickup : MonoBehaviour {
    
    private int state;
    
    public GameObject ball;
    
    public ArmControls armControls;
    public MovementControls movementControls;
    public PickupObject pickupRoutine;
    public bool loop;
    
    void Start ()
    {
        state = 0;
    }
	
	void FixedUpdate ()
    {
        var pos1 = new Vector3(-5, 0, 0);
        var pos2 = new Vector3(5, 0, 0);
        //Debug.Log(string.Format("Sequence at state {0}", state));
        switch (state)
        {
        // Move to 0,0,0 position
        case 0:
            armControls.SetStaticPosition("idle");
            if (movementControls.DriveTo(Vector3.zero, true) == 0)
            {
                state = 1;
            }
            break;
        // Pick up ball (move towards it through pickup routine automatically if needed)
        case 1:
            if (pickupRoutine.Execute(ball) == 0)
            {
                state = 2;
            }
            break;
        // Move towards first position, drop ball
        case 2:
            if (movementControls.DriveTo(pos1, true) == 0)
            {
                pickupRoutine.Drop();
                state = 3;
            }
            break;
        // Move to 0,0,0 position again
        case 3:
            armControls.SetStaticPosition("idle");
            if (movementControls.DriveTo(Vector3.zero, true) == 0)
            {
                state = 4;
            }
            break;
        // Pick up ball (move towards it through pickup routine automatically if needed)
        case 4:
            if (pickupRoutine.Execute(ball) == 0)
            {
                state = 5;
            }
            break;
        // Move towards second position, drop ball, loop back to beginning if required
        case 5:
            if (movementControls.DriveTo(pos2, true) == 0)
            {
                pickupRoutine.Drop();
                if (loop)
                {
                    state = 0;
                }
            }
            break;
        }
    }
}

} // End namespace