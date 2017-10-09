using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LilBotNamespace;
public class MLAagent : Agent {
	
    public MovementControls Move;


	public override List<float> CollectState()
	{
		List<float> state = new List<float>();

        state.Add(gameObject.transform.position.x);
        state.Add(gameObject.transform.position.y);
        state.Add(gameObject.transform.position.z);

        return state;
	}

    public override void AgentStep(float[] act)
    {
        if (act[0]!=0)
        {
            Move.RotateWheel("left",act[0]);
        }
        if (act[1] != 0)
        {
            Move.RotateWheel("right", act[1]);
        }


        /*
        if (act[0] == act[1]) {
            Move.Drive(act[0]);

        }
        else if (act[0] != act[1])
        {
            Move.Turn(act[0] - act[1]);
        }
        */
    }

	public override void AgentReset()
	{

	}

	public override void AgentOnDone()
	{

	}
}
