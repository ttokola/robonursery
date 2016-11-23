using UnityEngine;
using System.Collections;

namespace LilBotNamespace
{

public class PickupObject : MonoBehaviour {

	public ArmControls armControls;
    public MovementControls movementControls;
    
    private int state;
    
	void Start ()
    {
	
	}
    
    int Execute (GameObject target)
    {
        switch(state)
        {
        // Move near the object to be picked up
        case 0:
            //movementControls.DriveTo(target.transform);
            break;
        }
        return 0;
    }
	
	void Update ()
    {
        //Debug.Log(body
	}
}

} // End namespace