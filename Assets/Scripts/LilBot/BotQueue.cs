using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LilBotNamespace;

namespace LilBotNamespace
{
    
public class BotQueue : MonoBehaviour {

    public Transform tr;
    
    public MovementControls controls;
	
	void Start ()
	{
	}
    
	void FixedUpdate ()
	{
        controls.DriveTo(tr);
	}
}

} // End namespace