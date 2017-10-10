using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;
public class MLAagent : Agent {
    
    public MovementControls Move;
    public BallJoint Arms;

    public float leftWheel;
    public float rightWheel;

    public float leftShoulderX;
    public float leftShoulderY;
    public float leftShoulderZ;

    public float rightShoulderX;
    public float rightShoulderY;
    public float rightShoulderZ;

    public float leftElbowX;
    public float leftElbowY;
    public float leftElbowZ;

    public float rightEbowX;
    public float rightElbowY;
    public float rightElbowZ;

    public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.y);
        state.Add(gameObject.transform.position.z);

        return state;
	}
    //This is called every frame.
    //Currently only works if brain is set to discrete. 
    public override void AgentStep(float[] act)
    {
        leftWheel = act[0];
        rightWheel = act[1];
        //WIP
        leftShoulderX = act[2];
        leftShoulderY = act[3];
        leftShoulderZ = act[4];

        rightShoulderX = act[5];
        rightShoulderY = act[6];
        rightShoulderZ = act[7];

        leftElbowX = act[8];
        leftElbowY = act[9];
        leftElbowZ = act[10];

        rightEbowX = act[11];
        rightElbowY = act[12];
        rightElbowZ = act[13];
        
        //move the wheels using controls from Movementcontrols script
        Move.RotateWheel("left",act[0]);
        Move.RotateWheel("right", act[1]);
        reward =+ 0.001f;
        if(act[0] >9 || act[1] >9)
        {
            reward =+ 0.1f;
        }



    }

	public override void AgentReset()
	{

	}

	public override void AgentOnDone()
	{
       
    }
}
