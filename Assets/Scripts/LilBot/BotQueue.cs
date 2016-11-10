using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class BotQueue : MonoBehaviour {

    public MovementControls controls;
	
	void Start ()
	{
	}
    
	void FixedUpdate ()
	{
        controls.DriveTo(new Vector3(5, 0, 5), true);
	}
}

} // End namespace